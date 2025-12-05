#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Undo.Editor.GameObject;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Events
{
    [InitializeOnLoad]
    public static class UndoEvents
    {
        static UndoEvents()
        {
            DestroyedGameObject.UndoPerformed -= DestroyedGameObjectUndoPerformed;
            DestroyedGameObject.UndoPerformed += DestroyedGameObjectUndoPerformed;
        }

        private static void DestroyedGameObjectUndoPerformed(List<GameObject> gameObjectList)
        {
            foreach (GameObject go in gameObjectList)
            {
                GameObjectCollider.Editor.GameObjectCollider.RegisterGameObjectToCurrentScene(go);
            }
        }
    }
}
#endif
