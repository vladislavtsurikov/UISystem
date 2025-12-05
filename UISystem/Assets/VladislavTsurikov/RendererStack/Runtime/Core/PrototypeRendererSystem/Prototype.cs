using System;
using System.Runtime.Serialization;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack;
using VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.Console;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem
{
    [Serializable]
    public abstract class Prototype : IShowIcon
    {
        [OdinSerialize]
        private bool _active = true;

        private ChangeShaderCodeAttribute _changeShaderCodeAttribute;

        [OdinSerialize]
        private int _id;

        [OdinSerialize]
        private Type _rendererType;

        [OdinSerialize]
        private bool _selected;

        [OdinSerialize]
        public PrototypeComponentStack PrototypeComponentStack;

        [NonSerialized]
        public PrototypeConsole PrototypeConsole = new();


        public RenderModel RenderModel;

        public int ID => _id;


        public bool Active
        {
            get
            {
                if (PrototypeConsole.HasError())
                {
                    return false;
                }

                return _active;
            }
            set => _active = value;
        }

        public abstract Object PrototypeObject { get; }

        public abstract LayerMask Layer { get; }

        public ChangeShaderCodeAttribute ChangeShaderCodeAttribute
        {
            get
            {
                if (_changeShaderCodeAttribute == null)
                {
                    _changeShaderCodeAttribute =
                        (ChangeShaderCodeAttribute)_rendererType.GetAttribute(typeof(ChangeShaderCodeAttribute));
                }

                return _changeShaderCodeAttribute;
            }
        }

        public bool Selected
        {
            get => _selected;
            set => _selected = value;
        }

        public abstract string Name { get; }

        [OnDeserializing]
        public void OnDeserializing()
        {
            PrototypeConsole = new PrototypeConsole();
            _active = true;
        }

        internal void Init(int id, Object obj, Type rendererType)
        {
            _id = id;
            _rendererType = rendererType;
            InitPrototype(obj);
            PrototypeComponentStack = new PrototypeComponentStack();
            PrototypeComponentStack.Setup(true, new object[] { _rendererType, this });
            PrototypeComponentStack.CreateAllComponents();
            Setup();
        }

        public void Setup()
        {
            PrototypeComponentStack.Setup(true, new object[] { _rendererType, this });

#if UNITY_EDITOR
            ChangeShaderCodeAttribute.ChangeShaderCode(this);
#endif

            RefreshRenderModelInfo();
        }

        public PrototypeComponent GetSettings(Type settingsType) => PrototypeComponentStack.GetElement(settingsType);

        protected abstract void InitPrototype(Object obj);
        public abstract MeshRenderer[] GetMeshRenderers();
        public abstract bool IsSamePrototypeObject(Object obj);
        public abstract void RefreshRenderModelInfo();

#if UNITY_EDITOR
        public bool IsRedIcon => PrototypeConsole.HasError();
        public abstract Texture2D PreviewTexture { get; }

        private PrototypeComponentStackEditor _prototypeComponentStackEditor;
        public PrototypeComponentStackEditor PrototypeComponentStackEditor
        {
            get
            {
                if (_prototypeComponentStackEditor == null)
                {
                    _prototypeComponentStackEditor = new PrototypeComponentStackEditor(PrototypeComponentStack);
                }

                return _prototypeComponentStackEditor;
            }
        }
#endif
    }
}
