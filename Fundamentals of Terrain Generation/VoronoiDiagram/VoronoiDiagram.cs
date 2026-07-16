using Microsoft.Xna.Framework;
using MonoGame.Library;
using MonoGame.Library.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VoronoiDiagram;

public class VoronoiDiagram
{
    private readonly Random _random = new ();

    private readonly int _size;

    private readonly int _siteCount;

    private readonly List<Site> _sites = [];

    private readonly List<Event> _events = [];

    private readonly List<Parabola> _beachline = [];

    private readonly List<Edge> _edges = [];

    private readonly List<Vector2> _vertices = [];

    private readonly List<Polygon> _polygons = [];

    private readonly LineSegment _sweepLine = new ();

    private float _sweeplineY;

    private Vector2 _min;

    private Vector2 _max;

    private IEnumerator<int>? _stepBehaviour;

    public VoronoiDiagram (int size, int siteCount)
    {
        _size = size;
        _siteCount = siteCount;

        Reset ();
    }

    public void Reset ()
    {
        _sites.Clear ();
        _events.Clear ();
        _beachline.Clear ();
        _edges.Clear ();
        _vertices.Clear ();
        _polygons.Clear ();
        _sweeplineY = 0f;
        _min = new Vector2 (0f, 0f);
        _max = new Vector2 (_size, _size);

        //while (_sites.Count < _siteCount)
        //{
        //    Vector2 position = new (_random.Next (_size), _random.Next (_size));

        //    if (_sites.Any (s => Vector2.DistanceSquared (s.Point, position) <= 1024f))
        //    {
        //        continue;
        //    }

        //    _sites.Add (new Site { Point = position, Color = Color.Black, Radius = 3f });
        //}

        _sites.Add (new Site { Point = new Vector2 (128f, 128f), Color = Color.Black, Radius = 3f });
        _sites.Add (new Site { Point = new Vector2 (128f, 214f), Color = Color.Black, Radius = 3f });
        _sites.Add (new Site { Point = new Vector2 (192f, 192f), Color = Color.Black, Radius = 3f });
        _sites.Add (new Site { Point = new Vector2 (214f, 214f), Color = Color.Black, Radius = 3f });
        _sites.Add (new Site { Point = new Vector2 (214f, 256f), Color = Color.Black, Radius = 3f });
        _sites.Add (new Site { Point = new Vector2 (512f, 512f), Color = Color.Black, Radius = 3f });

        _events.AddRange (_sites.Select (s => new SiteEvent (s.Point)));
        _events.Sort ();

        _stepBehaviour = Run ();

        Draw ();
    }

    public void Redo ()
    {
        _events.Clear ();
        _beachline.Clear ();
        _edges.Clear ();
        _vertices.Clear ();
        _polygons.Clear ();
        _sweeplineY = 0f;
        _min = new Vector2 (0f, 0f);
        _max = new Vector2 (_size, _size);

        _events.AddRange (_sites.Select (s => new SiteEvent (s.Point)));
        _events.Sort ();

        _stepBehaviour = Run ();

        Draw ();
    }

    public void NextStep ()
    {
        if (_stepBehaviour != null)
        {
            if (!_stepBehaviour.MoveNext ())
            {
                _stepBehaviour = null;
            }

            Draw ();
        }
    }

    private IEnumerator<int> Run ()
    {
        while (_events.Count > 0)
        {
            Event e = _events[0];

            while (_sweeplineY < e.Site.Y)
            {
                _sweeplineY = float.Min (_sweeplineY + 1f, e.Site.Y);
                UpdateDirectrix (_sweeplineY);

                yield return 0;
            }

            _sweeplineY = e.Site.Y;
            UpdateDirectrix (_sweeplineY);

            _events.RemoveAt (0);

            if (e is SiteEvent siteEvent)
            {
                HandleSiteEvent (siteEvent);
            }
            else if (e is CircleEvent circleEvent)
            {
                HandleCircleEvent (circleEvent);
            }

            yield return 0;
        }

        //ExtendEdges ();

        //BuildPolygons ();

        //ClipPolygons ();
    }

