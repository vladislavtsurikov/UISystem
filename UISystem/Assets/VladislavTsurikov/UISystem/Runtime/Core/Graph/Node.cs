using System;
using System.Collections.Generic;
using OdinSerializer;

namespace VladislavTsurikov.UISystem.Runtime.Core.Graph
{
    [Serializable]
    public class Node
    {
        [OdinSerialize]
        public List<Node> Children = new();

        [OdinSerialize]
        public List<Type> Filters = new();

        [OdinSerialize]
        public Type HandlerType;
    }
}
