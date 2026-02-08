using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Samples.Library;
using MonoGame.Samples.Library.SDF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Samples.VoronoiDiagram;

public class VoronoiDiagram
{
    private static readonly Random s_random = new ();

    private readonly SDFBatch _sdfBatch;
    private readonly int _size;
    private readonly int _pointCount;
    private Vector2 _min;
    private Vector2 _max;

    private readonly List<Vector2> _points = [];
    private readonly List<Event> _events = [];
    private readonly List<Parabola> _beachline = [];
    private readonly List<Edge> _edges = [];
    private readonly List<Vector2> _vertices = [];
    private readonly List<Polygon> _polygons = [];
    private float _sweeplineY;

    public enum EStepState
    {
        Fortune,
        Polygon,
        Finished,
    }

    private EStepState _state;

    public VoronoiDiagram (GraphicsDevice graphicsDevice, int size, int pointCount)
    {
        if (size <= 0)
        {
            throw new ArgumentException ("Size must be greater than 0", nameof (size));
        }

        Camera.Main.LookAt (new Vector2 (size / 2f));

        _sdfBatch = new SDFBatch (graphicsDevice);

        _size = size;
        _pointCount = pointCount;

        _min = new Vector2 (0f, 0f);
        _max = new Vector2 (_size, _size);

        Reset ();
    }

    public void Reset ()
    {
        _points.Clear ();
        _events.Clear ();
        _beachline.Clear ();
        _edges.Clear ();
        _vertices.Clear ();
        _polygons.Clear ();
        _sweeplineY = _size - 1;

        while (_points.Count < _pointCount)
        {
            Vector2 point = new (s_random.Next (_size), s_random.Next (_size));
            if (_points.Any (p => (p.X - point.X) * (p.X - point.X) + (p.Y - point.Y) * (p.Y - point.Y) <= 100))
            {
                continue;
            }

            _points.Add (point);
        }

        foreach (Vector2 point in _points)
        {
            _events.Add (new Event (point, isSiteEvent: true));
        }

        SortEvents ();

        _state = EStepState.Fortune;
    }

    public void Redo ()
    {
        _events.Clear ();
        _beachline.Clear ();
        _edges.Clear ();
        _vertices.Clear ();
        _polygons.Clear ();
        _sweeplineY = _size - 1;

        foreach (Vector2 point in _points)
        {
            _events.Add (new Event (point, isSiteEvent: true));
        }

        SortEvents ();

        _state = EStepState.Fortune;
    }

    public void NextStep ()
    {
        if (_state == EStepState.Fortune)
        {
            NextFortuneStep ();
        }
        else if (_state == EStepState.Polygon)
        {
            NextPolygonStep ();
        }
    }

    private void NextFortuneStep ()
    {
        if (_events.Count > 0)
        {
            Event e = _events[0];
            _sweeplineY = e.Site.Y;
            UpdateDirectrix (_sweeplineY);

            _events.RemoveAt (0);

            if (e.IsSiteEvent)
            {
                HandleSiteEvent (e);
            }
            else
            {
                HandleCircleEvent (e);
            }
        }
        else
        {
            ExtendEdges ();

            _state = EStepState.Polygon;
        }
    }

