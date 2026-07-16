using Microsoft.Xna.Framework;

namespace VoronoiDiagram;

public class CircleEvent (Vector2 site, Parabola? parabola, Vector2 vertex) : Event (site)
{
    public Parabola? Parabola { get; } = parabola;

    public Vector2 Vertex { get; } = vertex;
}
