using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem.Utility;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem
{
    [Serializable]
    public class ComponentStackManager
    {
        private Type _addElementsAttribute;
        private Type _addGeneralElementsAttribute;

        [OdinSerialize]
        private GeneralComponentStack _generalGeneralComponentStack;

        private Type _prototypeType;

        [OdinSerialize]
        private ToolsComponentStack _toolsComponentStack;

        public GeneralComponentStack GeneralComponentStack => _generalGeneralComponentStack;
        public ToolsComponentStack ToolsComponentStack => _toolsComponentStack;

        internal void SetupElementStack(Type addGeneralElementsAttribute, Type addElementsAttribute, Type prototypeType)
        {
            _prototypeType = prototypeType;
            _addGeneralElementsAttribute = addGeneralElementsAttribute;
            _addElementsAttribute = addElementsAttribute;

            _generalGeneralComponentStack ??= new GeneralComponentStack();
            _toolsComponentStack ??= new ToolsComponentStack();

            _generalGeneralComponentStack.Setup(true, new object[] { addGeneralElementsAttribute, prototypeType });
            _toolsComponentStack.Setup(true, new object[] { addElementsAttribute, prototypeType });
        }

        public List<Type> GetAllElementTypes(Type toolType, Type prototypeType = null)
        {
            List<Type> types = GetGeneralElementTypes(toolType, prototypeType);
            types.AddRange(GetElementTypes(toolType, prototypeType));

            return types;
        }

        public List<Type> GetGeneralElementTypes(Type toolType, Type prototypeType = null)
        {
            var drawTypes = new List<Type>();

            foreach (var attribute in MegaWorldComponentsUtility.GetAttributes(_addGeneralElementsAttribute,
                         _prototypeType, toolType))
            {
                var addComponentsAttribute = (AddComponentsAttribute)attribute;

                if (prototypeType != null)
                {
                    if (!addComponentsAttribute.PrototypeTypes.Contains(prototypeType))
                    {
                        continue;
                    }
                }

                drawTypes.AddRange(addComponentsAttribute.Types);
            }

            return drawTypes;
        }

        public List<Type> GetElementTypes(Type toolType, Type prototypeType = null)
        {
            var drawTypes = new List<Type>();

            foreach (var attribute in MegaWorldComponentsUtility.GetAttributes(_addElementsAttribute, _prototypeType,
                         toolType))
            {
                var addComponentsAttribute = (AddComponentsAttribute)attribute;

                if (prototypeType != null)
                {
                    if (!addComponentsAttribute.PrototypeTypes.Contains(prototypeType))
                    {
                        continue;
                    }
                }

                drawTypes.AddRange(addComponentsAttribute.Types);
            }

            return drawTypes;
        }

        internal void ResetElementsStacks(Type toolType)
        {
            foreach (Type type in GetGeneralElementTypes(toolType))
            {
                _generalGeneralComponentStack.Reset(type);
            }

            ToolComponentStack toolComponentStack = ToolsComponentStack.GetToolStackElement(toolType);

            toolComponentStack.ComponentStack.Reset();
        }

#if UNITY_EDITOR
        [NonSerialized]
        private IMGUIComponentStackEditor<Runtime_Core_Component, IMGUIElementEditor> _generalComponentStackEditor;

        public IMGUIComponentStackEditor<Runtime_Core_Component, IMGUIElementEditor> GeneralComponentStackEditor
        {
            get
            {
                if (_generalComponentStackEditor == null)
                {
                    _generalComponentStackEditor =
                        new IMGUIComponentStackEditor<Runtime_Core_Component, IMGUIElementEditor>(
                            _generalGeneralComponentStack);
                }

                return _generalComponentStackEditor;
            }
        }

        [NonSerialized]
        private ToolsComponentStackEditor _toolsComponentStackEditor;

        public ToolsComponentStackEditor ToolsComponentStackEditor
        {
            get
            {
                if (_toolsComponentStackEditor == null)
                {
                    _toolsComponentStackEditor = new ToolsComponentStackEditor(_toolsComponentStack);
                }

                return _toolsComponentStackEditor;
            }
        }
#endif

#if UNITY_EDITOR
        public void DrawToolElements(Type toolType)
        {
            List<Type> drawGeneralTypes = GetGeneralElementTypes(toolType);
            List<Type> drawTypes = GetElementTypes(toolType);

            GeneralComponentStackEditor.DrawElements(drawGeneralTypes);
            ToolsComponentStackEditor.DrawElements(toolType, drawTypes);
        }

        public void DrawElement(Type type) => GeneralComponentStackEditor.DrawElement(type);

        public void DrawElement(Type type, Type toolType) =>
            ToolsComponentStackEditor.DrawElements(toolType, new List<Type> { type });

        public void ResetElementsMenu(Type toolType)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Reset"), false, () => ResetElementsStacks(toolType));

            menu.ShowAsContext();
        }
#endif
    }
}
