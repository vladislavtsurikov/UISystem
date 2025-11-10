namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public static class StringExtensions
    {
        public static string ToLowerFirstChar(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var firstChar = char.ToLower(input[0]);
            return firstChar + input.Substring(1);
        }
    }
}
