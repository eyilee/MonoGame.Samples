using System;
using MonoGame.Samples.Library;

namespace MonoGame.Samples.PerlinNoiseBiome;

public class PerlinNoise
{
    private readonly int[] _permutation;

    public PerlinNoise (int seed)
    {
        Random random = new (seed);

        int[] permutation = new int[256];

        for (int i = 0; i < permutation.Length; i++)
        {
            permutation[i] = i;
        }

        for (int i = permutation.Length - 1; i > 0; i--)
        {
            int swapIndex = random.Next (i + 1);
            (permutation[i], permutation[swapIndex]) = (permutation[swapIndex], permutation[i]);
        }

        _permutation = [.. permutation, .. permutation];
    }

    public float Noise (float x, float y)
    {
        int indexX = (int)MathF.Floor (x) & 255;
        int indexY = (int)MathF.Floor (y) & 255;

        int aa = _permutation[_permutation[indexX] + indexY];
        int ab = _permutation[_permutation[indexX] + indexY + 1];
        int ba = _permutation[_permutation[indexX + 1] + indexY];
        int bb = _permutation[_permutation[indexX + 1] + indexY + 1];

        float relativeX = x - MathF.Floor (x);
        float relativeY = y - MathF.Floor (y);

        float smoothX = relativeX.SmoothStep ();
        float smoothY = relativeY.SmoothStep ();

        return (float.Lerp (float.Lerp (Gradient (aa, relativeX, relativeY), Gradient (ba, relativeX - 1, relativeY), smoothX),
            float.Lerp (Gradient (ab, relativeX, relativeY - 1), Gradient (bb, relativeX - 1, relativeY - 1), smoothX),
            smoothY) + 1) / 2f;
    }

    public static float SmoothStep (float t) => t * t * t * (t * (t * 6 - 15) + 10);

    public static float Gradient (int hash, float x, float y) => (hash & 7) switch
    {
        0 => x + y,
        1 => -x + y,
        2 => x - y,
        3 => -x - y,
        4 => x,
        5 => -x,
        6 => y,
        7 => -y,
        _ => 0,
    };

    public float FractalBrownianMotionNoise (float x, float y, int octaves)
    {
        if (octaves <= 0)
        {
            throw new ArgumentOutOfRangeException (nameof (octaves), "Octaves must be greater than zero.");
        }

        float value = 0;
        float maxValue = 0;

        float frequency = 1;
        float amplitude = 1;

        for (int i = 0; i < octaves; i++)
        {
            value += Noise (x * frequency, y * frequency) * amplitude;
            maxValue += amplitude;

            frequency *= 1.6f;
            amplitude *= 0.625f;
        }

        return value / maxValue;
    }

    public float DomainWarpedNoise (float x, float y, int octaves, float warpFrequency, float warpAmplitude, int warpOctaves)
    {
        float warpX = x + FractalBrownianMotionNoise (x * warpFrequency + 13, y * warpFrequency + 19, warpOctaves) * warpAmplitude;
        float warpY = y + FractalBrownianMotionNoise (x * warpFrequency + 23, y * warpFrequency + 29, warpOctaves) * warpAmplitude;

        return FractalBrownianMotionNoise (warpX, warpY, octaves);
    }
}