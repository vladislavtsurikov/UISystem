#if UNITY_EDITOR
#if RENDERER_STACK
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;
using VladislavTsurikov.UnityUtility.Runtime;
using Mesh = VladislavTsurikov.ColliderSystem.Runtime.Mesh;

namespace VladislavTsurikov.GameObjectCollider.Editor.Debug
{
    /// <summary>
    ///     An enum which allows us to switch between different demos. Essentially,
    ///     it will be used to control the type of test that we will perform in the
    ///     scene view (raycast, overlap etc).
    /// </summary>
    public enum OverlapMode
    {
        BoxOverlap,
        SphereOverlap
    }

    public enum ShowMode
    {
        Object,
        Scene
    }

    public enum DebugObjectMode
    {
        ShowAllCells,
        ShowHitRaycastCells,
        ShowMeshTree,
        ShowHitRaycast,
        ShowOverlapObjects
    }

    public enum DebugSceneMode
    {
        ShowAllCells,
        ShowOverlapScenes
    }

    /// <summary>
    ///     A simple demo class which allows the user to perform different tests using the
    ///     CFE API and show the results inside the scene view.
    /// </summary>
    [ExecuteInEditMode]
    public class ColliderDebug : MonoBehaviourSingleton<ColliderDebug>
    {
        public LayerMask LayerMask;
        public LayerMask OverlapObjectLayerMask;
        public ShowMode ShowMode = ShowMode.Object;
        public DebugObjectMode DebugObjectMode = DebugObjectMode.ShowAllCells;
        public DebugSceneMode DebugSceneMode = DebugSceneMode.ShowAllCells;

        [SerializeField]
        private OverlapMode _demoMode = OverlapMode.SphereOverlap;

        [SerializeField]
        private Vector3 _overlapBoxSize = Vector3.one;

        [SerializeField]
        private Vector3 _overlapBoxEuler = Vector3.zero;

        [SerializeField]
        private float _overlapSphereRadius = 1.0f;

        [SerializeField]
        private Color _overlapShapeColor = Color.blue.WithAlpha(0.5f);

        [SerializeField]
        private Color _overlappedSolidColor = Color.green.WithAlpha(0.5f);

        [SerializeField]
        private Color _overlappedWireColor = Color.black;


        [SerializeField]
        private float _hitPointSize = 0.08f;

        [SerializeField]
        private Color _hitNormalColor = Color.green;

        [SerializeField]
        private Color _hitPointColor = Color.green;

        public Color HitTriangleColor = Color.red;
        public Color NodeColor = Color.white;
        private ColliderObject _bvhObject;

        private RayHit _objectRayHit;

        private Vector3 _overlapBoxCenter;
        private List<ColliderObject> _overlappedObjects = new();
        private List<SceneDataManager> _overlappedScenes = new();

        private Vector3 _overlapSphereCenter;

        public Vector3 OverlapBoxSize
        {
            get => _overlapBoxSize;
            set =>
                _overlapBoxSize =
                    Vector3.Max(value, Vector3.one * 1e-5f);
        }

        public float OverlapSphereRadius
        {
            get => _overlapSphereRadius;
            set =>
                _overlapSphereRadius =
                    Mathf.Max(value, 1e-5f);
        }

        public float HitPointSize
        {
            get => _hitPointSize;
            set => _hitPointSize = Mathf.Max(1e-5f, value);
        }

        /// <summary>
        ///     Called when the script is enabled.
        /// </summary>
        private void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;

        /// <summary>
        ///     Called when the script is disabled.
        /// </summary>
        private void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

