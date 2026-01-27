using Cysharp.Threading.Tasks;
using VladislavTsurikov.EntityDataAction.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Runtime
{
    public abstract class FilteredEntity : Entity
    {
        public virtual string[] GetAllowedDataNamePrefixes()
        {
            return null;
        }

        public virtual string[] GetAllowedActionNamePrefixes()
        {
            return null;
        }

        protected override void OnSetupEntity()
        {
            Data.Setup();
            Actions.Setup();

            if (Active)
            {
                Data.ElementAdded += HandleDataChanged;
                Data.ElementRemoved += HandleDataChanged;

                Actions.Run().Forget();
            }
        }
    }
}
