using VladislavTsurikov.ScriptableObjectUtility.Runtime;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [LocationAsset("CustomInspector")]
    public class CustomInspectorPreferences : SerializedScriptableObjectSingleton<CustomInspectorPreferences>
    {
        public bool ShowFieldWithHideInInspectorAttribute;
    }
}
