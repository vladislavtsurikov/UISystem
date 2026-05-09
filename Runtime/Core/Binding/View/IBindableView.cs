namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public interface IBindableView
    {
        string BindingId => GetType().Name;
    }
}
