using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;

namespace VladislavTsurikov.UnityUtility.Runtime
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

#if UNITY_6000_0_OR_NEWER
                T[] singletonInstances = FindObjectsByType<T>(FindObjectsSortMode.None);
#else
                T[] singletonInstances = FindObjectsOfType<T>();
#endif

                if (singletonInstances.Length == 0)
                {
                    var monoBehaviourNameAttribute =
                        (MonoBehaviourNameAttribute)typeof(T).GetAttribute(typeof(MonoBehaviourNameAttribute));
                    var dontDestroyOnLoadAttribute =
                        (DontDestroyOnLoadAttribute)typeof(T).GetAttribute(typeof(DontDestroyOnLoadAttribute));

                    var obj = new GameObject { name = monoBehaviourNameAttribute.Name };
                    if (dontDestroyOnLoadAttribute != null)
                    {
                        DontDestroyOnLoad(obj);
                    }

                    _instance = obj.AddComponent<T>();
                }
                else if (singletonInstances.Length > 1)
                {
                    for (var i = 0; i < singletonInstances.Length - 1; i++)
                    {
                        DestroyImmediate(singletonInstances[i]);
                    }
                }
                else
                {
                    _instance = singletonInstances[0];
                }

                return _instance;
            }
        }

        public static T GetInstance() => _instance;

        public static void DestroyGameObject()
        {
            if (_instance != null)
            {
                DestroyImmediate(_instance.gameObject);
            }
        }
    }
}
