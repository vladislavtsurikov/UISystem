using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core.Ports;

namespace VladislavTsurikov.Nody.Runtime.Core
{
    public abstract class Node : Element, ISelectable, IRemovable
    {
        [OdinSerialize]
        [HideInInspector]
        protected bool _selected;

        //protected internal object Stack;
        [NonSerialized]
        [HideInInspector]
        public object Stack;

        [OdinSerialize]
        [HideInInspector]
        private string _nodeId = Guid.NewGuid().ToString();

        [OdinSerialize]
        [HideInInspector]
        private Vector2 _graphPosition;

        [NonSerialized]
        private PortDefinitionContext _portCache;

        public event Action<Node> Dirtied;

        public string NodeId
        {
            get => _nodeId;
            set => _nodeId = value;
        }

        public Vector2 GraphPosition
        {
            get => _graphPosition;
            set => _graphPosition = value;
        }

        void IRemovable.OnRemove()
        {
            IsSetup = false;
            OnDeleteElement();
            ((IDisableable)this).OnDisable();
        }

        public bool Selected
        {
            get => _selected;
            set
            {
                var changedSelect = _selected != value;

                _selected = value;

                if (changedSelect)
                {
                    if (value)
                    {
                        OnSelect();
                    }
                    else
                    {
                        OnDeselect();
                    }
                }
            }
        }

        public bool IsDeletable()
        {
            if (GetType().GetAttribute<PersistentNodeAttribute>() != null)
            {
                return false;
            }

            return IsDeletableNode();
        }

        protected virtual bool IsDeletableNode() => true;

        internal void OnCreateInternal() => OnCreate();

        protected virtual void OnDeleteElement()
        {
        }

        protected virtual void OnCreate()
        {
        }

        protected virtual void OnDeselect()
        {
        }

        protected virtual void OnSelect()
        {
        }

        public virtual bool DeleteElement() => true;

        protected override void OnDirtied()
        {
            Dirtied?.Invoke(this);
        }

        public virtual void OnDefinePorts(PortDefinitionContext context)
        {
        }

        public PortDefinitionContext GetPorts()
        {
            if (_portCache == null)
            {
                _portCache = new PortDefinitionContext();
                OnDefinePorts(_portCache);
            }
            return _portCache;
        }

        protected void InvalidatePorts()
        {
            _portCache = null;
        }

        public virtual void ExecuteInStack(object context)
        {
        }

        public virtual void ExecuteInGraph(Dictionary<string, object> inputs, out Dictionary<string, object> outputs)
        {
            outputs = new Dictionary<string, object>();
        }
    }
}
