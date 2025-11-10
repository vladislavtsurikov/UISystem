Shader "Hidden/MegaWorld/ExpandDown"
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
        sampler2D _HeightmapTex;
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

        float GetHeight(float2 UV, int kernelWeightIndex, float terrainHeight)
        {
            float terrainHeightOffset = UnpackHeightmap(tex2D(_HeightmapTex, UV));

            if (terrainHeight > terrainHeightOffset)
            {
                return 0;
            }

            return KernelWeights[kernelWeightIndex] * UnpackHeightmap(tex2D(_MainTex, UV));
        }

        float Smooth(float2 heightmapUV, float height, bool vertical)
        {
            float offset = 0.0f;
            float summaryHeight = height;

            float heightTerrain = UnpackHeightmap(tex2D(_HeightmapTex, heightmapUV));

            for (int i = 0; i < KernelWeightCount; i++)
            {
                offset += _MainTex_TexelSize.x * KernelSize;

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

                summaryHeight += GetHeight(UV1, i, heightTerrain);
                summaryHeight += GetHeight(UV2, i, heightTerrain);
            }

            float3 newHeight = float3(summaryHeight, min(summaryHeight, height), max(summaryHeight, height));
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

                return newHeight;
            }
            ENDCG
        }
    }
    Fallback Off
}
