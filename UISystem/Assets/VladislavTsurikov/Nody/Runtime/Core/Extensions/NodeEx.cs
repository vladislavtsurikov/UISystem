namespace VladislavTsurikov.Nody.Runtime.Core
{
    public static class NodeEx
    {
        public static bool IsValid(this Node node)
        {
            if (node == null || !node.Active)
            {
                return false;
            }

            return true;
        }
    }
}
