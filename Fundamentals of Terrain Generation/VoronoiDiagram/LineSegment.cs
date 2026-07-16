using Microsoft.Xna.Framework;
using MonoGame.Library.Graphics;

namespace VoronoiDiagram;

public class LineSegment
{
    private readonly SdfLine _shape = new ();

    public Vector2 Start
    {
        get => _shape.Start;
        set => _shape.Start = value;
    }

    public Vector2 End
    {
        get => _shape.End;
        set => _shape.End = value;
    }

    public Vector2 Direction => End - Start;

    public void Draw (RenderManager render) => _shape.Draw (render);
}
