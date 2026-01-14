namespace VladislavTsurikov.Nody.Runtime.Core.Ports
{
    public interface IPortBuilder
    {
        IPortBuilder WithDefault(object value);
        IPortBuilder WithId(string id);
        NodePort Build();
    }
}
