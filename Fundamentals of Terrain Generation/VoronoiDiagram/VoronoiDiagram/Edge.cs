using Microsoft.Xna.Framework;

namespace MonoGame.Samples.VoronoiDiagram;

public class Edge
{
    private static int _edgeIDCounter = 0;

    private enum Direction
    {
        Left,
        Right,
    }

    public readonly int EdgeID = _edgeIDCounter++;

    public Vector2 StartPoint;
    public Vector2 EndPoint;
    public bool HasVertex;

    public Vector2 LeftSite;
    public Vector2 RightSite;

    private readonly float _lerp;
    private readonly Direction _direction;

    public bool IsInfinityLerp () => float.IsPositiveInfinity (_lerp) || float.IsNegativeInfinity (_lerp);

    public Edge (Vector2 startPoint, Vector2 leftSite, Vector2 rightSite)
    {
        StartPoint = startPoint;
        EndPoint = startPoint;
        HasVertex = false;

        LeftSite = leftSite;
        RightSite = rightSite;

        _lerp = GetLerp ();
        _direction = GetDirection ();
    }

    private float GetLerp ()
    {
        if (RightSite.Y == LeftSite.Y)
        {
            return float.PositiveInfinity;
        }

        return (RightSite.X - LeftSite.X) / (LeftSite.Y - RightSite.Y);
    }

    private Direction GetDirection ()
    {
        if (_lerp > 0)
        {
            return LeftSite.X < RightSite.X ? Direction.Left : Direction.Right;
        }
        else if (_lerp < 0)
        {
            return LeftSite.X > RightSite.X ? Direction.Left : Direction.Right;
        }
        else
        {
            return LeftSite.Y > RightSite.Y ? Direction.Left : Direction.Right;
        }
    }

    public void SetVertex (Vector2 vertexPoint)
    {
        if (HasVertex)
        {
            return;
        }

        HasVertex = true;
        EndPoint = vertexPoint;
    }

    public void UpdateDirectrix (float directrixY)
    {
        if (!HasVertex)
        {
            Vector2[] intersectPoints = Parabola.GetIntersectPoints (LeftSite, RightSite, directrixY);

            if (intersectPoints.Length == 0)
            {
                return;
            }
            else if (intersectPoints.Length == 1)
            {
                EndPoint = intersectPoints[0];
            }
            else
            {
                if (_direction == Direction.Left)
                {
                    if (intersectPoints[0].X < intersectPoints[1].X)
                    {
                        EndPoint = intersectPoints[0];
                    }
                    else
                    {
                        EndPoint = intersectPoints[1];
                    }
                }
                else
                {
                    if (intersectPoints[0].X > intersectPoints[1].X)
                    {
                        EndPoint = intersectPoints[0];
                    }
                    else
                    {
                        EndPoint = intersectPoints[1];
                    }
                }
            }
        }
    }

    public void Extend (float minX, float minY, float maxX, float maxY)
    {
        if (HasVertex)
        {
            return;
        }

        HasVertex = true;

        if (_direction == Direction.Left)
        {
            if (_lerp >= 0)
            {
                float x = GetX (minY);
                if (x >= minX && x <= maxX)
                {
                    EndPoint = new Vector2 (x, minY);
                }
            }
            else
            {
                float x = GetX (maxY);
                if (x >= minX && x <= maxX)
                {
                    EndPoint = new Vector2 (x, maxY);
                }
            }

            float y = GetY (minX);
            if (y >= minY && y <= maxY)
            {
                EndPoint = new Vector2 (minX, y);
            }
        }
        else
        {
            if (_lerp >= 0)
            {
                float x = GetX (maxY);
                if (x >= minX && x <= maxX)
                {
                    EndPoint = new Vector2 (x, maxY);
                }
            }
            else
            {
                float x = GetX (minY);
                if (x >= minX && x <= maxX)
                {
                    EndPoint = new Vector2 (x, minY);
                }
            }

            float y = GetY (maxX);
            if (y >= minY && y <= maxY)
            {
                EndPoint = new Vector2 (maxX, y);
            }
        }
    }

    public float GetX (float y)
    {
        if (_lerp == 0)
        {
            return StartPoint.X;
        }

        return StartPoint.X + (y - StartPoint.Y) / _lerp;
    }

    public float GetY (float x)
    {
        if (IsInfinityLerp ())
        {
            return StartPoint.Y;
        }

        return StartPoint.Y + (x - StartPoint.X) * _lerp;
    }

    public bool HasValidIntersectPoint (Edge? other)
    {
        if (other == null)
        {
            return false;
        }

        if (_lerp == other._lerp || StartPoint == other.StartPoint)
        {
            return false;
        }

        Vector2 intersectPoint = GetIntersectPoint (other);

        bool isValid = true;
        if (!IsInfinityLerp ())
        {
            if (_direction == Direction.Left)
            {
                isValid &= intersectPoint.X <= StartPoint.X;
            }
            else
            {
                isValid &= intersectPoint.X >= StartPoint.X;
            }
        }

        if (!other.IsInfinityLerp ())
        {
            if (other._direction == Direction.Left)
            {
                isValid &= intersectPoint.X <= other.StartPoint.X;
            }
            else
            {
                isValid &= intersectPoint.X >= other.StartPoint.X;
            }
        }

        return isValid;
    }

    public Vector2 GetIntersectPoint (Edge other)
    {
        if (IsInfinityLerp ())
        {
            float x = StartPoint.X;
            float y = other.StartPoint.Y + (x - other.StartPoint.X) * other._lerp;
            return new Vector2 (x, y);
        }
        else if (other.IsInfinityLerp ())
        {
            float x = other.StartPoint.X;
            float y = StartPoint.Y + (x - StartPoint.X) * _lerp;
            return new Vector2 (x, y);
        }
        else
        {
            float x = (StartPoint.Y - other.StartPoint.Y + (other.StartPoint.X * other._lerp) - (StartPoint.X * _lerp)) / (other._lerp - _lerp);
            float y = StartPoint.Y + (x - StartPoint.X) * _lerp;
            return new Vector2 (x, y);
        }
    }
}
