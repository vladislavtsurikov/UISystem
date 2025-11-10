using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.SceneSettings.Camera;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.SceneSettings.Camera;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem
{
    public class ScriptingSystem
    {
        private Transform _colliderParent;
        private List<PrototypeScriptingManager> _prototypeScriptingManager = new();

        public void Setup()
        {
            CreateColliderParent();

            _prototypeScriptingManager = new List<PrototypeScriptingManager>();

            foreach (Prototype prototype in TerrainObjectRenderer.Instance.SelectionData.PrototypeList)
            {
                var prototypeScriptingManager =
                    new PrototypeScriptingManager((PrototypeTerrainObject)prototype, _colliderParent);

                _prototypeScriptingManager.Add(prototypeScriptingManager);
            }
        }

        public void OnDisable()
        {
            if (_prototypeScriptingManager == null)
            {
                return;
            }

            for (var j = 0; j <= _prototypeScriptingManager.Count - 1; j++)
            {
                PrototypeScriptingManager colliderManager = _prototypeScriptingManager[j];
                colliderManager?.OnDisable();
            }

            _prototypeScriptingManager.Clear();
            DestroyColliderParent();
        }

        public void SetCollidersAroundCameras()
        {
            if (_prototypeScriptingManager == null || _prototypeScriptingManager.Count == 0)
            {
                return;
            }

            if (_colliderParent == null)
            {
                Setup();
            }

            Profiler.BeginSample("Collider system processing");

            foreach (PrototypeScriptingManager prototypeScriptingManager in _prototypeScriptingManager)
            {
                if (prototypeScriptingManager == null)
                {
                    continue;
                }

                var collidersSettings = (Colliders)prototypeScriptingManager.Proto.GetSettings(typeof(Colliders));
                var scriptingSettings = (Scripting)prototypeScriptingManager.Proto.GetSettings(typeof(Scripting));

                if (collidersSettings.IsValid() || scriptingSettings.IsValid())
                {
                    var cameraManager = (CameraManager)RendererStackManager.Instance.SceneComponentStack
                        .GetElement(typeof(CameraManager));

                    foreach (VirtualCamera cam in cameraManager.VirtualCameraList)
                    {
                        if (!prototypeScriptingManager.Proto.Active)
                        {
                            prototypeScriptingManager.RemoveColliders(cam);
                            continue;
                        }

                        var cameraCollidersController =
                            (CameraCollidersController)cam.CameraTemporaryComponentStack.GetElement(
                                typeof(CameraCollidersController));
                        var terrainObjectRendererCameraSettings =
                            (TerrainObjectRendererCameraSettings)cam.CameraComponentStack.GetElement(
                                typeof(TerrainObjectRendererCameraSettings));

                        if (cam.Ignored || !terrainObjectRendererCameraSettings.EnableColliders)
                        {
                            if (cameraCollidersController.UsedForColliders)
                            {
                                prototypeScriptingManager.RemoveColliders(cam);
                            }

                            cameraCollidersController.UsedForColliders = false;
                            continue;
                        }

                        var currentSphere = new Sphere(cam.GetCameraPosition(),
                            prototypeScriptingManager.GetMaxDistance());

                        prototypeScriptingManager.SetColliders(currentSphere, cam);

                        cameraCollidersController.UsedForColliders = true;
                    }
                }
            }

            Profiler.EndSample();
        }

        public static void SetColliders(Sphere sphere, object obj)
        {
            ScriptingSystem scriptingSystem = TerrainObjectRenderer.Instance.ScriptingSystem;

            if (scriptingSystem._prototypeScriptingManager == null ||
                scriptingSystem._prototypeScriptingManager.Count == 0)
            {
                return;
            }

            if (scriptingSystem._colliderParent == null)
            {
                scriptingSystem.Setup();
            }

            foreach (PrototypeScriptingManager prototypeScriptingManager in scriptingSystem._prototypeScriptingManager)
            {
                if (!prototypeScriptingManager.Proto.Active)
                {
                    prototypeScriptingManager.RemoveColliders(obj);
                    continue;
                }

                var collidersSettings = (Colliders)prototypeScriptingManager.Proto.GetSettings(typeof(Colliders));
                var scriptingSettings = (Scripting)prototypeScriptingManager.Proto.GetSettings(typeof(Scripting));

                if (collidersSettings.IsValid() || scriptingSettings.IsValid())
                {
                    prototypeScriptingManager.SetColliders(sphere, obj);
                }
            }
        }

        public static void RemoveCollider(int prototypeID)
        {
            TerrainObjectRenderer terrainObjectRenderer = TerrainObjectRenderer.Instance;

            if (terrainObjectRenderer == null)
            {
                return;
            }

            ScriptingSystem scriptingSystem = terrainObjectRenderer.ScriptingSystem;

            if (scriptingSystem == null || scriptingSystem._colliderParent == null)
            {
                return;
            }

            if (scriptingSystem._prototypeScriptingManager.Count == 0)
            {
                return;
            }

            foreach (PrototypeScriptingManager prototypeScriptingManager in scriptingSystem._prototypeScriptingManager)
            {
                if (prototypeScriptingManager.Proto.ID == prototypeID)
                {
                    prototypeScriptingManager.DefaultPrototypeInstancesSelector.OnDisableCollider();
                    prototypeScriptingManager.CellsPrototypeInstancesSelector.OnDisableCollider();

                    return;
                }
            }
        }

        public static void RemoveCollider(TerrainObjectInstance instance)
        {
            TerrainObjectRenderer terrainObjectRenderer = TerrainObjectRenderer.Instance;

            if (terrainObjectRenderer == null)
            {
                return;
            }

            ScriptingSystem scriptingSystem = terrainObjectRenderer.ScriptingSystem;

            if (scriptingSystem == null || scriptingSystem._colliderParent == null)
            {
                return;
            }

            if (scriptingSystem._prototypeScriptingManager.Count == 0)
            {
                return;
            }

            foreach (PrototypeScriptingManager prototypeScriptingManager in scriptingSystem._prototypeScriptingManager)
            {
                if (prototypeScriptingManager.Proto.ID == instance.Proto.ID)
                {
                    prototypeScriptingManager.DefaultPrototypeInstancesSelector.OnInstanceInvisible(instance);
                    prototypeScriptingManager.CellsPrototypeInstancesSelector.OnInstanceInvisible(instance);

                    return;
                }
            }
        }

        public static void RemoveColliders(object usedObj)
        {
            TerrainObjectRenderer terrainObjectRenderer = TerrainObjectRenderer.Instance;

            if (terrainObjectRenderer == null)
            {
                return;
            }

            ScriptingSystem scriptingSystem = terrainObjectRenderer.ScriptingSystem;

            if (scriptingSystem == null || scriptingSystem._colliderParent == null)
            {
                return;
            }

            if (scriptingSystem._prototypeScriptingManager.Count == 0)
            {
                return;
            }

            foreach (PrototypeScriptingManager prototypeScriptingManager in scriptingSystem._prototypeScriptingManager)
            {
                prototypeScriptingManager.RemoveColliders(usedObj);
            }
        }

        private void CreateColliderParent()
        {
            var colliderParentObject = new GameObject("Terrain Object Renderer Colliders")
            {
                hideFlags = HideFlags.DontSave
            };

            if (_colliderParent)
            {
                DestroyColliderParent();
            }

            _colliderParent = colliderParentObject.transform;
        }

        private void DestroyColliderParent()
        {
            if (!_colliderParent)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(_colliderParent.gameObject);
            }
            else
            {
                Object.DestroyImmediate(_colliderParent.gameObject);
            }
        }

        private void OnDrawGizmosSelected()
        {
        }
    }
}
