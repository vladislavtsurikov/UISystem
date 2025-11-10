using UnityEngine;

namespace VladislavTsurikov.ScriptableObjectUtility.Runtime
{
    public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T s_instance;

        protected ScriptableSingleton()
        {
            if (s_instance != null)
            {
                Debug.LogError("ScriptableSingleton already exists. Did you query the singleton in a constructor?");
            }
            else
            {
                s_instance = this as T;
                System.Diagnostics.Debug.Assert(s_instance != null);
            }
        }

        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    CreateAndLoad();
                }

                return s_instance;
            }
        }

        private static void CreateAndLoad()
        {
            System.Diagnostics.Debug.Assert(s_instance == null);

            if (s_instance == null)
            {
                T t = CreateInstance<T>();
                t.hideFlags = HideFlags.HideAndDontSave;
            }

            System.Diagnostics.Debug.Assert(s_instance != null);
        }
    }
}
