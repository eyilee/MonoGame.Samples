using Microsoft.Xna.Framework;

namespace MonoGame.Samples.Library;

public static class Vector2Utility
{
    public static Vector2 Rotate (this Vector2 v, float angle)
    {
        float cos = float.Cos (angle);
        float sin = float.Sin (angle);
        return new Vector2 ((v.X * cos) - (v.Y * sin), (v.X * sin) + (v.Y * cos));
    }

    public static Vector2 Perpendicular (this Vector2 v)
    {
        return new Vector2 (-v.Y, v.X);
    }

    public static float Dot (this Vector2 v1, Vector2 v2)
    {
        return (v1.X * v2.X) + (v1.Y * v2.Y);
    }

    public static float Cross (this Vector2 v1, Vector2 v2)
    {
        return (v1.X * v2.Y) - (v1.Y * v2.X);
    }
}