    private void NextPolygonStep ()
    {
        Dictionary<Vector2, List<Edge>> siteEdges = [];

        foreach (Edge edge in _edges)
        {
            if (!siteEdges.TryGetValue (edge.LeftSite, out List<Edge>? edges1))
            {
                edges1 = [];
                siteEdges[edge.LeftSite] = edges1;
            }

            edges1.Add (edge);

            if (!siteEdges.TryGetValue (edge.RightSite, out List<Edge>? edges2))
            {
                edges2 = [];
                siteEdges[edge.RightSite] = edges2;
            }

            edges2.Add (edge);
        }

        foreach ((Vector2 site, List<Edge> edges) in siteEdges)
        {
            List<Tuple<Vector2, Vector2>> lines = [];

            Dictionary<Vector2, Edge> twinEdges = [];
            foreach (Edge edge in edges)
            {
                Vector2 twinSite = edge.LeftSite == site ? edge.RightSite : edge.LeftSite;
                if (!twinEdges.TryGetValue (twinSite, out Edge? twinEdge))
                {
                    twinEdges[twinSite] = edge;
                }
                else
                {
                    Vector2 v1 = (edge.EndPoint - site);
                    Vector2 v2 = (twinEdge.EndPoint - site);

                    float cross = v1.X * v2.Y - v1.Y * v2.X;
                    if (cross > 0)
                    {
                        lines.Add (Tuple.Create (edge.EndPoint, twinEdge.EndPoint));
                    }
                    else
                    {
                        lines.Add (Tuple.Create (twinEdge.EndPoint, edge.EndPoint));
                    }

                    twinEdges.Remove (twinSite);
                }
            }

            foreach ((_, Edge edge) in twinEdges)
            {
                Vector2 v1 = (edge.StartPoint - site);
                Vector2 v2 = (edge.EndPoint - site);

                float cross = v1.X * v2.Y - v1.Y * v2.X;
                if (cross > 0)
                {
                    lines.Add (Tuple.Create (edge.StartPoint, edge.EndPoint));
                }
                else
                {
                    lines.Add (Tuple.Create (edge.EndPoint, edge.StartPoint));
                }
            }

            LinkedList<Tuple<Vector2, Vector2>> orderedLines = [];

            orderedLines.AddFirst (lines[^1]);
            lines.RemoveAt (lines.Count - 1);

            do
            {
                if (orderedLines.Last is null)
                {
                    throw new InvalidOperationException ("Ordered lines is empty");
                }

                int index = lines.FindIndex (p => p.Item1 == orderedLines.Last.Value.Item2);
                if (index >= 0)
                {
                    orderedLines.AddLast (lines[index]);
                    lines.RemoveAt (index);
                }
                else
                {
                    break;
                }
            }
            while (true);

            do
            {
                if (orderedLines.First is null)
                {
                    throw new InvalidOperationException ("Ordered lines is empty");
                }

                int index = lines.FindIndex (p => p.Item2 == orderedLines.First.Value.Item1);
                if (index >= 0)
                {
                    orderedLines.AddFirst (lines[index]);
                    lines.RemoveAt (index);
                }
                else
                {
                    break;
                }
            }
            while (true);

            Vector2 firstVertex = orderedLines.First.Value.Item1;
            Vector2 lastVertex = orderedLines.Last.Value.Item2;

            if (firstVertex != lastVertex)
            {
                if (firstVertex.X == lastVertex.X && (firstVertex.X == _min.X || firstVertex.X == _max.X))
                {
                    orderedLines.AddLast (Tuple.Create (lastVertex, firstVertex));
                }
                else if (firstVertex.Y == lastVertex.Y && (firstVertex.Y == _min.Y || firstVertex.Y == _max.Y))
                {
                    orderedLines.AddLast (Tuple.Create (lastVertex, firstVertex));
                }
                else
                {
                    List<Vector2> corners = [
                        new (_min.X, _min.Y),
                        new (_max.X, _min.Y),
                        new (_max.X, _max.Y),
                        new (_min.X, _max.Y)
                    ];

                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < corners.Count; j++)
                        {
                            if (lastVertex.X == corners[j].X || lastVertex.Y == corners[j].Y)
                            {
                                Vector2 v1 = (site - corners[j]);
                                Vector2 v2 = (lastVertex - corners[j]);

                                float cross = v1.X * v2.Y - v1.Y * v2.X;
                                if (cross > 0)
                                {
                                    orderedLines.AddLast (Tuple.Create (lastVertex, corners[j]));
                                    lastVertex = corners[j];
                                    corners.RemoveAt (j);
                                    break;
                                }
                            }
                        }
                    }

                    orderedLines.AddLast (Tuple.Create (lastVertex, firstVertex));
                }
            }

            List<Vector2> vertices = [];
            foreach ((Vector2 startPoint, Vector2 endPoint) in orderedLines)
            {
                vertices.Add (startPoint);
            }

