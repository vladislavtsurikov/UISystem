using System;

namespace VladislavTsurikov.Utility.Runtime
{
    public static class TimeSpanExtensions
    {
        public static string ToReadableString(this TimeSpan span) =>
            span.TotalMinutes >= 1
                ? $"{(int)span.TotalMinutes}m {span.Seconds}s"
                : $"{span.Seconds}.{span.Milliseconds:D3}s";
    }
}
