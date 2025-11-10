#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.UnityUtility.Editor;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using GameObjectUtility = VladislavTsurikov.UnityUtility.Runtime.GameObjectUtility;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem
{
    public abstract class Action : Component
    {
        public virtual void OnMouseMove()
        {
        }

        public virtual bool CheckShortcutCombo() => false;

        protected virtual void OnObjectFound()
        {
        }

        protected virtual Color GetColorHandleButton() => new(0.5f, 0.5f, 0.5f, 0.7f);

        protected virtual void RegisterUndo()
        {
        }

        public void HandleButtons()
        {
            var editToolSettings =
                (EditToolSettings)ToolsComponentStack.GetElement(typeof(EditTool), typeof(EditToolSettings));

            RayHit rayHit = ColliderUtility.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), ~0);
            if (rayHit != null)
            {
                if (WindowData.Instance.SelectedData.GetSelectedPrototypes(typeof(PrototypeGameObject)).Count != 0)
                {
                    PrototypeGameObjectOverlap.OverlapSphere(rayHit.Point, editToolSettings.SphereSize / 2,
                        (proto, go) =>
                        {
                            if (proto.Active == false || proto.Selected == false)
                            {
                                return true;
                            }

                            GameObject prefabRoot = GameObjectUtility.GetPrefabRoot(go);
                            if (prefabRoot == null)
                            {
                                return true;
                            }

                            Vector3 sceneCameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;
                            var distanceFromSceneCamera =
                                Vector3.Distance(sceneCameraPosition, prefabRoot.transform.position);

                            if (distanceFromSceneCamera > editToolSettings.MaxDistance)
                            {
                                return true;
                            }

                            if (DrawHandles.HandleButton(ToolWindow.EditorHash + prefabRoot.GetHashCode(),
                                    prefabRoot.transform.position, GetColorHandleButton(), new Color(1f, 1f, 0f, 0.7f)))
                            {
                                EditTool.FindObject = new CommonInstance(proto, prefabRoot);

                                RegisterUndo();
                                OnObjectFound();
                                return false;
                            }

                            return true;
                        });
                }

#if RENDERER_STACK
                if (WindowData.Instance.SelectedData.GetSelectedPrototypes(typeof(PrototypeTerrainObject)).Count != 0)
                {
                    PrototypeTerrainObjectOverlap.OverlapSphere(rayHit.Point, editToolSettings.SphereSize / 2, null,
                        false, true, (proto, obj) =>
                        {
                            if (proto.Active == false || proto.Selected == false)
                            {
                                return true;
                            }

                            Vector3 sceneCameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;
                            var distanceFromSceneCamera = Vector3.Distance(sceneCameraPosition, obj.Position);

                            if (distanceFromSceneCamera > editToolSettings.MaxDistance)
                            {
                                return true;
                            }

                            if (DrawHandles.HandleButton(ToolWindow.EditorHash + obj.GetHashCode(), obj.Position,
                                    GetColorHandleButton(), new Color(1f, 1f, 0f, 0.7f)))
                            {
                                EditTool.FindObject = new CommonInstance(proto, obj);

                                RegisterUndo();
                                OnObjectFound();
                                return false;
                            }

                            return true;
                        });
                }
#endif
            }
        }
    }
}
#endif
