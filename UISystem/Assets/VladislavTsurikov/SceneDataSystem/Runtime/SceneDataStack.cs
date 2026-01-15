using System;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;

namespace VladislavTsurikov.SceneDataSystem.Runtime
{
    public class SceneDataStack : NodeStackOnlyDifferentTypes<SceneData>
    {
        protected override bool AllowCreate(Type type)
        {
            AllowCreateComponentAttribute allowCreateComponentAttribute =
                type.GetAttribute<AllowCreateComponentAttribute>();

            if (allowCreateComponentAttribute == null ||
                allowCreateComponentAttribute.Allow((SceneDataManager)SetupData[0]))
            {
                return true;
            }

            return false;
        }
    }
}
