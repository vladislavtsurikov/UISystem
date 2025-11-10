using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem
{
    [Name("Spawn Prefabs")]
    [AfterLoadSceneComponent]
    public class SpawnPrefabs : Operation
    {
        public List<GameObject> GameObjects = new();

        public override async UniTask DoOperation()
        {
            foreach (GameObject gameObject in GameObjects)
            {
                Object.Instantiate(gameObject);
            }
            await UniTask.CompletedTask;
        }
    }
}
