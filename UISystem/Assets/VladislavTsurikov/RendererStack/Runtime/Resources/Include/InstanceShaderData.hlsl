struct InstanceShaderData
{
    float4x4 positionMatrix;
    float4x4 inversePositionMatrix;
    float4 lodFade;

    //x y z w 

    float3 Position()
    {
        return positionMatrix._m03_m13_m23;
    }

    float3 Scale()
    {
        return positionMatrix._m00_m11_m22;
    }

    void AddFloatingOriginOffset(float3 offset)
    {
        positionMatrix._m03_m13_m23 += offset;
    }
};
