using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace VoronoiDiagram
{
    public class Polygon (Vector2 site, List<Vector2> points)
    {
        public Vector2 Site = site;
        public List<Vector2> Points = points;
    }
}
