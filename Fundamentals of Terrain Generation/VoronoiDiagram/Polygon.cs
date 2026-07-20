using Microsoft.Xna.Framework;
using MonoGame.Library.Graphics;
using System.Collections.Generic;

namespace VoronoiDiagram;

public class Polygon (List<Vector2> points)
{
    public List<Vector2> Points
    {
        get => _points;
        set
        {
            if (_points != value)
            {
                _points = value;
                _dirty = true;
            }
        }
    }

    private readonly List<LineSegment> _edges = [];

    private List<Vector2> _points = points;

    private bool _dirty = true;

    public void Draw (RenderManager render)
    {
        if (_dirty)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                Vector2 current = _points[i];
                Vector2 next = _points[(i + 1) % _points.Count];

                if (i >= _edges.Count)
                {
                    _edges.Add (new LineSegment () { Start = current, End = next, Color = Color.Blue });
                }
                else
                {
                    LineSegment edge = _edges[i];
                    edge.Start = current;
                    edge.End = next;
                }
            }

            _dirty = false;
        }

        for (int i = 0; i < _points.Count; i++)
        {
            if (i < _edges.Count)
            {
                _edges[i].Draw (render);
            }
        }
    }
}