    private void Draw ()
    {
        foreach (Parabola parabola in _beachline)
        {
            Vector2 left = parabola.LeftEdge == null ? new Vector2 (_min.X, 0f) : parabola.LeftEdge.Start;
            Vector2 right = parabola.RightEdge == null ? new Vector2 (_max.X, 0f) : parabola.RightEdge.Start;
            Vector2 vertex = Parabola.GetVertex (parabola.Focus, _sweeplineY);
            Vector2 min = Vector2.Clamp (Vector2.Min (Vector2.Min (left, right), vertex), _min, _max);
            Vector2 max = Vector2.Clamp (Vector2.Max (Vector2.Max (left, right), vertex), _min, _max);
            parabola.Position = (min + max) * 0.5f;
            parabola.Scale = max - min;
            parabola.Position = new Vector2 (_size * 0.5f, _size * 0.5f);
            parabola.Scale = new Vector2 (_size, _size);
            parabola.Vertex = vertex;
        }

        _sweepLine.Start = new Vector2 (0, _sweeplineY);
        _sweepLine.End = new Vector2 (_size, _sweeplineY);
    }

    public void Draw (RenderManager render)
    {
        foreach (Site site in _sites)
        {
            site.Draw (render);
        }

        //foreach (Parabola parabola in _beachline)
        //{
        //    parabola.Draw (render);
        //}

        foreach (Edge edge in _edges)
        {
            edge.Draw (render);

            if (edge.Start.Y > _sweeplineY || edge.End.Y > _sweeplineY)
            {
                int a = 0;
                a++;
            }
        }

        _sweepLine.Draw (render);
    }

