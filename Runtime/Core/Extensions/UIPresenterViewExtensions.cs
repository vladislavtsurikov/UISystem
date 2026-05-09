namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public static class UIPresenterViewExtensions
    {
        public static TView GetView<TView>(this UIPresenter presenter, string bindingId, int index = 0)
        {
            return presenter.ViewResolver.GetView<TView>(bindingId, index);
        }
    }
}
