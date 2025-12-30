namespace VladislavTsurikov.Utility.Runtime
{
    public abstract class DataTypeSingleton<T> where T : DataTypeSingleton<T>, new()
    {
        public static T Instance { get; } = new T();
    }
}

