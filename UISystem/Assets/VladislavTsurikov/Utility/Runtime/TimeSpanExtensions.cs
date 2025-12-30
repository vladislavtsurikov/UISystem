using System;

namespace VladislavTsurikov.Utility.Runtime
{
    public static class TimeSpanExtensions
    {
        public static string ToReadableDetailed(this TimeSpan span)
        {
            if (span.TotalMinutes >= 1)
            {
                return $"{(int)span.TotalMinutes}m {span.Seconds}.{span.Milliseconds:D3}s";
            }

            return $"{span.TotalSeconds:F3}s";
        }
    }
}