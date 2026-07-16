using Microsoft.Xna.Framework;
using MonoGame.Library.Graphics;

namespace VoronoiDiagram;

public class Site
{
    private readonly SdfCircle _shape = new ();

    public Vector2 Point
    {
        get => _shape.Position;
        set => _shape.Position = value;
    }

    public Color Color
    {
        get => _shape.Color;
        set => _shape.Color = value;
    }

    public float Radius
    {
        get => _shape.Radius;
        set => _shape.Radius = value;
    }

    public void Draw (RenderManager render) => _shape.Draw (render);
}
