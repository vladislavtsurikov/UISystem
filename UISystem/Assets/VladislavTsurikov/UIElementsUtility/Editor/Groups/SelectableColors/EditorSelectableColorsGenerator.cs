#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.Core;
using VladislavTsurikov.UIElementsUtility.Editor.Groups.EditorColors;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.SelectableColors
{
    public class
        EditorSelectableColorsGenerator : DataGroupAPIGenerator<EditorSelectableColorPalette, EditorSelectableColorInfo>
    {
        protected override void Generate(List<EditorSelectableColorPalette> groups)
        {
            var classModel = new ClassModel("GetSelectableColor");
            classModel.SingleKeyWord = KeyWord.Static;
            var nestedClasses = new List<ClassModel>();
            foreach (EditorSelectableColorPalette group in groups)
            {
                var nestedClass = new ClassModel(group.GroupName);
                nestedClass.SingleKeyWord = KeyWord.Static;

                var groupName = "SelectableColorPalette";

                var fields = new[]
                {
                    new Field(typeof(EditorSelectableColorPalette), "s_selectableColorPalette")
                    {
                        AccessModifier = AccessModifier.Private, SingleKeyWord = KeyWord.Static
                    }
                }.ToList();

                var properties = new[]
                {
                    new Property(typeof(EditorSelectableColorPalette), groupName)
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody =
                            "s_selectableColorPalette != null? s_selectableColorPalette: s_selectableColorPalette = " +
                            $"{nameof(DataGroupUtility)}.GetGroup<{nameof(EditorSelectableColorPalette)}, {nameof(EditorSelectableColorInfo)}>(\"{group.GroupName}\")"
                    }
                }.ToList();

                var methods = new[]
                {
                    new Method(typeof(Color), "GetColor")
                    {
                        SingleKeyWord = KeyWord.Static,
                        Parameters =
                            new List<Parameter> { new("ColorName", "colorName"), new("SelectionState", "state") },
                        BodyLines =
                            new List<string> { $"return {groupName}.GetColor(colorName.ToString(), state);" }
                    },
                    new Method(typeof(EditorThemeColor), "GetThemeColor")
                    {
                        SingleKeyWord = KeyWord.Static,
                        Parameters =
                            new List<Parameter> { new("ColorName", "colorName"), new("SelectionState", "state") },
                        BodyLines =
                            new List<string> { $"return {groupName}.GetThemeColor(colorName.ToString(), state);" }
                    },
                    new Method(typeof(EditorSelectableColorInfo), "GetSelectableColorInfo")
                    {
                        SingleKeyWord = KeyWord.Static,
                        AccessModifier = AccessModifier.Private,
                        Parameters = new List<Parameter> { new("ColorName", "colorName") },
                        BodyLines = new List<string>
                        {
                            $"return {groupName}.GetSelectableColorInfo(colorName.ToString());"
                        }
                    }
                }.ToList();

                var enumModel = new EnumModel("ColorName");

                foreach (EditorSelectableColorInfo item in group.Items)
                {
                    var privateFieldItemName = $"s_{item.ColorName.ToLowerFirstChar()}";

                    enumModel.EnumValues.Add(new EnumValue(item.ColorName));

                    var field = new Field(typeof(EditorSelectableColorInfo), privateFieldItemName)
                    {
                        AccessModifier = AccessModifier.Private, SingleKeyWord = KeyWord.Static
                    };

                    var property = new Property(typeof(EditorSelectableColorInfo), item.ColorName)
                    {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody =
                            $"{privateFieldItemName} ?? ({privateFieldItemName} = GetSelectableColorInfo(ColorName.{item.ColorName}))"
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
            fileModel.PreprocessorDirectives.Add("UNITY_EDITOR");
            fileModel.LoadUsingDirectives(
                typeof(Color),
                typeof(EditorSelectableColorPalette),
                typeof(DataGroupUtility),
                typeof(EditorThemeColor),
                typeof(SelectionState),
                typeof(EditorSelectableColorInfo));
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
