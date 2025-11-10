using UnityEngine.UIElements;

namespace VladislavTsurikov.UIElementsUtility.Runtime.Utility
{
    public static class TextElementExtensions
    {
        /// <summary> Set the text associated with the element </summary>
        /// <param name="target"> Target TextElement </param>
        /// <param name="text"> Text value </param>
        public static T SetText<T>(this T target, string text) where T : TextElement
        {
            target.text = text;
            return target;
        }

        /// <summary> Get the text associated with the element </summary>
        /// <param name="target"> Target TextElement </param>
        public static string GetText<T>(this T target) where T : TextElement => target.text;
    }
}