        private void OnDrawGizmosSelected()
        {
            switch (ShowMode)
            {
                case ShowMode.Object:
                {
                    switch (DebugObjectMode)
                    {
                        case DebugObjectMode.ShowAllCells:
                        {
                            DrawAllCells();

                            break;
                        }
                        case DebugObjectMode.ShowMeshTree:
                        {
                            if (_objectRayHit != null)
                            {
                                if (_objectRayHit.MeshRayHit != null)
                                {
                                    // Retrieve the editor mesh instance based on the Unity mesh which is attached to the hit object
                                    Mesh editorMesh = _bvhObject.GetMesh();

                                    editorMesh.DrawAllCells(_bvhObject.GetMatrix(), NodeColor);
                                }
                            }

                            ShowHitRaycast();

                            break;
                        }
                        case DebugObjectMode.ShowHitRaycast:
                        {
                            ShowHitRaycast();

                            break;
                        }
                        case DebugObjectMode.ShowHitRaycastCells:
                        {
                            if (_objectRayHit != null)
                            {
                                if (_objectRayHit.MeshRayHit != null)
                                {
                                    // Retrieve the editor mesh instance based on the Unity mesh which is attached to the hit object
                                    Mesh editorMesh = _bvhObject.GetMesh();

                                    editorMesh.DrawRaycast(
                                        HandleUtility.GUIPointToWorldRay(Event.current.mousePosition),
                                        _bvhObject.GetMatrix(), NodeColor);
                                }
                            }

                            DrawRaycast();

                            ShowHitRaycast();

                            break;
                        }
                        case DebugObjectMode.ShowOverlapObjects:
                        {
                            if (_demoMode == OverlapMode.BoxOverlap)
                            {
                                // Draw the box shape
                                GizmosEx.PushColor(_overlapShapeColor);
                                GizmosEx.PushMatrix(Matrix4x4.TRS(_overlapBoxCenter, Quaternion.Euler(_overlapBoxEuler),
                                    _overlapBoxSize));
                                Gizmos.DrawCube(Vector3.zero, Vector3.one);
                                GizmosEx.PopMatrix();
                                GizmosEx.PopColor();

                                // Draw the overlapped volumes
                                DrawOverlappedVolumesGizmos();
                            }
                            else if (_demoMode == OverlapMode.SphereOverlap)
                            {
                                // Draw the sphere shape
                                GizmosEx.PushColor(_overlapShapeColor);
                                GizmosEx.PushMatrix(Matrix4x4.TRS(_overlapSphereCenter, Quaternion.identity,
                                    Vector3.one * _overlapSphereRadius));
                                Gizmos.DrawSphere(Vector3.zero, 1.0f);
                                GizmosEx.PopMatrix();
                                GizmosEx.PopColor();

                                // Draw the overlapped volumes
                                DrawOverlappedVolumesGizmos();
                            }

                            break;
                        }
                    }

                    break;
                }
                case ShowMode.Scene:
                {
                    switch (DebugSceneMode)
                    {
                        case DebugSceneMode.ShowAllCells:
                        {
                            SectorLayer.GetCurrentSectorLayers()[0].ObjectBoundsBVHTree.DrawAllCells(NodeColor);

                            break;
                        }
                        case DebugSceneMode.ShowOverlapScenes:
                        {
                            if (_demoMode == OverlapMode.BoxOverlap)
                            {
                                // Draw the box shape
                                GizmosEx.PushColor(_overlapShapeColor);
                                GizmosEx.PushMatrix(Matrix4x4.TRS(_overlapBoxCenter, Quaternion.Euler(_overlapBoxEuler),
                                    _overlapBoxSize));
                                Gizmos.DrawCube(Vector3.zero, Vector3.one);
                                GizmosEx.PopMatrix();
                                GizmosEx.PopColor();

                                // Draw the overlapped volumes
                                DrawOverlappedScenesVolumesGizmos();
                            }
                            else if (_demoMode == OverlapMode.SphereOverlap)
                            {
                                // Draw the sphere shape
                                GizmosEx.PushColor(_overlapShapeColor);
                                GizmosEx.PushMatrix(Matrix4x4.TRS(_overlapSphereCenter, Quaternion.identity,
                                    Vector3.one * _overlapSphereRadius));
                                Gizmos.DrawSphere(Vector3.zero, 1.0f);
                                GizmosEx.PopMatrix();
                                GizmosEx.PopColor();

                                // Draw the overlapped volumes
                                DrawOverlappedScenesVolumesGizmos();
                            }

                            break;
                        }
                    }

                    break;
                }
            }
        }

