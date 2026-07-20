using Microsoft.Xna.Framework;
using MonoGame.Library.Graphics;

namespace VoronoiDiagram;

public class Edge
{
    public enum EdgeDirection
    {
        Left,
        Right
    }

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

    public bool HasVertex;

    public Vector2 LeftSite;

    public Vector2 RightSite;

    public float Lerp;

    public EdgeDirection Direction;

    public bool IsInfinityLerp () => float.IsPositiveInfinity (Lerp) || float.IsNegativeInfinity (Lerp);

    public Edge (Vector2 startPoint, Vector2 leftSite, Vector2 rightSite)
    {
        Start = startPoint;
        End = startPoint;
        LeftSite = leftSite;
        RightSite = rightSite;

        Lerp = GetLerp ();
        Direction = GetDirection ();
    }

    public void Draw (RenderManager render)
    {
        _shape.Draw (render);
    }

    private float GetLerp ()
    {
        if (RightSite.Y == LeftSite.Y)
        {
            return float.PositiveInfinity;
        }

        return (RightSite.X - LeftSite.X) / (LeftSite.Y - RightSite.Y);
    }

    private EdgeDirection GetDirection ()
    {
        if (Lerp > 0)
        {
            return LeftSite.X < RightSite.X ? EdgeDirection.Right : EdgeDirection.Left;
        }
        else if (Lerp < 0)
        {
            return LeftSite.X > RightSite.X ? EdgeDirection.Right : EdgeDirection.Left;
        }
        else
        {
            return LeftSite.Y > RightSite.Y ? EdgeDirection.Right : EdgeDirection.Left;
        }
    }

    public void SetVertex (Vector2 vertexPoint)
    {
        if (HasVertex)
        {
            return;
        }

        HasVertex = true;
        End = vertexPoint;
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
                End = intersectPoints[0];
            }
            else
            {
                if (Direction == EdgeDirection.Left)
                {
                    if (intersectPoints[0].X < intersectPoints[1].X)
                    {
                        End = intersectPoints[0];
                    }
                    else
                    {
                        End = intersectPoints[1];
                    }
                }
                else
                {
                    if (intersectPoints[0].X > intersectPoints[1].X)
                    {
                        End = intersectPoints[0];
                    }
                    else
                    {
                        End = intersectPoints[1];
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

        if (Direction == EdgeDirection.Left)
        {
            if (Lerp >= 0)
            {
                float x = GetX (minY);
                if (x >= minX && x <= maxX)
                {
                    End = new Vector2 (x, minY);
                }
            }
            else
            {
                float x = GetX (maxY);
                if (x >= minX && x <= maxX)
                {
                    End = new Vector2 (x, maxY);
                }
            }

            float y = GetY (minX);
            if (y >= minY && y <= maxY)
            {
                End = new Vector2 (minX, y);
            }
        }
        else
        {
            if (Lerp >= 0)
            {
                float x = GetX (maxY);
                if (x >= minX && x <= maxX)
                {
                    End = new Vector2 (x, maxY);
                }
            }
            else
            {
                float x = GetX (minY);
                if (x >= minX && x <= maxX)
                {
                    End = new Vector2 (x, minY);
                }
            }

            float y = GetY (maxX);
            if (y >= minY && y <= maxY)
            {
                End = new Vector2 (maxX, y);
            }
        }
    }

    public float GetX (float y)
    {
        if (Lerp == 0)
        {
            return Start.X;
        }

        return Start.X + (y - Start.Y) / Lerp;
    }

    public float GetY (float x)
    {
        if (IsInfinityLerp ())
        {
            return Start.Y;
        }

        return Start.Y + (x - Start.X) * Lerp;
    }

    public bool HasValidIntersectPoint (Edge? other)
    {
        if (other == null)
        {
            return false;
        }

        if (Lerp == other.Lerp || Start == other.Start)
        {
            return false;
        }

        Vector2 intersectPoint = GetIntersectPoint (other);

        bool isValid = true;
        if (!IsInfinityLerp ())
        {
            if (Direction == EdgeDirection.Left)
            {
                isValid &= intersectPoint.X <= Start.X;
            }
            else
            {
                isValid &= intersectPoint.X >= Start.X;
            }
        }

        if (!other.IsInfinityLerp ())
        {
            if (other.Direction == EdgeDirection.Left)
            {
                isValid &= intersectPoint.X <= other.Start.X;
            }
            else
            {
                isValid &= intersectPoint.X >= other.Start.X;
            }
        }

        return isValid;
    }

    public Vector2 GetIntersectPoint (Edge other)
    {
        if (IsInfinityLerp ())
        {
            float x = Start.X;
            float y = other.Start.Y + (x - other.Start.X) * other.Lerp;
            return new Vector2 (x, y);
        }
        else if (other.IsInfinityLerp ())
        {
            float x = other.Start.X;
            float y = Start.Y + (x - Start.X) * Lerp;
            return new Vector2 (x, y);
        }
        else
        {
            float x = (Start.Y - other.Start.Y + other.Start.X * other.Lerp - Start.X * Lerp) / (other.Lerp - Lerp);
            float y = Start.Y + (x - Start.X) * Lerp;
            return new Vector2 (x, y);
        }
    }
}
