
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.ActionFlow.Runtime.Events
{
    public abstract class Event : Node
    {
        internal Trigger Trigger;

        protected override void SetupComponent(object[] setupData = null)
        {
            Trigger = (Trigger)setupData[0];
        }
    }
}
