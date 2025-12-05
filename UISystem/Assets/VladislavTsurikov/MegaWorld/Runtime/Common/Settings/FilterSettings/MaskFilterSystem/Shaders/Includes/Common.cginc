float Clamp01(float value)
{
    if (value < 0.0)
        return 0.0f;
    return value > 1.0 ? 1 : value;
}

float InverseLerp(float a, float b, float t)
{
    return (t - a) / (b - a);
}

float Lerp(float a, float b, float t)
{
    return a + (b - a) * Clamp01(t);
}

float Remap(float height, float remapMin, float remapMax)
{
    float value;

    if (height < remapMin)
    {
        value = 0;
    }
    else if (height > remapMax)
    {
        value = 1;
    }
    else
    {
        value = InverseLerp(remapMin, remapMax, height);
    }

    return float4(value, value, value, 1);
}
