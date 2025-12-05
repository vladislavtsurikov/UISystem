namespace VladislavTsurikov.ComponentStack.Runtime.Core
{
    public static class ComponentEx
    {
        public static bool IsValid(this Component component)
        {
            if (component == null || !component.Active)
            {
                return false;
            }

            return true;
        }
    }
}
