#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create.Attributes;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    [ResourceLoaderTemplateBaseType(typeof(ResourceLoader))]
    public class SimpleResourceLoaderTemplate : ResourceLoaderFieldTemplate
    {
        public override void Run()
        {
            if (string.IsNullOrEmpty(ClassName) || Fields == null || Fields.Count == 0)
            {
                Debug.LogError("[SimpleResourceLoaderGenerator] Invalid generator data");
                return;
            }

            var classModel = new ClassModel(ClassName)
            {
                AccessModifier = AccessModifier.Public,
                BaseClass = GetBaseTypeName()
            };

            var fields = new List<Field>();
            var properties = new List<Property>();
            var loadLines = new List<string>();
            var taskNames = new List<string>();

            for (int i = 0; i < Fields.Count; i++)
            {
                var field = Fields[i];
                if (field == null || field.Asset == null)
                    continue;

                string fieldName = $"_{field.Asset.name.ToLowerInvariant()}";
                string propName = field.Asset.name;
                string taskName = $"t{i + 1}";
                taskNames.Add(taskName);

                Type fieldType = field.Asset.GetType();

                fields.Add(new Field(fieldType, fieldName)
                {
                    AccessModifier = AccessModifier.Private
                });

                properties.Add(new Property(fieldType, propName)
                {
                    AccessModifier = AccessModifier.Public,
                    IsGetOnly = true,
                    GetterBody = $"return {fieldName};"
                });

                loadLines.Add($"var {taskName} = LoadAndTrack<{fieldType.Name}>(\"{fieldType.Name}\", token).ContinueWith(result => {fieldName} = result);");
            }

            string whenAllLine = taskNames.Count > 0
                ? $"return UniTask.WhenAll({string.Join(", ", taskNames)});"
                : "return UniTask.CompletedTask;";

            classModel.Fields = fields;
            classModel.Properties = properties;

            var loadMethod = new Method(typeof(UniTask), "LoadResourceLoader")
            {
                AccessModifier = AccessModifier.Public,
                SingleKeyWord = KeyWord.Override,
                Parameters = new List<Parameter>
                {
                    new(typeof(CancellationToken), "token")
                },
                BodyLines = loadLines.Concat(new[] { whenAllLine }).ToList()
            };

            classModel.Methods.Add(loadMethod);

            var fileModel = new FileModel(ClassName);
            fileModel.LoadUsingDirectives(
                typeof(UniTask),
                typeof(CancellationToken),
                typeof(ResourceLoader),
                typeof(UnityEngine.Object)
            );
            fileModel.SetNamespaceFromFolder("Assets/Scripts/Generated", "Assets", "Scripts");
            fileModel.Classes.Add(classModel);

            var generator = new CsGenerator();
            generator.Files.Add(fileModel);
            generator.Path = "Assets/Scripts/Generated";
            generator.CreateFiles();

            Debug.Log($"[SimpleResourceLoaderGenerator] Generated {ClassName}.cs");
        }
    }
}
#endif
