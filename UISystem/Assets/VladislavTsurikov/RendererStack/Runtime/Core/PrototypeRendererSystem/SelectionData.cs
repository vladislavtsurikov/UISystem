using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Core.RenderManager.GPUInstancedIndirect;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem
{
    [Serializable]
    public class SelectionData
    {
        [OdinSerialize]
        public List<Group> GroupList = new() { new Group("Default", true) };

        public readonly SelectedData SelectedData = new();
        private PrototypeRenderer _prototypeRenderer;
        private Type _prototypeType;

        public List<Prototype> PrototypeList
        {
            get
            {
                var prototypes = new List<Prototype>();

                foreach (Group group in GroupList)
                {
                    prototypes.AddRange(group.PrototypeList);
                }

                return prototypes;
            }
        }

        public Type PrototypeType
        {
            get
            {
                if (_prototypeType == null)
                {
                    if (RendererType != null)
                    {
                        _prototypeType = RendererType.GetAttribute<PrototypeTypeAttribute>().Type;
                        return _prototypeType;
                    }

                    return null;
                }

                return _prototypeType;
            }
        }

        public Type RendererType => _prototypeRenderer.GetType();

        public void Setup(PrototypeRenderer prototypeRenderer)
        {
            _prototypeRenderer = prototypeRenderer;

            CheckPrototypeChanges();
            SetupGroups();
            SelectedData.RefreshSelectedParameters(this);
        }

        public void SetupGroups()
        {
            if (GroupList == null || GroupList.Count == 0)
            {
                var group = new Group("Default", true);
                GroupList = new List<Group> { group };
            }
            else
            {
                if (SelectedData.GetLastGroup() == null)
                {
                    GroupList[0].Selected = true;
                    SelectedData.RefreshSelectedParameters(this);
                }
            }

            foreach (Group group in GroupList)
            {
                group.Setup(RendererType);
            }
        }

        public Prototype AddMissingPrototype(Prototype proto)
        {
            if (proto == null || proto.PrototypeObject == null)
            {
                return null;
            }

            if (GetProto(proto.ID) != null)
            {
                return proto;
            }

            if (SelectedData.GetLastGroup() == null)
            {
                GroupList[0].Selected = true;
                SelectedData.RefreshSelectedParameters(this);
            }

            SelectedData.GetLastGroup().PrototypeList.Add(proto);

            RendererStackManager.Instance.Setup(true);

            return proto;
        }

        public Prototype AddMissingPrototype(Object obj) =>
            AddMissingPrototype(
                PrototypesStorage.Instance.GeneratePrototypeIfNecessary(obj, PrototypeType, RendererType));

        public void RemovePrototypes(List<Prototype> protoList, bool removePrototypeFromAllScenes = false)
        {
            foreach (Group group in GroupList)
            {
                group.PrototypeList.RemoveAll(protoList.Contains);
            }

            if (removePrototypeFromAllScenes)
            {
                PrototypesStorage.Instance.Remove(protoList, RendererType);
            }

            RendererStackManager.Instance.Setup(true);
        }

        public void DeleteInvalidPrototypes()
        {
            var happenedDelete = false;

            foreach (Group group in GroupList)
            {
                if (group.DeleteInvalidPrototypes(RendererType))
                {
                    happenedDelete = true;
                }
            }

            if (happenedDelete)
            {
                RendererStackManager.Instance.Setup(true);
            }
        }

        public Prototype GetProto(Object obj)
        {
            foreach (Prototype proto in PrototypeList)
            {
                if (proto.IsSamePrototypeObject(obj))
                {
                    return proto;
                }
            }

            return null;
        }

        public Prototype GetProto(int id)
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

        public void CheckPrototypeChanges()
        {
            PrototypesStorage.Instance.DeleteInvalidPrototypes();
            DeleteInvalidPrototypes();
        }

        private void ChangeShaderCode()
        {
            if (Application.isPlaying)
            {
                return;
            }

            GPUInstancedIndirectShaderStack.Instance.ClearEmptyShaders();
        }
    }
}
