using System;
using System.Collections.Generic;

namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public class Method : BaseElement
    {
        public Method()
        {
        }

        public Method(BuiltInDataType builtInDataType, string name) : base(builtInDataType, name)
        {
        }

        public Method(Type customDataType, string name) : base(customDataType, name)
        {
        }

        public Method(AccessModifier accessModifier, KeyWord singleKeyWord, BuiltInDataType builtInDataType,
            string name) : base(builtInDataType, name)
        {
            AccessModifier = accessModifier;
            KeyWords.Add(singleKeyWord);
        }

        public virtual bool IsVisible { get; set; } = true;

        public List<Parameter> Parameters { get; set; } = new();

        public string BaseParameters { get; set; }
        public string BaseParametersFormated => BaseParameters != null ? $" : base({BaseParameters})" : "";

        public virtual bool BracesInNewLine { get; set; } = true;

        public List<string> BodyLines { get; set; } = new();

        public override string Signature => base.Signature + Parameters.ToStringList();

        public override string ToString()
        {
            if (!IsVisible)
            {
                return "";
            }

            var result = base.ToString() + BaseParametersFormated;
            var bracesPrefix = BracesInNewLine ? Constants.NewLine + Indent : " ";
            var curentIndent = Constants.NewLine + Indent + CsGenerator.IndentSingle;
            result += bracesPrefix + "{";
            result += BodyLines.Count == 0
                ? ""
                : (BracesInNewLine ? curentIndent : " ") + string.Join(curentIndent, BodyLines);
            result += bracesPrefix + "}";
            return result;
        }
    }
}
