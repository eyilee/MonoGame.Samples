using Microsoft.Xna.Framework;
using System;

namespace MonoGame.Samples.Library;

public static class ColorUtility
{
    public static Color HSVToRGB (float h, float s, float v)
    {
        if (s == 0f)
        {
            return new Color (v, v, v);
        }
        else if (v == 0f)
        {
            return Color.Black;
        }

        h *= 6f;

        int i = (int)float.Floor (h);

        float f = h - i;
        float p = v * (1f - s);
        float q = v * (1f - (f * s));
        float t = v * (1f - ((1f - f) * s));

        float r = 0f;
        float g = 0f;
        float b = 0f;

        switch (i % 6)
        {
            case 0:
                r = v;
                g = t;
                b = p;
                break;
            case 1:
                r = q;
                g = v;
                b = p;
                break;
            case 2:
                r = p;
                g = v;
                b = t;
                break;
            case 3:
                r = p;
                g = q;
                b = v;
                break;
            case 4:
                r = t;
                g = p;
                b = v;
                break;
            case 5:
                r = v;
                g = p;
                b = q;
                break;
        }

        return new Color (r, g, b);
    }

    public static void RGBToHSV (float r, float g, float b, out float h, out float s, out float v)
    {
        float max = MathF.Max (r, MathF.Max (g, b));
        float min = MathF.Min (r, MathF.Min (g, b));
        float delta = max - min;

        v = max;

        if (max == 0f)
        {
            s = 0f;
            h = 0f;
            return;
        }
        else
        {
            s = delta / max;
        }

        if (delta == 0f)
        {
            h = 0f;
        }
        else if (max == r)
        {
            h = (g - b) / delta;
        }
        else if (max == g)
        {
            h = 2f + (b - r) / delta;
        }
        else
        {
            h = 4f + (r - g) / delta;
        }

        h /= 6f;

        if (h < 0f)
        {
            h += 1f;
        }
    }

    public static void RGBToHSV (Color color, out float h, out float s, out float v)
    {
        RGBToHSV (color.R / 255f, color.G / 255f, color.B / 255f, out h, out s, out v);
    }

    public static Color LerpHSV (Color lhs, Color rhs, float t)
    {
        RGBToHSV (lhs, out float h1, out float s1, out float v1);
        RGBToHSV (rhs, out float h2, out float s2, out float v2);

        float dh = h2 - h1;

        if (dh > 0.5f)
        {
            h1 += 1f;
        }
        else if (dh < -0.5f)
        {
            h2 += 1f;
        }

        float h = float.Lerp (h1, h2, t) % 1f;
        float s = float.Lerp (s1, s2, t);
        float v = float.Lerp (v1, v2, t);

        return HSVToRGB (h, s, v);
    }
}
