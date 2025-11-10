namespace VladislavTsurikov.ComponentStack.Runtime.Core
{
    public interface ISetupable
    {
        void Setup(object[] setupData = null, bool force = false);
    }
}
