#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace VladislavTsurikov.AutoDefines.Editor.Core
{
    public static class UpmListCache
    {
        public enum State
        {
            Idle,
            Listing,
            Ready,
            Failed
        }

        private static HashSet<string> s_installed;
        private static ListRequest s_list;

        public static State CurrentState { get; private set; } = State.Idle;

        public static event Action<State> Refreshed;

        public static bool IsInstalled(string packageId) => s_installed != null && s_installed.Contains(packageId);

        public static void Refresh()
        {
            if (CurrentState == State.Listing)
            {
                return;
            }

            s_installed = null;
            CurrentState = State.Listing;
            s_list = Client.List(true);
            EditorApplication.update += Tick;
        }

        private static void Tick()
        {
            if (s_list == null)
            {
                return;
            }

            if (!s_list.IsCompleted)
            {
                return;
            }

            if (s_list.Error == null)
            {
                s_installed = s_list.Result.Select(p => p.name).ToHashSet();
                CurrentState = State.Ready;
            }
            else
            {
                s_installed = new HashSet<string>();
                CurrentState = State.Failed;
                Debug.LogWarning($"UPM List failed: {s_list.Error.message}");
            }

            s_list = null;
            EditorApplication.update -= Tick;

            Refreshed?.Invoke(CurrentState);
        }
    }
}
#endif
