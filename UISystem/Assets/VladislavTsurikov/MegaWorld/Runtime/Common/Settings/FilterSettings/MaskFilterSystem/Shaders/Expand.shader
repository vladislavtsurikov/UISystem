Shader "Hidden/MegaWorld/Expand"
{
    Properties
    {
        _MainTex ("Texture", any) = "" {}
    }

    SubShader
    {

        ZTest Always Cull Off ZWrite Off

        CGINCLUDE
        #include "UnityCG.cginc"
        #include "TerrainTool.cginc"
        #include "Includes/Common.cginc"

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;

        float KernelSize;
        float MaxKernelSize;

        static int KernelWeightCount = 7;
        static float KernelWeights[7] = {0.95f, 0.85f, 0.7f, 0.4f, 0.2f, 0.15f, 0.05f};


        struct appdata_t
        {
            float4 vertex : POSITION;
            float2 pcUV : TEXCOORD0;
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            float2 pcUV : TEXCOORD0;
        };

        v2f vert(appdata_t v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.pcUV = v.pcUV;
            return o;
        }

        float Smooth(float2 heightmapUV, float height, bool vertical)
        {
            float divisor = 1.0f;
            float offset = 0.0f;
            float h = height;

            for (int i = 0; i < KernelWeightCount; i++)
            {
                offset += _MainTex_TexelSize.x * KernelSize;
                divisor += 2.0f * KernelWeights[i];

                float2 UV1;
                float2 UV2;

                if (vertical)
                {
                    UV1 = heightmapUV + float2(0.0f, offset);
                    UV2 = heightmapUV - float2(0.0f, offset);
                }
                else
                {
                    UV1 = heightmapUV + float2(offset, 0.0f);
                    UV2 = heightmapUV - float2(offset, 0.0f);
                }

                h += KernelWeights[i] * UnpackHeightmap(tex2D(_MainTex, UV1));
                h += KernelWeights[i] * UnpackHeightmap(tex2D(_MainTex, UV2));
            }

            h /= divisor;

            float3 newHeight = float3(h, min(h, height), max(h, height));
            return newHeight;
        }
        ENDCG

        Pass
        {
            Name "Expand Horizontal"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment SmoothHorizontal

            float4 SmoothHorizontal(v2f i) : SV_Target
            {
                float2 heightmapUV = PaintContextUVToHeightmapUV(i.pcUV);

                float height = UnpackHeightmap(tex2D(_MainTex, heightmapUV));

                float newHeight = Smooth(heightmapUV, height, false);

                float inverseValue = InverseLerp(0, MaxKernelSize, KernelSize);

                float maxClamp = Lerp(1, 0.3f, inverseValue);

                newHeight = Remap(newHeight, 0, maxClamp);

                return newHeight;
            }
            ENDCG
        }
        Pass
        {
            Name "Expand Vertical"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment SmoothHorizontal

            float4 SmoothHorizontal(v2f i) : SV_Target
            {
                float2 heightmapUV = PaintContextUVToHeightmapUV(i.pcUV);

                float height = UnpackHeightmap(tex2D(_MainTex, heightmapUV));

                float newHeight = Smooth(heightmapUV, height, true);

                float inverseValue = InverseLerp(0, MaxKernelSize, KernelSize);

                float maxClamp = Lerp(1, 0.3f, inverseValue);

                newHeight = Remap(newHeight, 0, maxClamp);

                return newHeight;
            }
            ENDCG
        }
    }
    Fallback Off
}
