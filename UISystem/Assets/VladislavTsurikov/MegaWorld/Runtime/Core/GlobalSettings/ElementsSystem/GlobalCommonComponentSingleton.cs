using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem
{
    public static class GlobalCommonComponentSingleton<T> where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null || _instance.IsHappenedReset)
                {
                    _instance = (T)GlobalSettings.Instance.CommonComponentStack.GetElement(typeof(T));
                }

                return _instance;
            }
        }
    }
}
