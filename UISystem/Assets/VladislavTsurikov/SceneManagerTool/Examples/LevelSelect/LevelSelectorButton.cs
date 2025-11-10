using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;

namespace VladislavTsurikov.SceneManagerTool.Examples.LevelSelect
{
    public class LevelSelectorButton : MonoBehaviour
    {
        public Text Text;
        public SceneCollection Collection;

        public void OpenLevel() => Collection.Load().Forget();

        public void ApplyData(string text, SceneCollection collection)
        {
            Text.text = text;
            Collection = collection;
        }
    }
}
