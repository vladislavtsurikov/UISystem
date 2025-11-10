using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize
{
    public partial class Sectorize
    {
        private AsynchronousLoading _asynchronousLoading;
        private Caching _caching;
        private CameraManager _cameraManager;
        private ImmediatelyLoading _immediatelyLoading;
        private PreventingUnloading _preventingUnloading;
        private StreamingRules _streamingRules;

        public CameraManager CameraManager =>
            _cameraManager ??=
                (CameraManager)RendererStackManager.Instance.SceneComponentStack.GetElement(typeof(CameraManager));

        public StreamingRules StreamingRules =>
            _streamingRules ??=
                (StreamingRules)Core.GlobalSettings.GlobalSettings.Instance.GetElement(typeof(StreamingRules),
                    GetType());

        public ImmediatelyLoading ImmediatelyLoading =>
            _immediatelyLoading ??=
                StreamingRules.StreamingRuleComponentStack.GetElement<ImmediatelyLoading>();

        public AsynchronousLoading AsynchronousLoading =>
            _asynchronousLoading ??=
                StreamingRules.StreamingRuleComponentStack.GetAndAutoUpdateComponent<AsynchronousLoading>(
                    component => _asynchronousLoading = component);

        public PreventingUnloading PreventingUnloading =>
            _preventingUnloading ??=
                StreamingRules.StreamingRuleComponentStack.GetAndAutoUpdateComponent<PreventingUnloading>(
                    component => _preventingUnloading = component);

        public Caching Caching =>
            _caching ??=
                StreamingRules.StreamingRuleComponentStack.GetAndAutoUpdateComponent<Caching>(component =>
                    _caching = component);
    }
}
