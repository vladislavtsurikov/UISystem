using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings
{
    [Name("LOD Group")]
    [PersistentComponent]
    public class LODGroup : PrototypeComponent
    {
        [SerializeField]
        private bool _enabledLODFade;

        public float LODBias = 1;

        public float LODDistanceRandomOffset;
        public bool LodFadeForLastLOD = true;
        public float LodFadeTransitionDistance = 10;

        public bool EnabledLODFade => _enabledLODFade;


        public void SetLODFade(RenderModel renderModel, bool value)
        {
            if (value != _enabledLODFade)
            {
                renderModel.SetLODFadeKeyword(value);
                _enabledLODFade = value;
            }
        }

        public override bool ShowActiveToggle() => false;
    }
}
