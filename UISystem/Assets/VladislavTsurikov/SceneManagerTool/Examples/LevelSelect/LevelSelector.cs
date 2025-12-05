using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.SceneManagerTool.Runtime;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;

namespace VladislavTsurikov.SceneManagerTool.Examples.LevelSelect
{
    public class LevelSelector : MonoBehaviour
    {
        public GameObject Prefab;

        private void Start()
        {
            IEnumerable<SceneCollection> search = Profile.Current.BuildSceneCollectionStack.ActiveBuildSceneCollection
                .GetAllSceneCollections()
                .Where(c => c.Name.Contains("Level"));

            foreach (SceneCollection item in search)
            {
                GameObject newButton = Instantiate(Prefab, transform);
                newButton.GetComponent<LevelSelectorButton>().ApplyData(item.Name, item);
            }
        }
    }
}
