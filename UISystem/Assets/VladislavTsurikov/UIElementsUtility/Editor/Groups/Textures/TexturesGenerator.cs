#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.Core;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;
using VladislavTsurikov.UIElementsUtility.Runtime.Groups.Textures;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.Textures
{
    internal class TexturesGenerator : DataGroupAPIGenerator<TextureGroup, TextureInfo>
    {
        protected override void Generate(List<TextureGroup> groups)
        {
            var classModel = new ClassModel("GetTexture");
            classModel.SingleKeyWord = KeyWord.Static;
            var nestedClasses = new List<ClassModel>();
            foreach (TextureGroup group in groups)
            {
                var nestedClass = new ClassModel(group.GroupName);
                nestedClass.SingleKeyWord = KeyWord.Static;

                var groupName = "TextureGroup";

                var fields = new[]
                {
                    new Field(typeof(TextureGroup), "s_textureGroup")
                    {
                        AccessModifier = AccessModifier.Private, SingleKeyWord = KeyWord.Static
                    }
                }.ToList();

                var properties = new[]
                {
                    new Property(typeof(TextureGroup), groupName)
                    {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody = "s_textureGroup != null? s_textureGroup: s_textureGroup = " +
                                     $"{nameof(DataGroupUtility)}.GetGroup<{nameof(TextureGroup)}, {nameof(TextureInfo)}>(\"{group.GroupName}\")"
                    }
                }.ToList();

                var methods = new[]
                {
                    new Method(typeof(Texture2D), "GetTexture2D")
                    {
                        SingleKeyWord = KeyWord.Static,
                        AccessModifier = AccessModifier.Private,
                        Parameters = new List<Parameter> { new("TextureName", "textureName") },
                        BodyLines = new List<string>
                        {
                            $"return {groupName}.GetTexture(textureName.ToString());"
                        }
                    }
                }.ToList();

                var enumModel = new EnumModel("TextureName");

                foreach (TextureInfo item in group.Items)
                {
                    var privateFieldItemName = $"s_{item.TextureName.ToLowerFirstChar()}";
                    enumModel.EnumValues.Add(new EnumValue(item.TextureName));

                    var field = new Field(typeof(Texture2D), privateFieldItemName)
                    {
                        AccessModifier = AccessModifier.Private, SingleKeyWord = KeyWord.Static
                    };

                    var property = new Property(typeof(Texture2D), item.TextureName)
                    {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody =
                            $"{privateFieldItemName} ? {privateFieldItemName} : {privateFieldItemName} = GetTexture2D(TextureName.{item.TextureName})"
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
                typeof(Texture2D),
                typeof(TextureGroup),
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