    private void UpdateDirectrix (float directrixY)
    {
        foreach (Parabola parabola in _beachline)
        {
            parabola.RightEdge?.UpdateDirectrix (directrixY);
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
            Vector2 startPoint = new ((aboveParabola.Focus.X + siteEvent.Site.X) * 0.5f, 0f);

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
                if (site.X >= rightEdge.End.X)
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

        if (leftEdge == null || rightEdge == null || !leftEdge.HasValidIntersectPoint (rightEdge))
        {
            return;
        }

        Vector2 intersectPoint = leftEdge.GetIntersectPoint (rightEdge);
        float distance = Vector2.Distance (intersectPoint, parabola.Focus);
        float targetDirectrix = intersectPoint.Y + distance;

        parabola.CircleEvent = new CircleEvent (new Vector2 (parabola.Focus.X, targetDirectrix), parabola, intersectPoint);

        _events.Add (parabola.CircleEvent);
        _events.Sort ();
    }

    private void HandleCircleEvent (CircleEvent circleEvent)
    {
        Parabola? parabola = circleEvent.Parabola;

        if (parabola == null)
        {
            return;
        }

        Parabola? leftParabola = parabola.LeftParabola;
        Parabola? rightParabola = parabola.RightParabola;

        if (leftParabola == null || rightParabola == null)
        {
            return;
        }

        Edge? leftEdge = parabola.LeftEdge;
        Edge? rightEdge = parabola.RightEdge;

        if (leftEdge == null || rightEdge == null)
        {
            return;
        }

        Edge edge = new (circleEvent.Vertex, leftParabola.Focus, rightParabola.Focus);

        _edges.Add (edge);
        _vertices.Add (circleEvent.Vertex);

        if (circleEvent.Vertex.Y > _sweeplineY)
        {
            int a = 0;
            a++;
        }

        leftEdge.SetVertex (circleEvent.Vertex);
        rightEdge.SetVertex (circleEvent.Vertex);

        leftParabola.RightParabola = rightParabola;
        leftParabola.RightEdge = edge;

        rightParabola.LeftParabola = leftParabola;
        rightParabola.LeftEdge = edge;

        if (parabola.CircleEvent != null)
        {
            _events.Remove (parabola.CircleEvent);
            parabola.CircleEvent = null;
        }

        if (leftParabola.CircleEvent != null)
        {
            _events.Remove (leftParabola.CircleEvent);
            leftParabola.CircleEvent = null;
        }

        if (rightParabola.CircleEvent != null)
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

        _min.X = float.Min (_vertices.Min (v => v.X), 0f);
        _min.Y = float.Min (_vertices.Min (v => v.Y), 0f);
        _max.X = float.Max (_vertices.Max (v => v.X), _size);
        _max.Y = float.Max (_vertices.Max (v => v.Y), _size);

        foreach (Edge edge in _edges)
        {
            edge.Extend (_min.X, _min.Y, _max.X, _max.Y);
        }
    }

    private void BuildPolygons ()
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

        List<Vector2> corners = [
            new (_min.X, _min.Y),
            new (_max.X, _min.Y),
            new (_max.X, _max.Y),
            new (_min.X, _max.Y)
            ];

        foreach ((Vector2 site, List<Edge> edges) in siteEdges)
        {
            List<Vector2[]> lines = [];

            foreach (IGrouping<Vector2, Edge> grouping in edges.GroupBy (e => e.Start))
            {
                List<Edge> twinEdges = [.. grouping];

                if (twinEdges.Count == 1)
                {
                    lines.Add ([twinEdges[0].Start, twinEdges[0].End]);
                }
                else if (twinEdges.Count == 2)
                {
                    lines.Add ([twinEdges[0].End, twinEdges[1].End]);
                }
            }

            lines.ForEach (line =>
            {
                float cross = (line[0] - site).Cross (line[1] - line[0]);
                if (cross < 0)
                {
                    (line[0], line[1]) = (line[1], line[0]);
                }
            });

            lines.Sort ((lhs, rhs) => float.Atan2 (lhs[0].Y - site.Y, lhs[0].X - site.X).CompareTo (float.Atan2 (rhs[0].Y - site.Y, rhs[0].X - site.X)));

            for (int i = 0; i < lines.Count; i++)
            {
                Vector2 current = lines[i][1];
                Vector2 next = lines[(i + 1) % lines.Count][0];
                if (current == next)
                {
                    continue;
                }

                int currentCornerIndex = FindCornerIndex (current);
                int nextCornerIndex = FindCornerIndex (next);
                if (currentCornerIndex == -1 || nextCornerIndex == -1)
                {
                    continue;
                }

                List<Vector2[]> insertions = [];

                while (currentCornerIndex != nextCornerIndex)
                {
                    insertions.Add ([current, corners[currentCornerIndex]]);
                    current = corners[currentCornerIndex];
                    currentCornerIndex = (currentCornerIndex + 1) % corners.Count;
                }

                insertions.Add ([current, next]);
                lines.InsertRange (i + 1, insertions);

                break;
            }

            List<Vector2> vertices = [];
            for (int i = 0; i < lines.Count; i++)
            {
                vertices.Add (lines[i][0]);
            }

            _polygons.Add (new Polygon (vertices));
        }
    }

    private int FindCornerIndex (Vector2 vertex)
    {
        const float epsilon = 1e-3f;

        if (float.Abs (vertex.X - _min.X) < epsilon)
        {
            return 0;
        }
        else if (float.Abs (vertex.Y - _min.Y) < epsilon)
        {
            return 1;
        }
        else if (float.Abs (vertex.X - _max.X) < epsilon)
        {
            return 2;
        }
        else if (float.Abs (vertex.Y - _max.Y) < epsilon)
        {
            return 3;
        }
        else
        {
            return -1;
        }
    }

    private void ClipPolygons ()
    {
        foreach (Polygon polygon in _polygons)
        {
            ClipAgainst (polygon, p => p.X >= 0, (s, e) => IntersectX (s, e, 0));
            ClipAgainst (polygon, p => p.X <= _size, (s, e) => IntersectX (s, e, _size));
            ClipAgainst (polygon, p => p.Y >= 0, (s, e) => IntersectY (s, e, 0));
            ClipAgainst (polygon, p => p.Y <= _size, (s, e) => IntersectY (s, e, _size));
        }
    }

