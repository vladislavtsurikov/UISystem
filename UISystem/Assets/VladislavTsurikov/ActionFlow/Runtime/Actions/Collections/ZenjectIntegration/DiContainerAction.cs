#if ACTIONFLOW_ZENJECT
using Zenject;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.ZenjectIntegration
{
    public abstract class DiContainerAction : Action
    {
        protected abstract string ClassName { get; }

        protected DiContainer DiContainer { get; private set; }
        protected bool IsErrorInDiContainer { get; private set; }
        protected string ErrorMessage { get; private set; }

        protected override void SetupComponent(object[] setupData = null)
        {
            if (setupData == null)
            {
                return;
            }

            DiContainer = (DiContainer)setupData[0];
            SetupDiContainerAction();
        }

        protected virtual void SetupDiContainerAction()
        {
        }

        protected T TryResolve<T>() where T : class
        {
            var component = DiContainer.TryResolve(typeof(T));
            if (component == null)
            {
                IsErrorInDiContainer = true;
                ErrorMessage = $"[{ClassName}] {nameof(T)} is not find.";
                return null;
            }

            return component as T;
        }

        protected void SetError(string problemClass)
        {
            IsErrorInDiContainer = true;
            ErrorMessage = $"[{ClassName}] {problemClass} is not find.";
        }
    }
}
#endif