            _polygons.Add (new Polygon (site, vertices));
        }

        _state = EStepState.Finished;
    }

    private void SortEvents ()
    {
        _events.Sort ((lhs, rhs) =>
        {
            int compareY = rhs.Site.Y.CompareTo (lhs.Site.Y);
            if (compareY != 0)
            {
                return compareY;
            }

            return lhs.Site.X.CompareTo (rhs.Site.X);
        });
    }

    private void UpdateDirectrix (float sweeplineY)
    {
        foreach (Parabola parabola in _beachline)
        {
            parabola.RightEdge?.UpdateDirectrix (sweeplineY);
        }
    }

    private void HandleSiteEvent (Event siteEvent)
    {
        if (_beachline.Count == 0)
        {
            _beachline.Add (new Parabola (siteEvent.Site));
            return;
        }

        int index = FindIndexOfAboveParabola (siteEvent.Site);
        Parabola aboveParabola = _beachline[index];
        Parabola leftParabola;
        Parabola rightParabola;

        if (aboveParabola.Focus.Y == siteEvent.Site.Y)
        {
            Vector2 startPoint = new ((aboveParabola.Focus.X + siteEvent.Site.X) / 2f, _size);

            Edge edge;
            if (aboveParabola.Focus.X < siteEvent.Site.X)
            {
                leftParabola = new Parabola (aboveParabola.Focus);
                rightParabola = new Parabola (siteEvent.Site);
                edge = new Edge (startPoint, aboveParabola.Focus, siteEvent.Site);
            }
            else
            {
                leftParabola = new Parabola (siteEvent.Site);
                rightParabola = new Parabola (aboveParabola.Focus);
                edge = new Edge (startPoint, siteEvent.Site, aboveParabola.Focus);
            }

            _edges.Add (edge);

            leftParabola.LeftParabola = aboveParabola.LeftParabola;
            leftParabola.LeftEdge = aboveParabola.LeftEdge;
            leftParabola.RightParabola = rightParabola;
            leftParabola.RightEdge = edge;

            rightParabola.LeftParabola = leftParabola;
            rightParabola.LeftEdge = edge;
            rightParabola.RightParabola = aboveParabola.RightParabola;
            rightParabola.RightEdge = aboveParabola.RightEdge;

            if (leftParabola.LeftParabola != null)
            {
                leftParabola.LeftParabola.RightParabola = leftParabola;
            }

            if (rightParabola.RightParabola != null)
            {
                rightParabola.RightParabola.LeftParabola = rightParabola;
            }

            _beachline.RemoveAt (index);
            _beachline.InsertRange (index, [leftParabola, rightParabola]);

            CheckCircleEvent (leftParabola);
            CheckCircleEvent (rightParabola);

            return;
        }

        Vector2 edgeStartPoint = new (siteEvent.Site.X, Parabola.GetY (aboveParabola.Focus, _sweeplineY, siteEvent.Site.X));
        Edge leftEdge = new (edgeStartPoint, aboveParabola.Focus, siteEvent.Site);
        Edge rightEdge = new (edgeStartPoint, siteEvent.Site, aboveParabola.Focus);
        _edges.Add (leftEdge);
        _edges.Add (rightEdge);

        Parabola parabola = new (siteEvent.Site);
        leftParabola = new (aboveParabola.Focus);
        rightParabola = new (aboveParabola.Focus);

        parabola.LeftParabola = leftParabola;
        parabola.LeftEdge = leftEdge;
        parabola.RightParabola = rightParabola;
        parabola.RightEdge = rightEdge;

        leftParabola.LeftParabola = aboveParabola.LeftParabola;
        leftParabola.LeftEdge = aboveParabola.LeftEdge;
        leftParabola.RightParabola = parabola;
        leftParabola.RightEdge = leftEdge;

        rightParabola.LeftParabola = parabola;
        rightParabola.LeftEdge = rightEdge;
        rightParabola.RightParabola = aboveParabola.RightParabola;
        rightParabola.RightEdge = aboveParabola.RightEdge;

        if (leftParabola.LeftParabola != null)
        {
            leftParabola.LeftParabola.RightParabola = leftParabola;
        }

        if (rightParabola.RightParabola != null)
        {
            rightParabola.RightParabola.LeftParabola = rightParabola;
        }

        if (aboveParabola.CircleEvent != null)
        {
            _events.Remove (aboveParabola.CircleEvent);
            aboveParabola.CircleEvent = null;
        }

        _beachline.RemoveAt (index);
        _beachline.InsertRange (index, [leftParabola, parabola, rightParabola]);

        CheckCircleEvent (leftParabola);
        CheckCircleEvent (rightParabola);
    }

    private int FindIndexOfAboveParabola (Vector2 site)
    {
        int beachLineIndex = 0;
        for (int index = 0; index < _beachline.Count; index++)
        {
            if (_beachline[index].RightEdge is Edge rightEdge)
            {
                if (site.X >= rightEdge.EndPoint.X)
                {
                    beachLineIndex = index + 1;
                }
            }
        }

        return beachLineIndex;
    }

    private void CheckCircleEvent (Parabola parabola)
    {
        Edge? leftEdge = parabola.LeftEdge;
        Edge? rightEdge = parabola.RightEdge;

        if (leftEdge is null || rightEdge is null || !leftEdge.HasValidIntersectPoint (rightEdge))
        {
            return;
        }

        Vector2 intersectPoint = leftEdge.GetIntersectPoint (rightEdge);

        float distance = Vector2.Distance (intersectPoint, parabola.Focus);
        float targetDirectrix = intersectPoint.Y - distance;

        Event circleEvent = new (new Vector2 (parabola.Focus.X, targetDirectrix), false)
        {
            Parabola = parabola,
            VertexPoint = intersectPoint
        };

        parabola.CircleEvent = circleEvent;

        _events.Add (circleEvent);

        SortEvents ();
    }

    private void HandleCircleEvent (Event circleEvent)
    {
        Parabola? parabola = circleEvent.Parabola;
        if (parabola is null)
        {
            return;
        }

        Parabola? leftParabola = parabola.LeftParabola;
        Parabola? rightParabola = parabola.RightParabola;
        if (leftParabola is null || rightParabola is null)
        {
            return;
        }

        Edge? leftEdge = parabola.LeftEdge;
        Edge? rightEdge = parabola.RightEdge;
        if (leftEdge is null || rightEdge is null)
        {
            return;
        }

        Edge edge = new (circleEvent.VertexPoint, leftParabola.Focus, rightParabola.Focus);
        _edges.Add (edge);
        _vertices.Add (circleEvent.VertexPoint);

        leftEdge.SetVertex (circleEvent.VertexPoint);
        rightEdge.SetVertex (circleEvent.VertexPoint);

        leftParabola.RightParabola = rightParabola;
        leftParabola.RightEdge = edge;

        rightParabola.LeftParabola = leftParabola;
        rightParabola.LeftEdge = edge;

        if (parabola.CircleEvent is not null)
        {
            _events.Remove (parabola.CircleEvent);
            parabola.CircleEvent = null;
        }

        if (leftParabola.CircleEvent is not null)
        {
            _events.Remove (leftParabola.CircleEvent);
            leftParabola.CircleEvent = null;
        }

        if (rightParabola.CircleEvent is not null)
        {
            _events.Remove (rightParabola.CircleEvent);
            rightParabola.CircleEvent = null;
        }

        _beachline.Remove (parabola);

        CheckCircleEvent (leftParabola);
        CheckCircleEvent (rightParabola);
    }

    private void ExtendEdges ()
    {
        foreach (Parabola parabola in _beachline)
        {
            if (parabola.LeftEdge != null && parabola.RightEdge != null)
            {
                if (parabola.LeftEdge.HasValidIntersectPoint (parabola.RightEdge))
                {
                    Vector2 vertex = parabola.LeftEdge.GetIntersectPoint (parabola.RightEdge);
                    _vertices.Add (vertex);

                    parabola.LeftEdge.SetVertex (vertex);
                    parabola.RightEdge.SetVertex (vertex);
                }
            }
        }

        _min.X = MathF.Min (_vertices.Min (v => v.X), 0f);
        _min.Y = MathF.Min (_vertices.Min (v => v.Y), 0f);
        _max.X = MathF.Max (_vertices.Max (v => v.X), _size);
        _max.Y = MathF.Max (_vertices.Max (v => v.Y), _size);

        foreach (Edge edge in _edges)
        {
            edge.Extend (_min.X, _min.Y, _max.X, _max.Y);
        }
    }

    public void Draw ()
    {
        _sdfBatch.Begin (Camera.Main.GetViewProjectionMatrix ());
        _sdfBatch.DrawLine (Vector2.Zero, new Vector2 (0f, _size), Color.Green);
        _sdfBatch.DrawLine (Vector2.Zero, new Vector2 (_size, 0f), Color.Green);
        _sdfBatch.DrawLine (new Vector2 (_size), new Vector2 (0f, _size), Color.Green);
        _sdfBatch.DrawLine (new Vector2 (_size), new Vector2 (_size, 0f), Color.Green);

        if (_state != EStepState.Finished)
        {
            _sdfBatch.DrawLine (new Vector2 (0, _sweeplineY), new Vector2 (_size, _sweeplineY), Color.Red);

            foreach ((Vector2 focus, Parabola parabola) in _beachline.GroupBy (p => p.Focus).ToDictionary (g => g.Key, g => g.First ()))
            {
                if (focus.Y == _sweeplineY)
                {
                    continue;
                }

                Vector2 vertex = Parabola.GetVertex (focus, _sweeplineY);
                Vector2 left = parabola.LeftEdge is null ? vertex : parabola.LeftEdge.StartPoint;
                Vector2 right = parabola.RightEdge is null ? vertex : parabola.RightEdge.StartPoint;
                Vector2 min = Vector2.Clamp (Vector2.Min (Vector2.Min (left, right), vertex), _min, _max);
                Vector2 max = Vector2.Clamp (Vector2.Min (Vector2.Max (left, right), vertex), _min, _max);
                _sdfBatch.DrawParabora (focus, new Vector2 (focus.X, _sweeplineY), _min, _max, Color.Orange);
            }

            foreach (Edge edge in _edges)
            {
                _sdfBatch.DrawLine (edge.StartPoint, edge.EndPoint, Color.Blue);
            }
        }

        if (_state == EStepState.Finished)
        {
            foreach (Polygon polygon in _polygons)
            {
                for (int i = 0; i < polygon.Points.Count; i++)
                {
                    _sdfBatch.DrawLine (polygon.Points[i], polygon.Points[(i + 1) % polygon.Points.Count], Color.Blue);
                }
            }
        }

        foreach (Vector2 point in _points)
        {
            _sdfBatch.DrawCircle (point, 3f, Color.Black);
        }

        _sdfBatch.End ();
    }
}