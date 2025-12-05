using System.Collections.Generic;
using System.Linq;

namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public class ClassModel : BaseElement
    {
        public ClassModel(string name = null)
        {
            base.CustomDataType = Constants.Class;
            base.Name = name;
            Constructors.Add(new Constructor(Name) { IsVisible = false, BracesInNewLine = false });
        }

        public bool HasPropertiesSpacing { get; set; } = true;

        public new BuiltInDataType? BuiltInDataType { get; }

        public new string CustomDataType => Constants.Class;

        public new string Name => base.Name;

        public string BaseClass { get; set; }

        public List<string> Interfaces { get; set; } = new();

        public virtual List<Constructor> Constructors { get; set; } = new();

        public virtual Constructor DefaultConstructor
        {
            get => Constructors[0];
            set => Constructors[0] = value;
        }

        public List<EnumModel> Enums { get; set; } = new();
        public virtual List<Field> Fields { get; set; } = new();
        public virtual List<Property> Properties { get; set; } = new();

        public virtual List<Method> Methods { get; set; } = new();

        public virtual List<ClassModel> NestedClasses { get; set; } = new();
        // Nested indent have to be set for each Nested element and subelement separately, or after generation manualy to select nested code and indent it with tab
        // Setting it automaticaly and propagating could be done if the parent sets the child's parent reference (to itself) when the child is added/assigned to a parent. Parent setter is internal.
        //   http://softwareengineering.stackexchange.com/questions/261453/what-is-the-best-way-to-initialize-a-childs-reference-to-its-parent

        public override string ToString()
        {
            var result = base.ToString();
            result += BaseClass != null || Interfaces?.Count > 0 ? " : " : "";
            result += BaseClass != null ? BaseClass : "";
            result += BaseClass != null && Interfaces?.Count > 0 ? ", " : "";
            result += Interfaces?.Count > 0 ? string.Join(", ", Interfaces) : "";
            result += Constants.NewLine + Indent + "{";

            result += string.Join(Constants.NewLine, Enums);
            result += string.Join("", Fields);

            IEnumerable<Constructor> visibleConstructors = Constructors.Where(a => a.IsVisible);
            var hasFieldsBeforeConstructor = visibleConstructors.Any() && Fields.Any();
            result += hasFieldsBeforeConstructor ? Constants.NewLine : "";
            result += string.Join(Constants.NewLine, visibleConstructors);
            var hasMembersAfterConstructor =
                (visibleConstructors.Any() || Fields.Any()) && (Properties.Any() || Methods.Any());
            result += hasMembersAfterConstructor ? Constants.NewLine : "";

            result += string.Join(HasPropertiesSpacing ? Constants.NewLine : "", Properties);

            result += hasMembersAfterConstructor ? Constants.NewLine : "";
            result += string.Join(Constants.NewLine, Methods);

            for (var i = 0; i < NestedClasses.Count; i++)
            {
                NestedClasses[i].IncreaseIndentLevel();

                result += string.Join(Constants.NewLine, NestedClasses[i]);

                if (i != NestedClasses.Count - 1)
                {
                    result += Constants.NewLine;
                }
            }

            result += Constants.NewLine + Indent + "}";
            return result;
        }

        private void IncreaseIndentLevel()
        {
            IndentLevel++;

            foreach (Field element in Fields)
            {
                element.IndentLevel = IndentLevel + 1;
            }

            foreach (Property element in Properties)
            {
                element.IndentLevel = IndentLevel + 1;
            }

            foreach (Method element in Methods)
            {
                element.IndentLevel = IndentLevel + 1;
            }

            foreach (EnumModel element in Enums)
            {
                element.IndentLevel = IndentLevel + 1;

                foreach (EnumValue enumValue in element.EnumValues)
                {
                    enumValue.IndentLevel = element.IndentLevel + 1;
                }
            }

            foreach (Constructor element in Constructors)
            {
                element.IndentLevel = IndentLevel + 1;
            }
        }
    }
}
