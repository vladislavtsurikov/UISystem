#define M1 1597334677U     //1719413*929
#define M2 3812015801U     //140473*2467*11

float RandomValueI(uint2 pos)
{
    pos *= uint2(M1, M2);
    const uint n = (pos.x ^ pos.y) * M1;
    return float(n) * (1.0 / float(0xffffffffU));
}

float SmoothStep(const float t, const float max)
{
    const float f = t / max;
    return f * f * f * (max - t * 0.5) * 2.0;
}

float GetRandomLerp(float value, float random)
{
    return value * SmoothStep(random, 1.0);
}
