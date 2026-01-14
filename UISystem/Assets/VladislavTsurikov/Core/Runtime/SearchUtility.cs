using System;

namespace VladislavTsurikov.Core.Runtime
{
    public static class SearchUtility
    {
        public static bool Contains(string source, string query)
        {
            if (string.IsNullOrEmpty(source))
                return string.IsNullOrEmpty(query);

            if (string.IsNullOrEmpty(query))
                return true;

            return source.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
