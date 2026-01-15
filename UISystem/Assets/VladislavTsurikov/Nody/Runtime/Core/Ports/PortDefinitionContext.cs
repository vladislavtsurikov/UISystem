using System;
using System.Collections.Generic;
using System.Linq;

namespace VladislavTsurikov.Nody.Runtime.Core.Ports
{
    public class PortDefinitionContext
    {
        private readonly List<NodePort> _ports = new List<NodePort>();

        public PortBuilder AddInputPort<T>(string name)
        {
            return new PortBuilder(this, name, typeof(T), PortDirection.Input);
        }

        public PortBuilder AddOutputPort<T>(string name)
        {
            return new PortBuilder(this, name, typeof(T), PortDirection.Output);
        }

        internal void RegisterPort(NodePort port)
        {
            _ports.Add(port);
        }

        public IEnumerable<NodePort> GetInputPorts()
        {
            return _ports.Where(p => p.Direction == PortDirection.Input);
        }

        public IEnumerable<NodePort> GetOutputPorts()
        {
            return _ports.Where(p => p.Direction == PortDirection.Output);
        }

        public IEnumerable<NodePort> GetAllPorts()
        {
            return _ports;
        }

        public void Clear()
        {
            _ports.Clear();
        }
    }
}
