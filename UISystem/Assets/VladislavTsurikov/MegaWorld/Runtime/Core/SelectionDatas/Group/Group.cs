using System;
using System.Collections.Generic;
using System.Linq;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.DefaultComponentsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Utility;
using VladislavTsurikov.ReflectionUtility;
using Object = UnityEngine.Object;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group
{
    public class Group : SerializedScriptableObject, IHasElementStack, IShowIcon
    {
        public string RenamingName = "Group";
        public bool Renaming;

        [OdinSerialize]
        private ComponentStackManager _componentStackManager;

        [OdinSerialize]
        private DefaultGroupComponentStack _defaultGroupComponentStack;

        [OdinSerialize]
        private Type _prototypeType;

        [OdinSerialize]
        private bool _selected;

        [OdinSerialize]
        public AdvancedElementList<Prototype> PrototypeList = new();

        public DefaultGroupComponentStack DefaultGroupComponentStack => _defaultGroupComponentStack;

        public Type PrototypeType => _prototypeType;

        public void OnEnable()
        {
            AllAvailableGroups.AddGroup(this);

            PrototypeList.OnRemoved -= OnRemove;
            PrototypeList.OnRemoved += OnRemove;

            if (_prototypeType != null)
            {
                SetupComponentStack();
                SetupPrototypesElementStack();
            }

            foreach (Prototype prototype in PrototypeList)
            {
                prototype.Setup();
            }
        }

        private void OnDestroy()
        {
            AllAvailableGroups.RemoveGroup(this);

            foreach (Prototype prototype in PrototypeList)
            {
                prototype.OnDisable();
            }
        }

        public ComponentStackManager ComponentStackManager => _componentStackManager;

        public void SetupComponentStack()
        {
            _componentStackManager ??= new ComponentStackManager();
            _defaultGroupComponentStack ??= new DefaultGroupComponentStack();

            _componentStackManager.SetupElementStack(typeof(AddGeneralGroupComponentsAttribute),
                typeof(AddGroupComponentsAttribute), PrototypeType);
            _defaultGroupComponentStack.Setup(true, new object[] { this });
        }

        public Runtime_Core_Component GetElement(Type elementType) =>
            _componentStackManager.GeneralComponentStack.GetElement(elementType);

        public Runtime_Core_Component GetElement(Type toolType, Type elementType) =>
            _componentStackManager.ToolsComponentStack.GetElement(toolType, elementType);

        public string Name => name;

        public bool Selected
        {
            get => _selected;
            set => _selected = value;
        }

        private void OnRemove(int index) => PrototypeList[index].OnDisable();

        internal void Init(Type prototypeType)
        {
            _prototypeType = prototypeType;

            SetupComponentStack();
            SetupPrototypesElementStack();
        }

        public void SetupPrototypesElementStack()
        {
            foreach (Prototype prototype in PrototypeList)
            {
                prototype.SetupComponentStack();
            }
        }

        public T GetDefaultElement<T>() where T : DefaultGroupComponent =>
            (T)_defaultGroupComponentStack.GetElement(typeof(T));

        public Prototype AddMissingPrototype(Object obj) => AddMissingPrototype(GeneratePrototypeIfNecessary(obj));

        private Prototype AddMissingPrototype(Prototype proto)
        {
            if (proto == null || proto.PrototypeObject == null)
            {
                return null;
            }

            if (GetPrototype(proto.ID) != null)
            {
                return proto;
            }

            PrototypeList.Add(proto);

            return proto;
        }

        private Prototype GeneratePrototypeIfNecessary(Object obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("You are adding a null object " + "(" + PrototypeType + ")");
                return null;
            }

            Prototype prototype = GetPrototype(obj);

            var id = obj.GetInstanceID();

            if (prototype == null)
            {
                prototype = (Prototype)Activator.CreateInstance(PrototypeType);
                prototype.OnCreate(id, obj);
                PrototypeList.Add(prototype);

#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }

            return prototype;
        }

        public Prototype GetPrototype(Object obj)
        {
            foreach (Prototype proto in PrototypeList)
            {
                if (proto.PrototypeObject == obj)
                {
                    return proto;
                }
            }

            return null;
        }

        public Prototype GetPrototype(int id)
        {
            foreach (Prototype proto in PrototypeList)
            {
                if (proto.ID == id)
                {
                    return proto;
                }
            }

            return null;
        }

        public List<Prototype> GetAllSelectedPrototypes()
        {
            var protoList = new List<Prototype>();

            foreach (Prototype proto in PrototypeList)
            {
                if (proto.Selected)
                {
                    protoList.Add(proto);
                }
            }

            return protoList;
        }

        public bool HasAllActivePrototypes()
        {
            foreach (Prototype proto in PrototypeList)
            {
                if (proto.Active)
                {
                    return true;
                }
            }

            return false;
        }

        public void DeleteSelectedPrototype() => PrototypeList.RemoveAll(proto => proto.Selected);

        public void SelectAllPrototypes(bool select) => PrototypeList.ForEach(proto => proto.Selected = select);

#if UNITY_EDITOR
        public bool IsRedIcon => false;
        public Texture2D PreviewTexture => null;
#endif

#if UNITY_EDITOR
        public string GetPrototypeTypeName() => PrototypeType.GetAttribute<NameAttribute>().Name.Split('/').Last();

        public void Save() => EditorUtility.SetDirty(this);

        // Sadly OnDestroy is not being called reliably by the editor. So we need this.
        private class OnDestroyProcessor : AssetModificationProcessor
        {
            private static readonly Type _type = typeof(Group);

            private static readonly string _fileEnding = ".asset";

            public static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions _)
            {
                if (!path.EndsWith(_fileEnding))
                {
                    return AssetDeleteResult.DidNotDelete;
                }

                Type assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
                if (assetType != null && (assetType == _type || assetType.IsSubclassOf(_type)))
                {
                    Group asset = AssetDatabase.LoadAssetAtPath<Group>(path);
                    asset.OnDestroy();
                }

                return AssetDeleteResult.DidNotDelete;
            }
        }
#endif
    }
}
