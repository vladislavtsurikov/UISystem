#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.ComponentStack.Editor.Core
{
    public abstract class ElementEditor
    {
        public Element Target { get; private set; }

        public void Init(Element target)
        {
            Target = target;
            InitElement();
            OnEnable();
        }

        protected virtual void InitElement()
        {
        }

        public virtual void OnEnable()
        {
        }

        public virtual void OnDisable()
        {
        }
    }
}
#endif
