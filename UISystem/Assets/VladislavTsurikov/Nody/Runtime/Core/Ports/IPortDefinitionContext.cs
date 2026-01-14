using System.Collections.Generic;

namespace VladislavTsurikov.Nody.Runtime.Core.Ports
{
    public interface IPortDefinitionContext
    {
        IPortBuilder AddInputPort<T>(string name);
        IPortBuilder AddOutputPort<T>(string name);

        IEnumerable<NodePort> GetInputPorts();
        IEnumerable<NodePort> GetOutputPorts();
        IEnumerable<NodePort> GetAllPorts();
    }
}
