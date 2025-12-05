using System;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.DeepCopy.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.MonoBehaviour;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data
{
    public class TerrainObjectInstance : Instance
    {
        private readonly ComponentStackOnlyDifferentTypes<Script> _scriptList = new();

        public readonly int PrototypeID;
        private bool _enable = true;

        private PrototypeTerrainObject _proto;

        [NonSerialized]
        public HierarchyTerrainObjectInstance HierarchyTerrainObjectInstance;

        [NonSerialized]
        public TerrainObjectCollider TerrainObjectCollider;

        public TerrainObjectInstance(Vector3 position, Vector3 scale, Quaternion rotation, PrototypeTerrainObject proto)
            : base(position, scale, rotation)
        {
            _proto = proto;
            PrototypeID = proto.ID;
            _scriptList.Setup(true, new object[] { this });
        }

        public TerrainObjectInstance(int id, Vector3 position, Vector3 scale, Quaternion rotation,
            PrototypeTerrainObject proto) : base(id, position, scale, rotation)
        {
            _proto = proto;
            PrototypeID = proto.ID;
            _scriptList.Setup(true, new object[] { this });
        }

        public PrototypeTerrainObject Proto
        {
            get
            {
                if (_proto == null)
                {
                    _proto = (PrototypeTerrainObject)TerrainObjectRenderer.Instance.SelectionData.GetProto(PrototypeID);
                }

                return _proto;
            }
        }

        public bool Enable
        {
            get => _enable;
            set
            {
                if (_enable == value)
                {
                    return;
                }

                _enable = value;

                TerrainObjectCollider.PathToTerrainObjectCollider.PrototypeRendererData.SetEnable(this);
            }
        }

        public bool HasScriptType(Type type) => GetScript(type) != null;

        public void AddScript(Script protoScript)
        {
            if (!HasScriptType(protoScript.GetType()))
            {
                Script script = DeepCopier.Copy(protoScript);
                _scriptList.AddIfMissingType(script);
            }
        }

        public void SetupScripts() => _scriptList.Setup();

        public Script[] GetScripts() => _scriptList.ElementList.ToArray();

        public Script GetScript(Type type) => _scriptList.GetElement(type);

        protected override void DestroyInstance()
        {
            PathToTerrainObjectCollider pathToTerrainObjectCollider = TerrainObjectCollider.PathToTerrainObjectCollider;

            pathToTerrainObjectCollider.BVHObjectTree.Tree.RemoveLeafNode(pathToTerrainObjectCollider.LeafNode);
            pathToTerrainObjectCollider.PrototypeRendererData.RemovePersistentInstance(this);

            pathToTerrainObjectCollider.ColliderCell.ChangeNodeSizeIfNecessary(TerrainObjectCollider);
            pathToTerrainObjectCollider.RenderCell.ChangeNodeSizeIfNecessary(TerrainObjectCollider);

            SceneObjectsBounds.ChangeSceneObjectsBounds(pathToTerrainObjectCollider.SceneDataManager.Sector);

            if (HierarchyTerrainObjectInstance != null)
            {
                ScriptingSystem.ScriptingSystem.RemoveCollider(this);
            }
        }

        protected override void TransformChanged()
        {
            PathToTerrainObjectCollider pathToTerrainObjectCollider = TerrainObjectCollider.PathToTerrainObjectCollider;

            pathToTerrainObjectCollider.BVHObjectTree.Tree.RemoveLeafNode(pathToTerrainObjectCollider.LeafNode);

            pathToTerrainObjectCollider.PrototypeRendererData.RemovePersistentInstance(this);

            TerrainObjectRendererData.AddInstance(this, Sectorize.Sectorize.GetSectorLayerTag());

            if (HierarchyTerrainObjectInstance != null)
            {
                GameObject gameObject = HierarchyTerrainObjectInstance.gameObject;

                gameObject.transform.position = Position;
                gameObject.transform.rotation = Rotation;
                gameObject.transform.localScale = Scale;
            }
        }

        protected override void Reposition()
        {
            PathToTerrainObjectCollider pathToTerrainObjectCollider = TerrainObjectCollider.PathToTerrainObjectCollider;

            pathToTerrainObjectCollider.BVHObjectTree.Tree.RemoveLeafNode(pathToTerrainObjectCollider.LeafNode);

            pathToTerrainObjectCollider.PrototypeRendererData.RemovePersistentInstance(this);

            TerrainObjectRendererData.AddInstance(this, Sectorize.Sectorize.GetSectorLayerTag());

            if (HierarchyTerrainObjectInstance != null)
            {
                GameObject gameObject = HierarchyTerrainObjectInstance.gameObject;

                gameObject.transform.position = Position;
            }
        }

        protected override Vector3 GetMultiplySize() => Proto.RenderModel.MultiplySize;
    }
}
