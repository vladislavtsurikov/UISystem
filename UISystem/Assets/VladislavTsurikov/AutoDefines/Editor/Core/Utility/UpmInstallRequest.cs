using System;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace VladislavTsurikov.AutoDefines.Editor.Core
{
    internal static class UpmInstallRequest
    {
        private static AddRequest s_addRequest;
        private static string s_packageId;

        public static event Action OnCompleted;

        public static void Install(string packageId)
        {
            s_packageId = packageId;
            s_addRequest = Client.Add(s_packageId);
            EditorApplication.update += Tick;
        }

        private static void Tick()
        {
            if (IsInstalling())
            {
                return;
            }

            if (s_addRequest.Error != null)
            {
                Debug.LogError($"UPM Add failed: {s_packageId} — {s_addRequest.Error.message}");
            }
            else
            {
                Debug.Log($"UPM Add success: {s_packageId}");
                UpmListCache.Refresh();
            }

            EditorApplication.update -= Tick;
            OnCompleted?.Invoke();
        }

        public static bool IsInstalling() => s_addRequest != null && s_addRequest.IsCompleted;
    }
}
