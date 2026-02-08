using Microsoft.Xna.Framework;
using System;

namespace MonoGame.Samples.Library;

public class ColorUtility
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

        int i = (int)MathF.Floor (h / 60f);

        float f = (h / 60f) - i;
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
}
