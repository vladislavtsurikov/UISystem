#if UNITY_EDITOR
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.Nody.Editor.Core
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

        protected void MarkTargetDirty()
        {
            Target.MarkDirty();
        }
    }
}
#endif
