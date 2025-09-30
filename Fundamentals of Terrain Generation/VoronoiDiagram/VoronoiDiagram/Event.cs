using Microsoft.Xna.Framework;

namespace VoronoiDiagram
{
    public class Event (Vector2 site, bool isSiteEvent)
    {
        public Vector2 Site = site;
        public bool IsSiteEvent = isSiteEvent;
        public Parabola Parabola;
        public Vector2 VertexPoint;
    }
}
