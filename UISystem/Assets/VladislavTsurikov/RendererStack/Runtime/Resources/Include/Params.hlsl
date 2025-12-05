#include "Common.hlsl"
#include "Random.hlsl"
#include "InstanceShaderData.hlsl"

uniform uint instanceCount;
uniform bool isFrustumCulling;
uniform bool isDistanceCulling;

uniform float4 cameraFrustumPlane0;
uniform float4 cameraFrustumPlane1;
uniform float4 cameraFrustumPlane2;
uniform float4 cameraFrustumPlane3;
uniform float4 cameraFrustumPlane4;
uniform float4 cameraFrustumPlane5;
uniform float fieldOfView;

uniform float4 worldSpaceCameraPos;

uniform float4 floatingOriginOffset;

uniform float boundingSphereRadius;
uniform float maxDistance;
uniform float minDistance;
uniform float distanceRandomOffset;

uniform bool useLODFade;
uniform bool lodFadeForLastLOD;
uniform float LODFadeDistance;
uniform float LODDistanceRandomOffset;
uniform bool isStandardRenderPipeline;

uniform int getAdditionalShadow;
uniform float shadowDistance;
uniform float minCullingDistanceForAdditionalShadow;
uniform float increaseBoundingSphereForShadows;

uniform float3 directionLight;
uniform float3 boundsSize;

uniform uint LODCount;
uniform uint startLOD;
uniform float4x4 lodDistances;
uniform float4x4 shadowLODMap;

uniform float time;

StructuredBuffer<InstanceShaderData> positions;
AppendStructuredBuffer<InstanceShaderData> positionLOD0;
AppendStructuredBuffer<InstanceShaderData> positionLOD1;
AppendStructuredBuffer<InstanceShaderData> positionLOD2;
AppendStructuredBuffer<InstanceShaderData> positionLOD3;

AppendStructuredBuffer<InstanceShaderData> positionShadowLOD0;
AppendStructuredBuffer<InstanceShaderData> positionShadowLOD1;
AppendStructuredBuffer<InstanceShaderData> positionShadowLOD2;
AppendStructuredBuffer<InstanceShaderData> positionShadowLOD3;

float GetDistanceToCamera(float3 itemPos)
{
    return distance(itemPos, worldSpaceCameraPos.xyz);
}

float GetModifiedDistanceToCamera(float3 instancePosition, float maxScaleValue, float random)
{
    const float offsetDistanceToCamera = GetRandomLerp(LODDistanceRandomOffset, random);

    float modifiedDistanceToCamera = GetDistanceToCamera(instancePosition) + offsetDistanceToCamera;

    const float fieldOfViewFactor = 90 / fieldOfView;

    modifiedDistanceToCamera /= fieldOfViewFactor;

    modifiedDistanceToCamera /= maxScaleValue;

    return modifiedDistanceToCamera;
}

void AddPosition(InstanceShaderData instanceData, uint lodIndex, float4 lodFade)
{
    instanceData.lodFade = lodFade;

    const uint correctIndex = LODCount > 4 ? lodIndex - startLOD : lodIndex;

    switch (correctIndex)
    {
    case 0:
        {
            positionLOD0.Append(instanceData);

            break;
        }
    case 1:
        {
            positionLOD1.Append(instanceData);

            break;
        }
    case 2:
        {
            positionLOD2.Append(instanceData);

            break;
        }
    case 3:
        {
            positionLOD3.Append(instanceData);

            break;
        }
    }
}

void AddPositionShadow(InstanceShaderData instanceData, float distanceToCamera, uint lodIndex, float4 lodFade)
{
    if (distanceToCamera > shadowDistance)
    {
        return;
    }

    instanceData.lodFade = lodFade;

    const uint correctIndex = LODCount > 4 ? lodIndex - startLOD : lodIndex;

    switch (correctIndex)
    {
    case 0:
        {
            positionShadowLOD0.Append(instanceData);
            break;
        }
    case 1:
        {
            positionShadowLOD1.Append(instanceData);
            break;
        }
    case 2:
        {
            positionShadowLOD2.Append(instanceData);
            break;
        }
    case 3:
        {
            positionShadowLOD3.Append(instanceData);
            break;
        }
    }
}
