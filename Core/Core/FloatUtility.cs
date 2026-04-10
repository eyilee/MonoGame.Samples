namespace MonoGame.Samples.Library;

public static class FloatUtility
{
    public static float SmoothStep (this float t) => t * t * t * (t * (t * 6 - 15) + 10);

    public static float Bias (this float value, float bias)
    {
        if (bias == 0.5f)
        {
            return value;
        }

        return value / ((1f / bias - 2f) * (1f - value) + 1f);
    }

    public static float Gain (this float value, float gain)
    {
        if (value < 0.5f)
        {
            return (value * 2f).Bias (gain) / 2f;
        }
        else
        {
            return (value * 2f - 1f).Bias (1f - gain) / 2f + 0.5f;
        }
    }
}
