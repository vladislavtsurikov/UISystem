using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem
{
    [Serializable]
    public class Group : IHasName, ISelectable
    {
        [SerializeField]
        private bool _selected;

        [SerializeField]
        private string _name;

        [OdinSerialize]
        public List<Prototype> PrototypeList = new();

        public Group(string name) => Name = name;

        public Group(string name, bool selected)
        {
            Name = name;
            _selected = selected;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public bool Selected
        {
            get => _selected;
            set => _selected = value;
        }

        public void Setup(Type rendererType)
        {
            SyncsReferencesWithAllPrototypeStack(rendererType);

            foreach (Prototype prototype in PrototypeList)
            {
                prototype.Setup();
            }
        }

        public void SyncsReferencesWithAllPrototypeStack(Type rendererType)
        {
            for (var i = 0; i < PrototypeList.Count; i++)
            {
                Prototype proto = PrototypesStorage.Instance.GetPrototype(PrototypeList[i].ID, rendererType);
                if (proto != null)
                {
                    PrototypeList[i] = proto;
                }
            }
        }

        public void SelectAllPrototypes() => PrototypeList.ForEach(proto => proto.Selected = true);

        internal bool DeleteInvalidPrototypes(Type rendererType)
        {
            var happenedDelete = false;

            for (var i = PrototypeList.Count - 1; i >= 0; i--)
            {
                if (PrototypeList[i] == null ||
                    !PrototypesStorage.Instance.HasPrototype(PrototypeList[i], rendererType))
                {
                    PrototypeList.RemoveAt(i);
                    happenedDelete = true;
                }
            }

            return happenedDelete;
        }
    }
}
