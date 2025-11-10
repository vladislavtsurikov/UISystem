#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;

namespace VladislavTsurikov.AutoDefines.Editor.Core
{
    public static class UpmInstaller
    {
        private static readonly Queue<string> s_queue = new();
        private static AddRequest s_addRequest;
        private static string s_currentPackage;

        static UpmInstaller() => UpmInstallRequest.OnCompleted += MoveNextQueue;

        public static void Install(string packageId)
        {
            if (AddToQueueIfPossible(packageId))
            {
                return;
            }

            if (!UpmInstallRequest.IsInstalling())
            {
                MoveNextQueue();
            }
        }

        private static void MoveNextQueue()
        {
            if (s_queue.Count == 0)
            {
                return;
            }

            s_currentPackage = s_queue.Dequeue();
            UpmInstallRequest.Install(s_currentPackage);
        }

        private static bool AddToQueueIfPossible(string packageId)
        {
            if (s_queue.Contains(packageId))
            {
                return true;
            }

            s_queue.Enqueue(packageId);
            return false;
        }
    }
}
#endif
