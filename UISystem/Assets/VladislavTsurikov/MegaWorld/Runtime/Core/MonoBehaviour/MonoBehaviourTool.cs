using System;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour
{
    public abstract class MonoBehaviourTool : SerializedMonoBehaviour
    {
        public SelectionData Data = new();

        [OdinSerialize]
        private ComponentStack _componentStack = new();

        [field: NonSerialized]
        public bool IsSetup { get; private set; }

        internal ComponentStack ComponentStack => _componentStack;

        private void Update()
        {
            Data.DeleteNullElements();
            Data.SelectedData.SetSelectedData();

            OnUpdate();
        }

        private void OnEnable() => Setup();

        public Runtime_Core_Component GetElement(Type elementType) => _componentStack.GetElement(elementType);

        public void Setup()
        {
            Data.Setup();
            _componentStack.Setup(true, new object[] { this });
            OnToolEnable();

            IsSetup = true;
        }

        protected virtual void OnUpdate()
        {
        }

        private protected virtual void OnToolEnable()
        {
        }
    }
}
