using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.ComponentStack.Runtime.Core
{
    public class AdvancedElementList<T> : CallbackList<T>
        where T : IRemovable
    {
        public AdvancedElementList() => OnRemoved += OnRemove;

        private void OnRemove(int index) => this[index].OnRemove();
    }
}
