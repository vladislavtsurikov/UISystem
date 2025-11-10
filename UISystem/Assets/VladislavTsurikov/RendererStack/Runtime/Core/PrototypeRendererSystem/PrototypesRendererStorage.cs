using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEditor;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem
{
    [Serializable]
    public class PrototypesRendererStorage
    {
        [OdinSerialize]
        private List<Prototype> _prototypeList = new();

        public Type RendererType;

        public PrototypesRendererStorage(Type rendererType) => RendererType = rendererType;

        public void DeleteInvalidPrototypes()
        {
            var removePrototypeList = new List<Prototype>();

            foreach (Prototype prototype in _prototypeList)
            {
                if (prototype == null || prototype.PrototypeObject == null)
                {
                    removePrototypeList.Add(prototype);
                }
            }

            if (removePrototypeList.Count != 0)
            {
                Remove(removePrototypeList);
            }
        }

        public Prototype GeneratePrototypeIfNecessary(Object obj, Type prototypeType)
        {
            Prototype prototype = GetPrototype(obj);

            var id = obj.GetInstanceID();

            if (prototype == null)
            {
                prototype = (Prototype)Activator.CreateInstance(prototypeType);
                prototype.Init(id, obj, RendererType);
                _prototypeList.Add(prototype);

#if UNITY_EDITOR
                EditorUtility.SetDirty(PrototypesStorage.Instance);
#endif
            }

            return prototype;
        }

        public void Remove(List<Prototype> protoList)
        {
            if (_prototypeList.RemoveAll(protoList.Contains) != 0)
            {
#if UNITY_EDITOR
                EditorUtility.SetDirty(PrototypesStorage.Instance);
#endif
            }
        }

        public Prototype GetPrototype(Object obj)
        {
            foreach (Prototype proto in _prototypeList)
            {
                if (proto.IsSamePrototypeObject(obj))
                {
                    return proto;
                }
            }

            return null;
        }

        public Prototype GetPrototype(int id)
        {
            foreach (Prototype proto in _prototypeList)
            {
                if (proto.ID == id)
                {
                    return proto;
                }
            }

            return null;
        }

        public bool HasPrototype(Prototype findProto)
        {
            foreach (Prototype proto in _prototypeList)
            {
                if (proto.ID == findProto.ID)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
