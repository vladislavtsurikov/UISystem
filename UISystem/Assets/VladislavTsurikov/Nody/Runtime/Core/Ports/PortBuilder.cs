using System;

namespace VladislavTsurikov.Nody.Runtime.Core.Ports
{
    public class PortBuilder : IPortBuilder
    {
        private readonly PortDefinitionContext _context;
        private readonly NodePort _port;

        public PortBuilder(PortDefinitionContext context, string name, Type dataType, PortDirection direction)
        {
            _context = context;
            _port = new NodePort
            {
                Name = name,
                DataType = dataType,
                Direction = direction
            };
        }

        public IPortBuilder WithDefault(object value)
        {
            _port.DefaultValue = value;
            return this;
        }

        public IPortBuilder WithId(string id)
        {
            _port.UniqueId = id;
            return this;
        }

        public NodePort Build()
        {
            _context.RegisterPort(_port);
            return _port;
        }
    }
}
