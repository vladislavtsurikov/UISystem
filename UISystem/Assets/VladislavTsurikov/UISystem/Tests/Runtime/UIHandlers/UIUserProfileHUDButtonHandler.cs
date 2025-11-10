namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    /*[SceneFilter("TestScene_1", "TestScene_2")]
    [ParentUIHandler(typeof(HUDScene1Handler))]
    public class UIUserProfileHUDButtonHandler : ButtonUIHandler
    {
        public UIUserProfileHUDButtonHandler(DiContainer container) : base(container, typeof(UIUserProfileHUDButton))
        {
        }

        protected override async UniTask InitializeButtonUIHandler(IBindableUIComponent view, CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            var buttonView = (UIUserProfileHUDButton)view;

            buttonView.OnClick
                .Subscribe(async _ =>
                {
                    await UINavigator.Show<UIUserProfilePresenter>(cancellationToken);
                })
                .AddTo(Disposables);

            await UniTask.CompletedTask;
        }
    }*/
}
