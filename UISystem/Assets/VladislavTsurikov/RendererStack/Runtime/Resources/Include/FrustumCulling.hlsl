bool FrustumCulling(float3 position, float4 cameraFrustumPlane0, float4 cameraFrustumPlane1,
                    float4 cameraFrustumPlane2, float4 cameraFrustumPlane3, float4 cameraFrustumPlane4,
                    float4 cameraFrustumPlane5,
                    float boundingSphereRadius, float increaseBounding = 0)
{
    if (!isFrustumCulling)
    {
        return true;
    }

    float4 cameraDistances0 = float4(
        dot(cameraFrustumPlane0.xyz, position) + cameraFrustumPlane0.w,
        dot(cameraFrustumPlane1.xyz, position) + cameraFrustumPlane1.w,
        dot(cameraFrustumPlane2.xyz, position) + cameraFrustumPlane2.w,
        dot(cameraFrustumPlane3.xyz, position) + cameraFrustumPlane3.w
    );

    float4 cameraDistances1 = float4(
        dot(cameraFrustumPlane4.xyz, position) + cameraFrustumPlane4.w,
        dot(cameraFrustumPlane5.xyz, position) + cameraFrustumPlane5.w,
        0.0f,
        0.0f
    );

    if (!(all(cameraDistances0 >= -(boundingSphereRadius + increaseBounding))
        && all(cameraDistances1 >= -(boundingSphereRadius + increaseBounding))))
    {
        return true;
    }
    return false;
}
