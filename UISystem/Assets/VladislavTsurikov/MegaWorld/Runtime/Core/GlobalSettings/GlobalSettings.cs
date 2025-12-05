using OdinSerializer;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Editor.Core.Window.ElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings
{
    [LocationAsset("MegaWorld/GlobalSettings")]
    public class GlobalSettings : SerializedScriptableObjectSingleton<GlobalSettings>
    {
        [OdinSerialize]
        public CommonComponentStack CommonComponentStack = new();

        [OdinSerialize]
        public ToolsComponentStack ToolsComponentStack = new();

        private void OnEnable()
        {
            ToolsComponentStack.Setup();
            CommonComponentStack.Setup();
        }

        private void OnDisable()
        {
            ToolsComponentStack.OnDisable();
            CommonComponentStack.OnDisable();

#if UNITY_EDITOR
            Save();
#endif
        }

#if UNITY_EDITOR
        public void Save() => EditorUtility.SetDirty(this);
#endif

#if UNITY_EDITOR
        private ToolsComponentStackEditor _toolsComponentStackEditor;
        private IMGUIComponentStackEditor<Component, IMGUIElementEditor> _commonComponentStackEditor;

        public ToolsComponentStackEditor ToolsComponentStackEditor
        {
            get
            {
                if (_toolsComponentStackEditor == null)
                {
                    _toolsComponentStackEditor = new ToolsComponentStackEditor(ToolsComponentStack);
                }

                return _toolsComponentStackEditor;
            }
        }

        public IMGUIComponentStackEditor<Component, IMGUIElementEditor> CommonComponentStackEditor
        {
            get
            {
                if (_commonComponentStackEditor == null)
                {
                    _commonComponentStackEditor =
                        new IMGUIComponentStackEditor<Component, IMGUIElementEditor>(CommonComponentStack);
                }

                return _commonComponentStackEditor;
            }
        }
#endif
    }
}
