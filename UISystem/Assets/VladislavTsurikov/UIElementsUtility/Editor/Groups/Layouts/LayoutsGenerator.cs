#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.Core;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;
using VladislavTsurikov.UIElementsUtility.Runtime.Groups.Layouts;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.Layouts
{
    public class LayoutsGenerator : DataGroupAPIGenerator<LayoutGroup, LayoutInfo>
    {
        protected override void Generate(List<LayoutGroup> groups)
        {
            var classModel = new ClassModel("GetLayout");
            classModel.SingleKeyWord = KeyWord.Static;
            var nestedClasses = new List<ClassModel>();
            foreach (LayoutGroup group in groups)
            {
                var nestedClass = new ClassModel(group.GroupName);
                nestedClass.SingleKeyWord = KeyWord.Static;

                var groupName = "LayoutGroup";

                var fields = new[]
                {
                    new Field(typeof(LayoutGroup), "s_layoutGroup")
                    {
                        AccessModifier = AccessModifier.Private, SingleKeyWord = KeyWord.Static
                    }
                }.ToList();

                var properties = new[]
                {
                    new Property(typeof(LayoutGroup), groupName)
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody = "s_layoutGroup != null ? s_layoutGroup: s_layoutGroup = " +
                                     $"{nameof(DataGroupUtility)}.GetGroup<{nameof(LayoutGroup)}, {nameof(LayoutInfo)}>(\"{group.GroupName}\")"
                    }
                }.ToList();

                var methods = new[]
                {
                    new Method(typeof(VisualTreeAsset), "GetVisualTreeAsset")
                    {
                        SingleKeyWord = KeyWord.Static,
                        AccessModifier = AccessModifier.Private,
                        Parameters = new List<Parameter> { new("LayoutName", "layoutName") },
                        BodyLines = new List<string>
                        {
                            $"return {groupName}.GetVisualTreeAsset(layoutName.ToString());"
                        }
                    }
                }.ToList();

                var enumModel = new EnumModel("LayoutName");

                foreach (LayoutInfo item in group.Items)
                {
                    enumModel.EnumValues.Add(new EnumValue(item.UxmlReference.name));

                    var privateFieldItemName = $"s_{item.UxmlReference.name.ToLowerFirstChar()}";

                    var field = new Field(typeof(VisualTreeAsset), privateFieldItemName)
                    {
                        AccessModifier = AccessModifier.Private, SingleKeyWord = KeyWord.Static
                    };

                    var property = new Property(typeof(VisualTreeAsset), item.UxmlReference.name)
                    {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody = $"{privateFieldItemName} ? " +
                                     $"{privateFieldItemName} : " +
                                     $"{privateFieldItemName} = GetVisualTreeAsset(LayoutName.{item.UxmlReference.name})"
                    };

                    fields.Add(field);
                    properties.Add(property);
                }

                nestedClass.Fields = fields;
                nestedClass.Properties = properties;
                nestedClass.Methods = methods;
                nestedClass.Enums.Add(enumModel);

                nestedClasses.Add(nestedClass);
            }

            classModel.NestedClasses.AddRange(nestedClasses);

            var fileModel = new FileModel(classModel.Name);
            fileModel.LoadUsingDirectives(
                typeof(DataGroupUtility),
                typeof(VisualTreeAsset),
                typeof(LayoutGroup));
            fileModel.SetNamespaceFromFolder(TargetFilePath, "Assets", "Runtime", "API");
            fileModel.Classes.Add(classModel);

            var csGenerator = new CsGenerator();
            csGenerator.Files.Add(fileModel);
            csGenerator.Path = TargetFilePath;
            csGenerator.CreateFiles();
        }
    }
}
#endif
