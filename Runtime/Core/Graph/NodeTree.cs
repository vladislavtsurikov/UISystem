using System;
using System.Collections.Generic;
using OdinSerializer;

namespace VladislavTsurikov.UISystem.Runtime.Core.Graph
{
    [Serializable]
    public class NodeTree
    {
        [OdinSerialize]
        public List<Node> Roots = new();
    }
}
