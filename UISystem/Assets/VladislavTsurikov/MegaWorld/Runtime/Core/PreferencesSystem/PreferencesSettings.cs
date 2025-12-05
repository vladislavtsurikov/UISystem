using System;
using OdinSerializer;
using UnityEditor;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem
{
    [LocationAsset("MegaWorld/PreferencesSettings")]
    public class PreferencesSettings : SerializedScriptableObjectSingleton<PreferencesSettings>
    {
        [OdinSerialize]
        private ComponentStackOnlyDifferentTypes<PreferenceSettings> _stack = new();

#if UNITY_EDITOR
        public IMGUIComponentStackEditor<PreferenceSettings, IMGUIElementEditor> StackEditor;
#endif

        private void OnEnable()
        {
            _stack.CreateAllElementTypes();

#if UNITY_EDITOR
            StackEditor = new IMGUIComponentStackEditor<PreferenceSettings, IMGUIElementEditor>(_stack);
#endif
        }

        public Component GetElement(Type elementType) => _stack.GetElement(elementType);

#if UNITY_EDITOR
        public void Save() => EditorUtility.SetDirty(this);
#endif
    }
}