    private static void ClipAgainst (Polygon polygon, Func<Vector2, bool> isInside, Func<Vector2, Vector2, Vector2> intersect)
    {
        List<Vector2> clippedPoints = [];

        for (int i = 0; i < polygon.Points.Count; i++)
        {
            Vector2 current = polygon.Points[i];
            Vector2 next = polygon.Points[(i + 1) % polygon.Points.Count];

            bool currentInside = isInside (current);
            bool nextInside = isInside (next);

            if (currentInside && nextInside)
            {
                clippedPoints.Add (next);
            }
            else if (currentInside && !nextInside)
            {
                clippedPoints.Add (intersect (current, next));
            }
            else if (!currentInside && nextInside)
            {
                clippedPoints.Add (intersect (current, next));
                clippedPoints.Add (next);
            }
        }

        polygon.Points = clippedPoints;
    }

    private static Vector2 IntersectX (Vector2 startPoint, Vector2 endPoint, float x)
    {
        return Vector2.Lerp (startPoint, endPoint, (endPoint.X - startPoint.X) / (x - startPoint.X));
        //Vector2 direction = endPoint - startPoint;
        //float t = (x - startPoint.X) / direction.X;
        //return startPoint + direction * t;
    }

    private static Vector2 IntersectY (Vector2 startPoint, Vector2 endPoint, float y)
    {
        return Vector2.Lerp (startPoint, endPoint, (endPoint.Y - startPoint.Y) / (y - startPoint.Y));
        //Vector2 direction = endPoint - startPoint;
        //float t = (y - startPoint.Y) / direction.Y;
        //return startPoint + direction * t;
    }

    //public void Draw ()
    //{
    //    _sdfBatch.Begin (Camera.Main.GetViewProjectionMatrix ());
    //    _sdfBatch.DrawLine (Vector2.Zero, new Vector2 (0f, _size), Color.Green);
    //    _sdfBatch.DrawLine (Vector2.Zero, new Vector2 (_size, 0f), Color.Green);
    //    _sdfBatch.DrawLine (new Vector2 (_size), new Vector2 (0f, _size), Color.Green);
    //    _sdfBatch.DrawLine (new Vector2 (_size), new Vector2 (_size, 0f), Color.Green);

    //    if (_state != EStepState.Finished)
    //    {
    //        _sdfBatch.DrawLine (new Vector2 (0, _sweeplineY), new Vector2 (_size, _sweeplineY), Color.Red);

    //        foreach ((Vector2 focus, Parabola parabola) in _beachline.GroupBy (p => p.Focus).ToDictionary (g => g.Key, g => g.First ()))
    //        {
    //            if (focus.Y == _sweeplineY)
    //            {
    //                continue;
    //            }

    //            Vector2 vertex = Parabola.GetVertex (focus, _sweeplineY);
    //            Vector2 left = parabola.LeftEdge is null ? vertex : parabola.LeftEdge.StartPoint;
    //            Vector2 right = parabola.RightEdge is null ? vertex : parabola.RightEdge.StartPoint;
    //            Vector2 min = Vector2.Clamp (Vector2.Min (Vector2.Min (left, right), vertex), _min, _max);
    //            Vector2 max = Vector2.Clamp (Vector2.Min (Vector2.Max (left, right), vertex), _min, _max);
    //            _sdfBatch.DrawParabora (focus, vertex, _min, _max, Color.Orange);
    //        }

    //        foreach (Edge edge in _edges)
    //        {
    //            _sdfBatch.DrawLine (edge.StartPoint, edge.EndPoint, Color.Blue);
    //        }
    //    }

    //    if (_state == EStepState.Finished)
    //    {
    //        foreach (Polygon polygon in _polygons)
    //        {
    //            for (int i = 0; i < polygon.Points.Count; i++)
    //            {
    //                _sdfBatch.DrawLine (polygon.Points[i], polygon.Points[(i + 1) % polygon.Points.Count], Color.Blue);
    //            }
    //        }
    //    }

    //    foreach (Vector2 point in _points)
    //    {
    //        _sdfBatch.DrawCircle (point, 3f, Color.Black);
    //    }

    //    _sdfBatch.End ();
    //}
}