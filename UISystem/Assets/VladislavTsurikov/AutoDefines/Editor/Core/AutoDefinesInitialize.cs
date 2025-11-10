#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEditor.PackageManager;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AutoDefines.Editor.Core
{
    public static class AutoDefinesInitialize
    {
        private static bool s_running;
        private static bool s_initialized;
        private static readonly List<AutoDefineRule> s_rules = new();

        [DidReloadScripts]
        private static void OnReloadScripts()
        {
            if (!s_initialized)
            {
                InitializeRules();
                RegisterEventsOnce();
                s_initialized = true;
            }

            UpmListCache.Refresh();
        }

        private static void InitializeRules()
        {
            s_rules.Clear();
            foreach (AutoDefineRule rule in ReflectionFactory.CreateAllInstances<AutoDefineRule>())
            {
                s_rules.Add(rule);
            }
        }

        private static void RegisterEventsOnce()
        {
            Events.registeredPackages += _ => UpmListCache.Refresh();
            UpmListCache.Refreshed += OnUpmListRefreshed;
        }

        private static void OnUpmListRefreshed(UpmListCache.State state)
        {
            if (state == UpmListCache.State.Ready)
            {
                RunAllRules();
            }
        }

        private static void RunAllRules()
        {
            if (s_running)
            {
                return;
            }

            s_running = true;

            foreach (AutoDefineRule rule in s_rules)
            {
                try
                {
                    rule.Run();
                }
                catch (SystemException e)
                {
                    Debug.LogException(e);
                }
            }

            s_running = false;
        }
    }
}
#endif
