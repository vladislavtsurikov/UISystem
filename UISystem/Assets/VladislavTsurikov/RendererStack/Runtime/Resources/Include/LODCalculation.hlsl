void CalculateLODFade(float cameraDistance, uint nextLOD, out float4 lodFade)
{
    if (isStandardRenderPipeline)
    {
        lodFade = float4(0, 0, 0, 0);
    }
    else
    {
        lodFade = float4(1, 0, 0, 0);
    }

    float nextLODDistance = lodDistances[nextLOD / 4][nextLOD % 4];
    float distance = nextLODDistance - cameraDistance;

    if (distance <= LODFadeDistance)
    {
        float fade = clamp(distance / LODFadeDistance, 0, 1);

        if (fade != 0)
        {
            float lodFadeQuantified = 1 - clamp(round(fade * 16) / 16, 0.0625, 1);
            lodFade = float4(fade, lodFadeQuantified, 0, 0);
        }
    }
}

void CalculateLOD(float distanceToCamera, float loadBias, out uint lod, out uint shadowLod, out float4 lodFade)
{
    lod = 9;
    shadowLod = 9;

    if (isStandardRenderPipeline)
    {
        lodFade = float4(0, 0, 0, 0);
    }
    else
    {
        lodFade = float4(1, 0, 0, 0);
    }

    for (uint i = startLOD; i < LODCount; i++)
    {
        bool lastLOD = i == LODCount - 1;

        float lodDistance = lodDistances[i / 4][i % 4] * loadBias;

        if (lastLOD || distanceToCamera <= lodDistance)
        {
            lod = i;
            shadowLod = shadowLODMap[i / 4][i % 4];

            if (i != LODCount - 1)
            {
                if (useLODFade)
                {
                    if (lodFadeForLastLOD)
                    {
                        if (i == LODCount - 2)
                        {
                            CalculateLODFade(distanceToCamera, i, lodFade);
                        }
                    }
                    else
                    {
                        CalculateLODFade(distanceToCamera, i, lodFade);
                    }
                }
            }

            break;
        }
    }
}

void CalculateLODWithoutLODFade(float distanceToCamera, out uint lod, out uint shadowLod)
{
    lod = 9;
    shadowLod = 9;

    for (uint i = startLOD; i < LODCount; i++)
    {
        uint nextLOD = i + 1;
        if (distanceToCamera <= lodDistances[nextLOD / 4][nextLOD % 4])
        {
            lod = i;
            shadowLod = shadowLODMap[i / 4][i % 4];

            break;
        }
    }
}
