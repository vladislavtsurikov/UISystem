#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.Core;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;
using VladislavTsurikov.UIElementsUtility.Runtime.Groups.Fonts;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.Fonts
{
    public class FontsGenerator : DataGroupAPIGenerator<FontFamily, FontInfo>
    {
        protected override void Generate(List<FontFamily> groups)
        {
            var classModel = new ClassModel("GetFont");
            classModel.SingleKeyWord = KeyWord.Static;
            var nestedClasses = new List<ClassModel>();
            foreach (FontFamily group in groups)
            {
                var nestedClass = new ClassModel(group.GroupName);
                nestedClass.SingleKeyWord = KeyWord.Static;

                var groupName = "FontFamily";

                var fields = new[]
                {
                    new Field(typeof(FontFamily), "s_fontFamily")
                    {
                        AccessModifier = AccessModifier.Private, SingleKeyWord = KeyWord.Static
                    }
                }.ToList();

                var properties = new[]
                {
                    new Property(typeof(FontFamily), groupName)
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody =
                            $"s_fontFamily != null? s_fontFamily: s_fontFamily = {nameof(DataGroupUtility)}.GetGroup<{nameof(FontFamily)}, {nameof(FontInfo)}>(\"{group.GroupName}\")"
                    }
                }.ToList();

                var methods = new[]
                {
                    new Method(typeof(Font), "GetFont")
                    {
                        SingleKeyWord = KeyWord.Static,
                        AccessModifier = AccessModifier.Private,
                        Parameters = new List<Parameter> { new("FontWeight", "weight") },
                        BodyLines = new List<string> { $"return {groupName}.GetFont((int)weight);" }
                    }
                }.ToList();

                var enumModel = new EnumModel("FontWeight");

                foreach (FontInfo item in group.Items)
                {
                    var privateFieldItemName = $"s_{item.Weight.ToString().ToLowerFirstChar()}";
                    enumModel.EnumValues.Add(new EnumValue(item.Weight.ToString(), (int)item.Weight));

                    var field = new Field(typeof(Font), privateFieldItemName)
                    {
                        AccessModifier = AccessModifier.Private, SingleKeyWord = KeyWord.Static
                    };

                    var property = new Property(typeof(Font), item.Weight.ToString())
                    {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody =
                            $"{privateFieldItemName} ? {privateFieldItemName} : {privateFieldItemName} = GetFont(FontWeight.{item.Weight.ToString()})"
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
                typeof(Font),
                typeof(FontFamily),
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
