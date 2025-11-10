#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.Core;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;
using VladislavTsurikov.UIElementsUtility.Runtime.Groups.Styles;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.Styles
{
    public class StylesGenerator : DataGroupAPIGenerator<StyleGroup, StyleInfo>
    {
        protected override void Generate(List<StyleGroup> groups)
        {
            var classModel = new ClassModel("GetStyle");
            classModel.SingleKeyWord = KeyWord.Static;
            var nestedClasses = new List<ClassModel>();
            foreach (StyleGroup group in groups)
            {
                var nestedClass = new ClassModel(group.GroupName);
                nestedClass.SingleKeyWord = KeyWord.Static;

                var groupName = "StyleGroup";

                var fields = new[]
                {
                    new Field(typeof(StyleGroup), "s_styleGroup")
                    {
                        AccessModifier = AccessModifier.Private, SingleKeyWord = KeyWord.Static
                    }
                }.ToList();

                var properties = new[]
                {
                    new Property(typeof(StyleGroup), groupName)
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody = "s_styleGroup != null? s_styleGroup: s_styleGroup = " +
                                     $"{nameof(DataGroupUtility)}.GetGroup<{nameof(StyleGroup)}, {nameof(StyleInfo)}>(\"{group.GroupName}\")"
                    }
                }.ToList();

                var methods = new[]
                {
                    new Method(typeof(StyleSheet), "GetStyleSheet")
                    {
                        SingleKeyWord = KeyWord.Static,
                        AccessModifier = AccessModifier.Private,
                        Parameters = new List<Parameter> { new("StyleName", "styleName") },
                        BodyLines = new List<string>
                        {
                            $"return {groupName}.GetStyleSheet(styleName.ToString());"
                        }
                    }
                }.ToList();

                var enumModel = new EnumModel("StyleName");

                foreach (StyleInfo style in group.Items)
                {
                    enumModel.EnumValues.Add(new EnumValue(style.UssReference.name));
                    var privateFieldItemName = $"s_{style.UssReference.name.ToLowerFirstChar()}";

                    var field = new Field(typeof(StyleSheet), privateFieldItemName)
                    {
                        AccessModifier = AccessModifier.Private, SingleKeyWord = KeyWord.Static
                    };

                    var property = new Property(typeof(StyleSheet), style.UssReference.name)
                    {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody =
                            $"{privateFieldItemName} ? {privateFieldItemName} : {privateFieldItemName} = GetStyleSheet(StyleName.{style.UssReference.name})"
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
                typeof(StyleGroup),
                typeof(StyleSheet),
                typeof(DataGroupUtility));
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
