#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Warning
{
    [Serializable]
    public class AddressableLoaderValidator
    {
        public static List<ValidationResult> ValidateAll()
        {
            Debug.Log("[AddressableLoaderValidator] Validation skipped: AutoLoadAttribute is no longer required.");

            return new List<ValidationResult>();
        }

        public static ValidationResult ValidateLoader(Type loaderType)
        {
            return new ValidationResult { LoaderType = loaderType };
        }

        [Serializable]
        public class ValidationResult
        {
            public List<string> MissingAutoLoadFields = new();
            public Type LoaderType;
        }
    }
}
#endif
