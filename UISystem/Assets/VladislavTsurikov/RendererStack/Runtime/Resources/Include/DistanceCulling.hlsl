float GetRandomDistanceToCamera(InstanceShaderData instanceData, float random)
{
    float offsetDistanceToCamera = GetRandomLerp(distanceRandomOffset, random);

    return GetDistanceToCamera(instanceData.Position()) + offsetDistanceToCamera;
}

bool Culling(float distanceToCamera, float minDistance, float maxDistance)
{
    if (distanceToCamera > maxDistance || distanceToCamera < minDistance)
    {
        return true;
    }

    return false;
}

bool DistanceCulling(InstanceShaderData instanceData, float maxScaleValue, float random)
{
    int minDistanceCulling = minDistance;

    if (startLOD != 0)
    {
        minDistanceCulling *= maxScaleValue;
    }

    return Culling(GetRandomDistanceToCamera(instanceData, random), minDistanceCulling, maxDistance);
}
