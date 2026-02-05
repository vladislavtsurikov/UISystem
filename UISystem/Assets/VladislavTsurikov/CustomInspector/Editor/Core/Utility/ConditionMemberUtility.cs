using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public static class ConditionMemberUtility
    {
        public static bool TryGetConditionValue(object target, string memberName, out object value)
        {
            value = null;
            if (target == null || string.IsNullOrWhiteSpace(memberName))
            {
                return false;
            }

            FieldInfo field = target.GetType().GetField(memberName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
            {
                return false;
            }

            value = field.GetValue(target);
            return true;
        }

        public static bool IsTruthy(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is bool boolValue)
            {
                return boolValue;
            }

            if (value is UnityEngine.Object unityObject)
            {
                return unityObject != null;
            }

            return true;
        }
    }
}
