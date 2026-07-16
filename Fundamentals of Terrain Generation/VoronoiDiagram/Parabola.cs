using Microsoft.Xna.Framework;
using MonoGame.Library.Graphics;

namespace VoronoiDiagram;

public class Parabola
{
    private readonly SdfParabola _shape = new ();

    public Vector2 Position
    {
        get => _shape.Position;
        set => _shape.Position = value;
    }

    public Vector2 Scale
    {
        get => _shape.Scale;
        set => _shape.Scale = value;
    }

    public Vector2 Focus
    {
        get => _shape.Focus;
        set => _shape.Focus = value;
    }

    public Vector2 Vertex
    {
        get => _shape.Vertex;
        set => _shape.Vertex = value;
    }

    public Parabola? LeftParabola;

    public Parabola? RightParabola;

    public Edge? LeftEdge;

    public Edge? RightEdge;

    public Event? CircleEvent;

    public Parabola (Vector2 focus)
    {
        _shape.Focus = focus;
        _shape.Vertex = focus;
    }

    public void Draw (RenderManager render)
    {
        if (Focus == Vertex)
        {
            return;
        }

        _shape.Draw (render);
    }

    public static float GetFocalLength (Vector2 focus, float directrixY)
    {
        return float.Abs (focus.Y - directrixY) / 2f;
    }

    public static Vector2 GetVertex (Vector2 focus, float directrixY)
    {
        return new Vector2 (focus.X, (focus.Y + directrixY) * 0.5f);
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

        float p = (focus.Y - directrixY) * 0.5f;

        return (x - focus.X) * (x - focus.X) / (4f * p) + (focus.Y + directrixY) * 0.5f;
    }

    public static Vector2[] GetIntersectPoints (Vector2 focus1, Vector2 focus2, float directrixY)
    {
        if (focus1.Y == focus2.Y)
        {
            float x = (focus1.X + focus2.X) / 2f;
            float y = GetY (focus1, directrixY, x);

            return [new (x, y)];
        }

        float p1 = (focus1.Y - directrixY) * 0.5f;

        if (float.Abs (p1) <= 0.0001f)
        {
            return [new (focus1.X, GetY (focus2, directrixY, focus1.X))];
        }

        float p2 = (focus2.Y - directrixY) * 0.5f;

        if (float.Abs (p2) <= 0.0001f)
        {
            return [new (focus2.X, GetY (focus1, directrixY, focus2.X))];
        }

        float a = 1f / (4f * p1) - 1f / (4f * p2);
        float b = -focus1.X / (2f * p1) + focus2.X / (2f * p2);
        float c = focus1.X * focus1.X / (4f * p1) - focus2.X * focus2.X / (4f * p2) + (focus1.Y + directrixY) / 2f - (focus2.Y + directrixY) / 2f;
        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
        {
            return [];
        }

        float sqrtDiscriminant = float.Sqrt (discriminant);
        float x1 = (-b + sqrtDiscriminant) / (2f * a);
        float x2 = (-b - sqrtDiscriminant) / (2f * a);

        return [new (x1, GetY (focus1, directrixY, x1)), new (x2, GetY (focus1, directrixY, x2))];
    }
}
