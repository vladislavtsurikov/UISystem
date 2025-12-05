struct Ray
{
    float3 origin;
    float3 direction;
};

Ray CreateRay(float3 origin, float3 direction)
{
    Ray newRay;
    newRay.origin = origin;
    newRay.direction = direction;
    return newRay;
}

struct Bounds
{
    float3 center;
    float3 extents;

    float3 Min()
    {
        return center - extents;
    }

    float3 Max()
    {
        return center + extents;
    }

    void SetMinMax(float3 min, float3 max)
    {
        extents = (max - min) * 0.5f;
        center = min + extents;
    }

    void Encapsulate(float3 targetPoint)
    {
        SetMinMax(min(Min(), targetPoint), max(Max(), targetPoint));
    }
};

bool IntersectPlane(Ray ray, float3 planeOrigin, out float3 hitPoint)
{
    float3 planeNormal = -float3(0, 1, 0);
    float denominator = dot(ray.direction, planeNormal);
    if (denominator > 0.00001f)
    {
        float t = dot(planeOrigin - ray.origin, planeNormal) / denominator;
        hitPoint = ray.origin + ray.direction * t;
        return true;
    }

    hitPoint = float3(0, 0, 0);
    return false;
}

Bounds GetShadowBounds(Bounds objectBounds, float3 lightDirection, float3 planeOrigin, out bool hitPlane)
{
    Ray p0 = CreateRay(float3(objectBounds.Min().x, objectBounds.Max().y, objectBounds.Min().z), lightDirection);
    Ray p1 = CreateRay(float3(objectBounds.Min().x, objectBounds.Max().y, objectBounds.Max().z), lightDirection);
    Ray p2 = CreateRay(float3(objectBounds.Max().x, objectBounds.Max().y, objectBounds.Min().z), lightDirection);
    Ray p3 = CreateRay(objectBounds.Max(), lightDirection);

    float3 hitPoint;
    hitPlane = false;

    if (IntersectPlane(p0, planeOrigin, hitPoint))
    {
        objectBounds.Encapsulate(hitPoint);
        hitPlane = true;
    }

    if (IntersectPlane(p1, planeOrigin, hitPoint))
    {
        objectBounds.Encapsulate(hitPoint);
        hitPlane = true;
    }

    if (IntersectPlane(p2, planeOrigin, hitPoint))
    {
        objectBounds.Encapsulate(hitPoint);
        hitPlane = true;
    }

    if (IntersectPlane(p3, planeOrigin, hitPoint))
    {
        objectBounds.Encapsulate(hitPoint);
        hitPlane = true;
    }

    return objectBounds;
}

bool TestPlaneIntersection(Bounds bounds, float4 plane)
{
    float3 center = bounds.center;
    float3 extents = bounds.extents;

    float3 planeNormal = plane.xyz;
    float planeDistance = plane.w;

    float3 absNormal = float3(abs(planeNormal.x), abs(planeNormal.y), abs(planeNormal.z));
    float r = extents.x * absNormal.x + extents.y * absNormal.y + extents.z * absNormal.z;
    float s = planeNormal.x * center.x + planeNormal.y * center.y + planeNormal.z * center.z;
    if (s + r < -planeDistance)
    {
        return false;
    }
    return true;
}

bool BoundsIntersectsFrustum(Bounds bounds, float4 cameraFrustumPlane0, float4 cameraFrustumPlane1,
                             float4 cameraFrustumPlane2, float4 cameraFrustumPlane3, float4 cameraFrustumPlane4,
                             float4 cameraFrustumPlane5)
{
    if (TestPlaneIntersection(bounds, cameraFrustumPlane0) == false)
    {
        return false;
    }
    if (TestPlaneIntersection(bounds, cameraFrustumPlane1) == false)
    {
        return false;
    }
    if (TestPlaneIntersection(bounds, cameraFrustumPlane2) == false)
    {
        return false;
    }
    if (TestPlaneIntersection(bounds, cameraFrustumPlane3) == false)
    {
        return false;
    }
    if (TestPlaneIntersection(bounds, cameraFrustumPlane4) == false)
    {
        return false;
    }
    if (TestPlaneIntersection(bounds, cameraFrustumPlane5) == false)
    {
        return false;
    }

    return true;
}

bool IsShadowVisible(Bounds objectBounds, float3 lightDirection, float3 planeOrigin, float4 cameraFrustumPlane0,
                     float4 cameraFrustumPlane1,
                     float4 cameraFrustumPlane2, float4 cameraFrustumPlane3, float4 cameraFrustumPlane4,
                     float4 cameraFrustumPlane5)
{
    bool hitPlane;
    Bounds shadowBounds = GetShadowBounds(objectBounds, lightDirection, planeOrigin, hitPlane);
    return hitPlane && BoundsIntersectsFrustum(shadowBounds, cameraFrustumPlane0, cameraFrustumPlane1,
                                               cameraFrustumPlane2, cameraFrustumPlane3, cameraFrustumPlane4,
                                               cameraFrustumPlane5);
}

bool AddAdditionalShadow(float distanceToCamera, float3 position)
{
    if (distanceToCamera > shadowDistance)
    {
        return false;
    }

    switch (getAdditionalShadow)
    {
    case 1: //GetAdditionalShadow.MinCullingDistance:
        {
            if (distanceToCamera <= minCullingDistanceForAdditionalShadow)
            {
                return true;
            }

            break;
        }
    case 2: //GetAdditionalShadow.IncreaseBoundingSphere:
        {
            if (FrustumCulling(position, cameraFrustumPlane0, cameraFrustumPlane1, cameraFrustumPlane2,
                               cameraFrustumPlane3, cameraFrustumPlane4, cameraFrustumPlane5, boundingSphereRadius,
                               increaseBoundingSphereForShadows) == false)
            {
                return true;
            }

            break;
        }
    case 3: //DirectionLightShadowVisible
        {
            Bounds bounds;
            bounds.center = position;
            bounds.extents = boundsSize / 2;

            float3 planeOrigin = float3(0, position.y - boundsSize.y, 0);

            if (IsShadowVisible(bounds, directionLight, planeOrigin, cameraFrustumPlane0, cameraFrustumPlane1,
                                cameraFrustumPlane2,
                                cameraFrustumPlane3, cameraFrustumPlane4, cameraFrustumPlane5))
            {
                return true;
            }

            break;
        }
    }

    return false;
}