        public void ShowHitRaycast()
        {
            if (_objectRayHit != null)
            {
                // Use a yellow color. Seems to work really well at least with the dev's workspace.
                var style = new GUIStyle("label");
                style.normal.textColor = Color.yellow;

                // Build the label text. We will show the coordinates of the hit point and the hit point normal.
                var labelText =
                    "Hit Point: " + _objectRayHit.Point + "; \r\nHit Normal: " + _objectRayHit.Normal;
                Handles.Label(_objectRayHit.Point, new GUIContent(labelText), style);

                // Draw a sphere centered on the position of the hit point and a normal emenating from that point
                GizmosEx.PushColor(_hitPointColor);
                Gizmos.DrawSphere(_objectRayHit.Point,
                    _hitPointSize * HandleUtility.GetHandleSize(_objectRayHit.Point));
                GizmosEx.PopColor();
                GizmosEx.PushColor(_hitNormalColor);
                Gizmos.DrawLine(_objectRayHit.Point,
                    _objectRayHit.Point + _objectRayHit.Normal * HandleUtility.GetHandleSize(_objectRayHit.Point));
                GizmosEx.PopColor();

                if (_objectRayHit.MeshRayHit != null)
                {
                    // Retrieve the editor mesh instance based on the Unity mesh which is attached to the hit object
                    Mesh editorMesh = _bvhObject.GetMesh();
                    if (editorMesh != null)
                    {
                        // Push the triangle forward a bit along its own normal so that the triangle
                        // line pixels won't fight with the mesh geometry.
                        Vector3 triangleOffset = _objectRayHit.Normal * 1e-3f;

                        // Activate the triangle color and retrieve the triangle vertices
                        GizmosEx.PushColor(HitTriangleColor);
                        List<Vector3> triangleVerts =
                            editorMesh.GetTriangleVerts(_objectRayHit.MeshRayHit.TriangleIndex, _bvhObject.GetMatrix());
                        for (var vertIndex = 0; vertIndex < 3; ++vertIndex)
                        {
                            // Draw the current line
                            Gizmos.DrawLine(triangleVerts[vertIndex] + triangleOffset,
                                triangleVerts[(vertIndex + 1) % 3] + triangleOffset);
                        }

                        // Restore the color
                        GizmosEx.PopColor();
                    }
                }
            }
        }

        /// <summary>
        ///     Event handler for the 'SceneView.onSceneGUIDelegate' event.
        /// </summary>
        /// <remarks>
        ///     You could also just have a custom editor (e.g. class MyCustomEditor : Editor {})
        ///     for your own MonoBehaviour and implement the logic there inside the OnSceneGUI
        ///     function. The advantage of using an event handler is that the object does not have
        ///     to be selected for the logic to execute.
        /// </remarks>
        private void OnSceneGUI(SceneView sceneView)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            if (!Selection.objects.Contains(gameObject))
            {
                return;
            }

            // We only do anything if the current event is a mouse move event
            Event e = Event.current;
            if (e.type == EventType.MouseMove)
            {
                // Reset data
                _objectRayHit = null;
                _bvhObject = null;
                _overlappedObjects.Clear();
                _overlappedScenes.Clear();

                switch (ShowMode)
                {
                    case ShowMode.Object:
                    {
                        switch (DebugObjectMode)
                        {
                            case DebugObjectMode.ShowHitRaycast:
                            case DebugObjectMode.ShowHitRaycastCells:
                            case DebugObjectMode.ShowMeshTree:
                            {
                                var objectFilter = new ObjectFilter();
                                objectFilter.LayerMask = LayerMask;

                                _objectRayHit =
                                    ColliderUtility.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition),
                                        objectFilter);

                                if (_objectRayHit != null)
                                {
                                    if (_objectRayHit.Object is BVHGameObject)
                                    {
                                        _bvhObject = (BVHGameObject)_objectRayHit.Object;
                                    }
                                }

                                break;
                            }
                            case DebugObjectMode.ShowOverlapObjects:
                            {
                                var objectFilter = new ObjectFilter();
                                objectFilter.LayerMask = LayerMask;

                                var overlapObjecFilter = new ObjectFilter();
                                overlapObjecFilter.LayerMask = OverlapObjectLayerMask;

                                if (_demoMode == OverlapMode.BoxOverlap)
                                {
                                    RayHit objectHit =
                                        ColliderUtility.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition),
                                            objectFilter);
                                    if (objectHit != null)
                                    {
                                        // Perform the overlap test
                                        _overlapBoxCenter = objectHit.Point;
                                        _overlappedObjects =
                                            GameObjectColliderUtility.OverlapBox(_overlapBoxCenter, _overlapBoxSize,
                                                Quaternion.Euler(_overlapBoxEuler), overlapObjecFilter);
                                    }
                                }
                                else if (_demoMode == OverlapMode.SphereOverlap)
                                {
                                    RayHit objectHit =
                                        ColliderUtility.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition),
                                            objectFilter);
                                    if (objectHit != null)
                                    {
                                        _overlapSphereCenter = objectHit.Point;
                                        _overlappedObjects =
                                            GameObjectColliderUtility.OverlapSphere(_overlapSphereCenter,
                                                _overlapSphereRadius, overlapObjecFilter);
                                    }
                                }

