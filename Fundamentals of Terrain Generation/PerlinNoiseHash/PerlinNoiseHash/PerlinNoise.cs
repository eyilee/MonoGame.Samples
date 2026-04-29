using System;
using MonoGame.Samples.Library;

namespace MonoGame.Samples.PerlinNoiseHash;

public class PerlinNoise
{
    private static readonly float GradientScale = 1f / float.Sqrt (2);

    private readonly float[] _permutation;

    public PerlinNoise (int seed)
    {
        Random random = new (seed);

        float[] permutation = new float[512];

        for (int i = 0; i < 256; i++)
        {
            int index = i * 2;
            float radian = 360f * (index / 256f) * (float.Pi / 180f);
            permutation[index] = float.Cos (radian);
            permutation[index + 1] = float.Sin (radian);
        }

        for (int i = 255; i > 0; i--)
        {
            int fromIndex = i * 2;
            int swapIndex = random.Next (i + 1) * 2;
            (permutation[fromIndex], permutation[swapIndex]) = (permutation[swapIndex], permutation[fromIndex]);
            (permutation[fromIndex + 1], permutation[swapIndex + 1]) = (permutation[swapIndex + 1], permutation[fromIndex + 1]);
        }

        _permutation = permutation;
    }

    private static uint LowBias32 (uint x)
    {
        x ^= x >> 16;
        x *= 0x21f0aaadu;
        x ^= x >> 15;
        x *= 0x735a2d97u;
        x ^= x >> 16;
        return x;
    }

    private float Gradient (float x, float y, int ix, int iy, int seed)
    {
        uint hashIndex = LowBias32 ((uint)(ix * 777391u + iy * 475243u + seed * 899069u));
        hashIndex ^= hashIndex >> 8;
        hashIndex &= 0xFFu;
        hashIndex <<= 1;

        float gradientX = _permutation[hashIndex];
        float gradientY = _permutation[hashIndex + 1];
        float relativeX = x - ix;
        float relativeY = y - iy;

        return (gradientX * relativeX + gradientY * relativeY) * GradientScale;
    }

    public float Noise (float x, float y, int seed)
    {
        int ix = (int)float.Floor (x);
        int iy = (int)float.Floor (y);

        float aa = Gradient (x, y, ix, iy, seed);
        float ab = Gradient (x, y, ix, iy + 1, seed);
        float ba = Gradient (x, y, ix + 1, iy, seed);
        float bb = Gradient (x, y, ix + 1, iy + 1, seed);

        float relativeX = x - ix;
        float relativeY = y - iy;

        float smoothX = relativeX.SmoothStep ();
        float smoothY = relativeY.SmoothStep ();

        return (float.Lerp (float.Lerp (aa, ba, smoothX), float.Lerp (ab, bb, smoothX), smoothY) + 1f) * 0.5f;
    }
}