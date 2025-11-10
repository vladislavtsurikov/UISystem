using System.Collections.Generic;

namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public class AttributeModel
    {
        public AttributeModel(string name = null) => Name = name;

        public string Name { get; set; }

        public List<Parameter> Parameters { get; set; } = new();

        public Parameter SingleParameter
        {
            set => Parameters.Add(value);
        }

        public override string ToString()
        {
            var parametersString = Parameters.Count > 0 ? Parameters.ToStringList() : "";
            var result = $"[{Name}{parametersString}]";
            return result;
        }
    }

    public static class AttributeModelExtensions
    {
        public static string ToStringList(this List<AttributeModel> attributes, string indent)
        {
            var result = attributes.Count > 0
                ? Constants.NewLine + indent + string.Join(Constants.NewLine + indent, attributes)
                : "";
            return result;
        }
    }
}
