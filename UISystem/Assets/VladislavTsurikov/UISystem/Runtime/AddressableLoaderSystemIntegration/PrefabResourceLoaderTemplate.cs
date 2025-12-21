#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration.Attributes;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    [ResourceLoaderTemplateBaseType(typeof(PrefabResourceLoader))]
    public class PrefabResourceLoaderTemplate : ResourceLoaderTemplate
    {
        public FieldData FieldData { get; private set; } = new FieldData();

        public string PrefabAddress { get; private set; }

        public void UpdatePrefabAddressFromFieldData()
        {
            PrefabAddress = FieldData?.Address ?? string.Empty;
        }

        public override void Run()
        {
            UpdatePrefabAddressFromFieldData();

            if (string.IsNullOrWhiteSpace(ClassName) || string.IsNullOrWhiteSpace(PrefabAddress))
            {
                Debug.LogError("[PrefabResourceLoaderGenerator] Invalid generator data");
                return;
            }

            var classModel = new ClassModel(ClassName)
            {
                AccessModifier = AccessModifier.Public,
                BaseClass = GetBaseTypeName()
            };

            var prefabAttribute = new AttributeModel(nameof(PrefabAddressAttribute))
            {
                SingleParameter = new Parameter($"\"{PrefabAddress}\"")
            };
            classModel.AddAttribute(prefabAttribute);

            if (Behaviors != null)
            {
                foreach (BehaviorAttributeData behavior in Behaviors)
                {
                    if (behavior == null || behavior.BehaviorType == null)
                    {
                        continue;
                    }

                    var behaviorAttribute = new AttributeModel(nameof(BehaviorAttribute));

                    var parameters = new List<Parameter>
                    {
                        new Parameter($"typeof({behavior.BehaviorType.Name})")
                    };

                    if (behavior.Contexts != null)
                    {
                        foreach (string context in behavior.Contexts)
                        {
                            if (string.IsNullOrWhiteSpace(context))
                            {
                                continue;
                            }

                            parameters.Add(new Parameter($"\"{context}\""));
                        }
                    }

                    behaviorAttribute.Parameters = parameters;
                    classModel.AddAttribute(behaviorAttribute);
                }
            }

            var fileModel = new FileModel(ClassName);
            fileModel.PreprocessorDirectives.Add("UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM");
            fileModel.LoadUsingDirectives(
                typeof(PrefabResourceLoader),
                typeof(PrefabAddressAttribute),
                typeof(BehaviorAttribute),
                typeof(Debug)
            );

            if (Behaviors != null)
            {
                foreach (BehaviorAttributeData behavior in Behaviors)
                {
                    if (behavior?.BehaviorType == null)
                    {
                        continue;
                    }

                    fileModel.LoadUsingDirectives(behavior.BehaviorType);
                }
            }

            string targetPath = !string.IsNullOrEmpty(CsFilePath)
                ? Path.GetDirectoryName(CsFilePath)
                : "Assets/Scripts/Generated";

            if (string.IsNullOrEmpty(targetPath))
            {
                Debug.LogError("[PrefabResourceLoaderGenerator] Target path is invalid.");
                return;
            }

            targetPath = targetPath.Replace("\\", "/");

            fileModel.SetNamespaceFromFolder(targetPath, "Assets", "Scripts");

            if (string.IsNullOrEmpty(fileModel.Namespace))
            {
                fileModel.Namespace = "VladislavTsurikov.Generated";
            }

            fileModel.Classes.Add(classModel);

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            var generator = new CsGenerator
            {
                Path = targetPath
            };
            generator.Files.Add(fileModel);
            generator.CreateFiles();

            Debug.Log($"[PrefabResourceLoaderGenerator] Generated {ClassName}.cs");
        }

        protected override void OnBuildFrom(Type loaderType)
        {
            PrefabAddressAttribute prefabAddressAttribute = loaderType.GetAttribute<PrefabAddressAttribute>();
            PrefabAddress = prefabAddressAttribute?.Address ?? string.Empty;

            FieldData = new FieldData("Prefab", typeof(Object), PrefabAddress);
            UpdatePrefabAddressFromFieldData();
        }

        public override void Validate(List<string> issues)
        {
            if (issues == null)
            {
                return;
            }

            UpdatePrefabAddressFromFieldData();

            if (FieldData?.Asset == null)
            {
                issues.Add("Prefab asset is empty.");
            }
        }
    }
}
#endif
