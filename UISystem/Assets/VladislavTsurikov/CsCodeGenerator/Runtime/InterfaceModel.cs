using System.Collections.Generic;

namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public class InterfaceModel : BaseElement
    {
        public InterfaceModel(string name = null)
        {
            base.CustomDataType = Constants.Interface;
            base.Name = name;
        }

        public new BuiltInDataType? BuiltInDataType { get; }

        public new string CustomDataType => base.CustomDataType;

        public new string Name => base.Name;

        public virtual List<Property> Properties { get; set; } = new();

        public virtual List<Method> Methods { get; set; } = new();

        public override string ToString()
        {
            var result = base.ToString();
            result += Constants.NewLine + Indent + "{";

            result += string.Join("", Properties);
            var hasPropertiesAndMethods = Properties.Count > 0 && Methods.Count > 0;
            result += hasPropertiesAndMethods ? Constants.NewLine : "";
            result += string.Join(Constants.NewLine, Methods);

            result += Constants.NewLine + Indent + "}";
            result = result.Replace(AccessModifier.Public.ToTextLower() + " ", "");
            result = result.Replace("\r\n        {\r\n        }", ";");
            return result;
        }
    }
}
