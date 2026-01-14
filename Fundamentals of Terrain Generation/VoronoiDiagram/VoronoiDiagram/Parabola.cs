using Microsoft.Xna.Framework;
using System;

namespace MonoGame.Samples.VoronoiDiagram;

public class Parabola (Vector2 focus)
{
    public Vector2 Focus = focus;
    public Parabola? LeftParabola;
    public Parabola? RightParabola;
    public Edge? LeftEdge;
    public Edge? RightEdge;
    public Event? CircleEvent;

    public static float GetFocalLength (Vector2 focus, float directrixY)
    {
        return (focus.Y - directrixY) / 2f;
    }

    public static Vector2 GetVertex (Vector2 focus, float directrixY)
    {
        return new Vector2 (focus.X, focus.Y + GetFocalLength (focus, directrixY));
    }

    public static float GetY (Vector2 focus, float directrixY, float x)
    {
        if (focus.Y == directrixY)
        {
            if (focus.X == x)
            {
                return focus.Y;
            }
        }

        return ((x - focus.X) * (x - focus.X) / (4f * GetFocalLength (focus, directrixY))) + (focus.Y + directrixY) / 2f;
    }

    public static Vector2[] GetIntersectPoints (Vector2 focus1, Vector2 focus2, float directrixY)
    {
        if (focus1.Y == focus2.Y)
        {
            float x = (focus1.X + focus2.X) / 2f;
            float y = GetY (focus1, directrixY, x);
            return [new (x, y)];
        }

        float focalLength1 = GetFocalLength (focus1, directrixY);
        if (focalLength1 <= 0.0001f)
        {
            return [new (focus1.X, GetY (focus2, directrixY, focus1.X))];
        }

        float focalLength2 = GetFocalLength (focus2, directrixY);
        if (focalLength2 <= 0.0001f)
        {
            return [new (focus2.X, GetY (focus1, directrixY, focus2.X))];
        }

        float a = (1f / (4f * focalLength1)) - (1f / (4f * focalLength2));
        float b = (-focus1.X / (2f * focalLength1)) + (focus2.X / (2f * focalLength2));
        float c = (focus1.X * focus1.X / (4f * focalLength1)) - (focus2.X * focus2.X / (4f * focalLength2)) + (focus1.Y + directrixY) / 2f - (focus2.Y + directrixY) / 2f;

        float discriminant = (b * b) - (4 * a * c);
        if (discriminant < 0)
        {
            return [];
        }

        float sqrtDiscriminant = MathF.Sqrt (discriminant);
        float x1 = (-b + sqrtDiscriminant) / (2f * a);
        float x2 = (-b - sqrtDiscriminant) / (2f * a);
        return [new (x1, GetY (focus1, directrixY, x1)), new (x2, GetY (focus1, directrixY, x2))];
    }
}
