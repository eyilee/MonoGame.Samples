using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VoronoiDiagram
{
    public class VoronoiDiagram
    {
        private static readonly Random _random = new ();

        private readonly Texture2D _texture;
        private readonly int _cellSize = 2;

        private readonly int _size;
        private readonly int _pointCount;

        private readonly List<Vector2> _points = [];
        private readonly List<Event> _events = [];
        private readonly List<Parabola> _beachline = [];
        private readonly List<Edge> _edges = [];
        private readonly List<Vector2> _vertices = [];
        private readonly List<Polygon> _polygons = [];
        private float _sweeplineY;
        private float _minX = 0f;
        private float _minY = 0f;
        private float _maxX = 0f;
        private float _maxY = 0f;

        public enum EStepState
        {
            Fortune,
            Polygon,
            Finished,
        }

        private EStepState _state;

        public VoronoiDiagram (int size, int pointCount)
        {
            if (_texture == null)
            {
                _texture = new Texture2D (Core.GraphicsDevice, 1, 1);
                _texture.SetData ([Color.White]);
            }

            if (size <= 0)
            {
                throw new ArgumentException ("Size must be greater than 0", nameof (size));
            }

            _size = size;
            _pointCount = pointCount;

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
            _minX = 0;
            _minY = 0;
            _maxX = _size;
            _maxY = _size;

            while (_points.Count < _pointCount)
            {
                Vector2 point = new (_random.Next (_size), _random.Next (_size));
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
            _minX = 0;
            _minY = 0;
            _maxX = _size;
            _maxY = _size;

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
                if (!siteEdges.TryGetValue (edge.LeftSite, out List<Edge> edges1))
                {
                    edges1 = [];
                    siteEdges[edge.LeftSite] = edges1;
                }

                edges1.Add (edge);

                if (!siteEdges.TryGetValue (edge.RightSite, out List<Edge> edges2))
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
                    if (!twinEdges.TryGetValue (twinSite, out Edge twinEdge))
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
                    if (firstVertex.X == lastVertex.X && (firstVertex.X == _minX || firstVertex.X == _maxX))
                    {
                        orderedLines.AddLast (Tuple.Create (lastVertex, firstVertex));
                    }
                    else if (firstVertex.Y == lastVertex.Y && (firstVertex.Y == _minY || firstVertex.Y == _maxY))
                    {
                        orderedLines.AddLast (Tuple.Create (lastVertex, firstVertex));
                    }
                    else
                    {
                        List<Vector2> corners = [
                            new (_minX, _minY),
                            new (_maxX, _minY),
                            new (_maxX, _maxY),
                            new (_minX, _maxY)
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
                if (_beachline[index].RightEdge != null)
                {
                    if (site.X >= _beachline[index].RightEdge.EndPoint.X)
                    {
                        beachLineIndex = index + 1;
                    }
                }
            }

            return beachLineIndex;
        }

        private void CheckCircleEvent (Parabola parabola)
        {
            Edge leftEdge = parabola.LeftEdge;
            Edge rightEdge = parabola.RightEdge;

            if (leftEdge == null || !leftEdge.HasValidIntersectPoint (rightEdge))
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
            Parabola parabola = circleEvent.Parabola;
            Parabola leftParabola = parabola.LeftParabola;
            Parabola rightParabola = parabola.RightParabola;

            if (leftParabola == null || rightParabola == null)
            {
                return;
            }

            Edge edge = new (circleEvent.VertexPoint, leftParabola.Focus, rightParabola.Focus);
            _edges.Add (edge);
            _vertices.Add (circleEvent.VertexPoint);

            parabola.LeftEdge.SetVertex (circleEvent.VertexPoint);
            parabola.RightEdge.SetVertex (circleEvent.VertexPoint);

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

            if (_vertices.Count > 0)
            {
                _minX = MathF.Min (_vertices.Min (v => v.X), 0f);
                _minY = MathF.Min (_vertices.Min (v => v.Y), 0f);
                _maxX = MathF.Max (_vertices.Max (v => v.X), _size);
                _maxY = MathF.Max (_vertices.Max (v => v.Y), _size);
            }

            foreach (Edge edge in _edges)
            {
                edge.Extend (_minX, _minY, _maxX, _maxY);
            }
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            if (_state == EStepState.Fortune)
            {
                DrawBackground (spriteBatch);
                DrawSweepLine (spriteBatch);

                foreach ((Vector2 focus, Parabola parabola) in _beachline.GroupBy (p => p.Focus).ToDictionary (g => g.Key, g => g.First ()))
                {
                    DrawParabola (spriteBatch, parabola, _sweeplineY, Color.Orange * 0.5f);
                }

                foreach (Edge edge in _edges)
                {
                    DrawLine (spriteBatch, edge.StartPoint, edge.EndPoint, Color.Blue * 0.5f);
                }
            }

            if (_state == EStepState.Finished)
            {
                foreach (Polygon polygon in _polygons)
                {
                    for (int i = 0; i < polygon.Points.Count; i++)
                    {
                        DrawLine (spriteBatch, polygon.Points[i], polygon.Points[(i + 1) % polygon.Points.Count], Color.Blue * 0.5f);
                    }
                }
            }

            foreach (Vector2 point in _points)
            {
                DrawPoint (spriteBatch, point.X, point.Y, Color.Black);
            }
        }

        private void DrawBackground (SpriteBatch spriteBatch)
        {
            int x = (int)MathF.Floor (_minX);
            int y = (int)MathF.Floor (_minY);
            int width = (int)MathF.Ceiling (_maxX) - x;
            int height = (int)MathF.Ceiling (_maxY) - y;
            spriteBatch.Draw (_texture, ToViewRectangle (x, y, width, height), Color.White);
        }

        private void DrawSweepLine (SpriteBatch spriteBatch)
        {
            int x = (int)MathF.Floor (_minX);
            int width = (int)MathF.Ceiling (_maxX) - x;
            spriteBatch.Draw (_texture, ToViewRectangle (x, (int)_sweeplineY, width, 1), Color.Red);
        }

        private void DrawPoint (SpriteBatch spriteBatch, int x, int y, Color color)
        {
            if (x < _minX || x > _maxX || y < _minY || y > _maxY)
            {
                return;
            }

            spriteBatch.Draw (_texture, ToViewRectangle (x, y, 1, 1), color);
        }

        private void DrawPoint (SpriteBatch spriteBatch, int x, float y, Color color)
        {
            int floorY = (int)MathF.Floor (y);
            int ceilingY = (int)MathF.Ceiling (y);

            if (floorY == ceilingY)
            {
                DrawPoint (spriteBatch, x, floorY, color);
                return;
            }

            float rateY = y - MathF.Floor (y);

            if (floorY >= _minY && floorY < _maxY)
            {
                DrawPoint (spriteBatch, x, floorY, color * (1f - rateY));
            }

            if (ceilingY >= _minY && ceilingY < _maxY)
            {
                DrawPoint (spriteBatch, x, ceilingY, color * rateY);
            }
        }

        private void DrawPoint (SpriteBatch spriteBatch, float x, int y, Color color)
        {
            int floorX = (int)MathF.Floor (x);
            int ceilingX = (int)MathF.Ceiling (x);

            if (floorX == ceilingX)
            {
                DrawPoint (spriteBatch, floorX, y, color);
                return;
            }

            float rateX = x - MathF.Floor (x);

            if (floorX >= _minX && floorX < _maxX)
            {
                DrawPoint (spriteBatch, floorX, y, color * (1f - rateX));
            }

            if (ceilingX >= _minX && ceilingX < _maxX)
            {
                DrawPoint (spriteBatch, ceilingX, y, color * rateX);
            }
        }

        private void DrawPoint (SpriteBatch spriteBatch, float x, float y, Color color)
        {
            int floorX = (int)MathF.Floor (x);
            int ceilingX = (int)MathF.Ceiling (x);

            int floorY = (int)MathF.Floor (y);
            int ceilingY = (int)MathF.Ceiling (y);

            if (floorX == ceilingX && floorY == ceilingY)
            {
                DrawPoint (spriteBatch, floorX, floorY, color);
                return;
            }
            else if (floorX == ceilingX)
            {
                DrawPoint (spriteBatch, floorX, y, color);
                return;
            }
            else if (floorY == ceilingY)
            {
                DrawPoint (spriteBatch, x, floorY, color);
                return;
            }

            float rateX = x - MathF.Floor (x);
            float rateY = y - MathF.Floor (y);

            if (floorX >= _minX && floorX < _maxX)
            {
                if (floorY >= _minY && floorY < _maxY)
                {
                    DrawPoint (spriteBatch, floorX, floorY, color * (1f - rateX) * (1f - rateY));
                }

                if (ceilingY >= _minY && ceilingY < _maxY)
                {
                    DrawPoint (spriteBatch, floorX, ceilingY, color * (1f - rateX) * rateY);
                }
            }

            if (ceilingX >= _minX && ceilingX < _maxX)
            {
                if (floorY >= _minY && floorY < _maxY)
                {
                    DrawPoint (spriteBatch, ceilingX, floorY, color * rateX * (1f - rateY));
                }

                if (ceilingY >= _minY && ceilingY < _maxY)
                {
                    DrawPoint (spriteBatch, ceilingX, ceilingY, color * rateX * rateY);
                }
            }
        }

        private void DrawLine (SpriteBatch spriteBatch, Vector2 startPoint, Vector2 endPoint, Color color)
        {
            float startX = startPoint.X;
            float startY = startPoint.Y;
            float endX = endPoint.X;
            float endY = endPoint.Y;

            float deltaX = MathF.Abs (endPoint.X - startPoint.X);
            float deltaY = MathF.Abs (endPoint.Y - startPoint.Y);

            if (deltaX > deltaY)
            {
                if (startX > endX)
                {
                    (startX, endX) = (endX, startX);
                    (startY, endY) = (endY, startY);
                }

                float fromX = MathF.Max (startX, _minX);
                float toX = MathF.Min (endX, _maxX);

                float lerp = (endY - startY) / (endX - startX);

                if (fromX < MathF.Ceiling (fromX))
                {
                    float y = startY + (fromX - startX) * lerp;
                    DrawPoint (spriteBatch, fromX, y, color);
                }

                for (int x = (int)MathF.Floor (fromX); x <= MathF.Ceiling (toX); x++)
                {
                    float y = startY + (x - startX) * lerp;
                    DrawPoint (spriteBatch, x, y, color);
                }

                if (toX > MathF.Floor (toX))
                {
                    float y = startY + (toX - startX) * lerp;
                    DrawPoint (spriteBatch, toX, y, color);
                }
            }
            else
            {
                if (startY > endY)
                {
                    (startX, endX) = (endX, startX);
                    (startY, endY) = (endY, startY);
                }

                float fromY = MathF.Max (startY, _minY);
                float toY = MathF.Min (endY, _maxY);

                if (startX == endX)
                {
                    if (fromY < MathF.Ceiling (fromY))
                    {
                        DrawPoint (spriteBatch, startX, fromY, color);
                    }

                    for (int y = (int)MathF.Floor (fromY); y <= MathF.Ceiling (toY); y++)
                    {
                        DrawPoint (spriteBatch, startX, y, color);
                    }

                    if (toY > MathF.Floor (toY))
                    {
                        DrawPoint (spriteBatch, startX, toY, color);
                    }
                }
                else
                {
                    float lerp = (endY - startY) / (endX - startX);

                    if (fromY < MathF.Ceiling (fromY))
                    {
                        float x = startX + (fromY - startY) / lerp;
                        DrawPoint (spriteBatch, x, fromY, color);
                    }

                    for (int y = (int)MathF.Ceiling (fromY); y <= MathF.Floor (toY); y++)
                    {
                        float x = startX + (y - startY) / lerp;
                        DrawPoint (spriteBatch, x, y, color);
                    }

                    if (toY > MathF.Floor (toY))
                    {
                        float x = startX + (toY - startY) / lerp;
                        DrawPoint (spriteBatch, x, toY, color);
                    }
                }
            }
        }

        private void DrawParabola (SpriteBatch spriteBatch, Parabola parabola, float directrixY, Color color)
        {
            for (int x = (int)MathF.Floor (_minX); x <= MathF.Ceiling (_maxX); x++)
            {
                float y = Parabola.GetY (parabola.Focus, directrixY, x);
                if (y > directrixY && y < _maxY)
                {
                    DrawPoint (spriteBatch, x, y, color);
                }
            }
        }

        private Rectangle ToViewRectangle (int x, int y, int width, int height)
        {
            int offset = _size * _cellSize / 2;
            int viewWidth = width * _cellSize;
            int viewHeight = height * _cellSize;
            int viewX = (x * _cellSize) - offset;
            int viewY = offset - (y * _cellSize) - viewHeight;
            return new Rectangle (viewX, viewY, viewWidth, viewHeight);
        }
    }
}