                                break;
                            }
                        }

                        break;
                    }
                    case ShowMode.Scene:
                    {
                        switch (DebugSceneMode)
                        {
                            case DebugSceneMode.ShowOverlapScenes:
                            {
                                var objectFilter = new ObjectFilter();
                                objectFilter.LayerMask = LayerMask;

                                if (_demoMode == OverlapMode.BoxOverlap)
                                {
                                    RayHit objectHit =
                                        ColliderUtility.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition),
                                            objectFilter);
                                    if (objectHit == null)
                                    {
                                        objectHit =
                                            ColliderUtility.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition),
                                                LayerMask);
                                    }

                                    if (objectHit != null)
                                    {
                                        // Perform the overlap test
                                        _overlapBoxCenter = objectHit.Point;
                                        _overlappedScenes =
                                            SceneDataManagerFinder.OverlapBox(_overlapBoxCenter, _overlapBoxSize,
                                                Quaternion.Euler(_overlapBoxEuler));
                                    }
                                }
                                else if (_demoMode == OverlapMode.SphereOverlap)
                                {
                                    RayHit objectHit =
                                        ColliderUtility.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition),
                                            objectFilter);
                                    if (objectHit != null)
                                    {
                                        _overlapSphereCenter = objectHit.Point;
                                        _overlappedScenes =
                                            SceneDataManagerFinder.OverlapSphere(_overlapSphereCenter,
                                                _overlapSphereRadius);
                                    }
                                }

                                break;
                            }
                        }

                        break;
                    }
                }

                sceneView.Repaint();
            }
        }

        private void DrawOverlappedVolumesGizmos()
        {
            // Set the color and then loop through each overlapped object and draw it
            GizmosEx.PushColor(_overlappedSolidColor);
            foreach (ColliderObject gameObj in _overlappedObjects)
            {
                // Calculate the object's world OBB. If the OBB is valid, draw it.
                OBB worldObb = gameObj.GetOBB();
                if (worldObb.IsValid)
                {
                    // Inflate the OBB a bit to avoid any Z wars (e.g. cubes)
                    worldObb.Inflate(1e-3f);

                    // Activate the trasnform matrix and then draw.
                    // Note: We use the OBB's transform information to build the matrix
                    GizmosEx.PushMatrix(Matrix4x4.TRS(worldObb.Center, worldObb.Rotation, worldObb.Size));
                    Gizmos.DrawCube(Vector3.zero, Vector3.one);
                    GizmosEx.PushColor(_overlappedWireColor);
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                    GizmosEx.PopColor();
                    GizmosEx.PopMatrix();
                }
            }

            // Restore color
            GizmosEx.PopColor();
        }

        private void DrawOverlappedScenesVolumesGizmos()
        {
            // Set the color and then loop through each overlapped object and draw it
            GizmosEx.PushColor(_overlappedSolidColor);
            foreach (SceneDataManager sceneDataManager in _overlappedScenes)
            {
                AABB aabb =
                    SectorLayer.GetCurrentSectorLayers()[0].SectorBvhTree
                        .GetAABB(SectorLayerManager.Instance.GetSector(sceneDataManager.Scene));
                var worldObb = new OBB(aabb.Center, aabb.Size);

                if (worldObb.IsValid)
                {
                    // Inflate the OBB a bit to avoid any Z wars (e.g. cubes)
                    worldObb.Inflate(1e-3f);

                    // Activate the trasnform matrix and then draw.
                    // Note: We use the OBB's transform information to build the matrix
                    GizmosEx.PushMatrix(Matrix4x4.TRS(worldObb.Center, worldObb.Rotation, worldObb.Size));
                    Gizmos.DrawCube(Vector3.zero, Vector3.one);
                    GizmosEx.PushColor(_overlappedWireColor);
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                    GizmosEx.PopColor();
                    GizmosEx.PopMatrix();
                }
            }

            // Restore color
            GizmosEx.PopColor();
        }

        public void DrawAllCells()
        {
            foreach (SceneDataManager sceneDataManager in SceneDataManagerUtility.GetAllSceneDataManager())
            {
                var gameObjectCollider =
                    (GameObjectCollider)sceneDataManager.SceneDataStack.GetElement(typeof(GameObjectCollider));

                if (gameObjectCollider != null)
                {
                    gameObjectCollider.DrawAllCells(NodeColor);
                }
            }
        }

        public void DrawRaycast()
        {
            foreach (SceneDataManager sceneDataManager in SceneDataManagerUtility.GetAllSceneDataManager())
            {
                var gameObjectCollider =
                    (GameObjectCollider)sceneDataManager.SceneDataStack.GetElement(typeof(GameObjectCollider));

                if (gameObjectCollider != null)
                {
                    gameObjectCollider.DrawRaycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition),
                        NodeColor);
                }
            }
        }
    }
}
#endif
#endif
