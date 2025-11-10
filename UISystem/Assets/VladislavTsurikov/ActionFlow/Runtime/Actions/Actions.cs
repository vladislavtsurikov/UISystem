using OdinSerializer;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions
{
    public class Actions : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public ActionCollection ActionCollection = new();
    }
}
