using System;

namespace VladislavTsurikov.Nody.Runtime.Core.Ports
{
    public class NodePort
    {
        public string Name { get; set; }
        public Type DataType { get; set; }
        public PortDirection Direction { get; set; }
        public object DefaultValue { get; set; }
        public string UniqueId { get; set; } = Guid.NewGuid().ToString();

        public NodePort() { }

        public NodePort(string name, Type dataType, PortDirection direction)
        {
            Name = name;
            DataType = dataType;
            Direction = direction;
        }

        public NodePort WithDefault(object value)
        {
            DefaultValue = value;
            return this;
        }

        public NodePort WithId(string id)
        {
            UniqueId = id;
            return this;
        }
    }
}
