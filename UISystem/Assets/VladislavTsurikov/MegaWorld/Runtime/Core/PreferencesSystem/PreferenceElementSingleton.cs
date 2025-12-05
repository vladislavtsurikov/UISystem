namespace VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem
{
    public static class PreferenceElementSingleton<T> where T : PreferenceSettings
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null || _instance.IsHappenedReset)
                {
                    _instance = (T)PreferencesSettings.Instance.GetElement(typeof(T));
                }

                return _instance;
            }
        }
    }
}
