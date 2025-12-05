using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Callbacks
{
    [RequireComponent(typeof(Button))]
    public class OpenSceneCollectionOnButtonClick : MonoBehaviour
    {
        public SceneCollectionReference SceneCollectionReference = new();

        private void Start()
        {
            Button button = GetComponent<Button>();

            button.onClick.AddListener(Load);
        }

        public void Load() => SceneManagerAPI.LoadSceneCollection(SceneCollectionReference.SceneCollection);
    }
}
