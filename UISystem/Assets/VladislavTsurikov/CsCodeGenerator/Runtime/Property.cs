using System;

namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public class Property : Field
    {
        private bool _isAutoImplemented;

        public Property()
        {
        }

        public Property(BuiltInDataType builtInDataType, string name) : base(builtInDataType, name)
        {
        }

        public Property(Type customDataType, string name) : base(customDataType, name)
        {
        }

        public override bool HasAttributes => false;

        public override AccessModifier AccessModifier { get; set; } = AccessModifier.Public;

        public bool IsGetOnly { get; set; }

        public bool IsAutoImplemented => string.IsNullOrEmpty(GetterBody) && string.IsNullOrEmpty(SetterBody);

        public string GetterBody { get; set; }

        public string SetterBody { get; set; }

        protected override string Ending => DefaultValue != null ? ";" : "";

        public override string Body
        {
            get
            {
                if (IsAutoImplemented)
                {
                    return IsGetOnly ? " { get; }" : " { get; set; }";
                }

                if (IsGetOnly)
                {
                    return " => " + GetterBody + ";";
                }

                var result = Constants.NewLine + Indent + "{";
                var currentIndent = Constants.NewLine + Indent + CsGenerator.IndentSingle;

                result += currentIndent + "get { return " + GetterBody + "; }";
                result += currentIndent + "set { " + SetterBody + "; }";
                result += Constants.NewLine + Indent + "}";

                return result;
            }
        }
    }
}
