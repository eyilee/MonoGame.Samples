using Microsoft.Xna.Framework;
using System;

namespace VoronoiDiagram;

public class Event (Vector2 site) : IComparable<Event>
{
    public Vector2 Site { get; } = site;

    public int CompareTo (Event? other)
    {
        if (other == null)
        {
            return 0;
        }

        int compareY = Site.Y.CompareTo (other.Site.Y);
        if (compareY != 0)
        {
            return compareY;
        }

        return Site.X.CompareTo (other.Site.X);
    }
}
