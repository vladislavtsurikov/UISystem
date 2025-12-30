namespace VladislavTsurikov.Utility.Runtime.Extensions
{
    public static class ParamArgsExtensions
    {
        public static T GetArg<T>(this object[] args) where T : class
        {
            if (args == null)
            {
                return null;
            }

            for (int i = 0; i < args.Length; i++)
            {
                T value = args[i] as T;
                if (value != null)
                {
                    return value;
                }
            }

            return null;
        }
    }
}
