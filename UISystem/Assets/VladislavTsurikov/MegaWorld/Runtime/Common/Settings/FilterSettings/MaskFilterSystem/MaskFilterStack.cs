using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    public enum BlendMode
    {
        Multiply,
        Add
    }

    [Serializable]
    public class MaskFilterStack : ComponentStackSupportSameType<MaskFilter>
    {
        public void Eval(MaskFilterContext maskFilterContext)
        {
            var count = _elementList.Count;

            RenderTexture prev = RenderTexture.active;

            var rts = new RenderTexture[2];

            var srcIndex = 0;
            var destIndex = 1;

            rts[0] = RenderTexture.GetTemporary(maskFilterContext.DestinationRenderTexture.descriptor);
            rts[1] = RenderTexture.GetTemporary(maskFilterContext.DestinationRenderTexture.descriptor);

            rts[0].enableRandomWrite = true;
            rts[1].enableRandomWrite = true;

            Graphics.Blit(Texture2D.whiteTexture, rts[0]);
            Graphics.Blit(Texture2D.blackTexture,
                rts[1]); //don't remove this! needed for compute shaders to work correctly.

            for (var i = 0; i < count; ++i)
            {
                if (_elementList[i].Active)
                {
                    maskFilterContext.SourceRenderTexture = rts[srcIndex];
                    maskFilterContext.DestinationRenderTexture = rts[destIndex];

                    _elementList[i].Eval(maskFilterContext, i);

                    destIndex += srcIndex;
                    srcIndex = destIndex - srcIndex;
                    destIndex -= srcIndex;
                }
            }

            Graphics.Blit(rts[srcIndex], maskFilterContext.Output);

            RenderTexture.ReleaseTemporary(rts[0]);
            RenderTexture.ReleaseTemporary(rts[1]);

            RenderTexture.active = prev;
        }
    }
}
