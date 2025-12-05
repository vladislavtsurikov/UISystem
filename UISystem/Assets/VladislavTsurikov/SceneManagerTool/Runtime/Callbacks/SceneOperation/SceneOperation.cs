using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Callbacks.SceneOperation
{
    public abstract class SceneOperation : MonoBehaviour
    {
        public abstract UniTask OnLoad();
        public abstract UniTask OnUnload();
    }
}
