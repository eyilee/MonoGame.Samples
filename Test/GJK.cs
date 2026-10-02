using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Test
{
    public class GJK
    {
        public static bool Intersects (List<Vector2> polygonA, List<Vector2> polygonB)
        {
            Vector2 direction = polygonB[0] - polygonA[0];
            if (direction.LengthSquared () < float.Epsilon)
            {
                direction = Vector2.UnitX;
            }

            List<Vector2> simplex = [Support (polygonA, polygonB, direction)];

            direction = -simplex[0];

            while (true)
            {
                if (direction.LengthSquared () < float.Epsilon)
                {
                    return true;
                }

                Vector2 newPoint = Support (polygonA, polygonB, direction);
                if (Vector2.Dot (newPoint, direction) <= 0)
                {
                    return false;
                }
                simplex.Add (newPoint);
                if (HandleSimplex (simplex, ref direction))
                {
                    return true;
                }
            }
        }

        private static bool HandleSimplex (List<Vector2> simplex, ref Vector2 direction)
        {
            if (simplex.Count == 2)
            {
                Vector2 a = simplex[1];
                Vector2 b = simplex[0];
                Vector2 ab = b - a;
                Vector2 ao = -a;
                direction = TripleProduct (ab, ao, ab);
            }
            else if (simplex.Count == 3)
            {
                Vector2 a = simplex[2];
                Vector2 b = simplex[1];
                Vector2 c = simplex[0];
                Vector2 ab = b - a;
                Vector2 ac = c - a;
                Vector2 ao = -a;
                Vector2 abPerp = TripleProduct (ac, ab, ab);
                Vector2 acPerp = TripleProduct (ab, ac, ac);
                if (Vector2.Dot (abPerp, ao) > 0)
                {
                    simplex.RemoveAt (0);
                    direction = abPerp;
                }
                else if (Vector2.Dot (acPerp, ao) > 0)
                {
                    simplex.RemoveAt (1);
                    direction = acPerp;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private static Vector2 TripleProduct (Vector2 a, Vector2 b, Vector2 c)
        {
            float ac = a.X * b.Y - a.Y * b.X;
            float bc = c.X * b.Y - c.Y * b.X;
            return new Vector2 (c.X * ac - a.X * bc, c.Y * ac - a.Y * bc);
        }

        private static Vector2 TripleProduct2 (Vector2 a, Vector2 b, Vector2 c)
        {
            float cross = a.X * b.Y - a.Y * b.X;

            return new Vector2 (-cross * c.Y, cross * c.X);
        }

        public static Vector2 Support (List<Vector2> polygonA, List<Vector2> polygonB, Vector2 direction)
        {
            Vector2 pointA = GetFarthestPoint (polygonA, direction);
            Vector2 pointB = GetFarthestPoint (polygonB, -direction);
            return pointA - pointB;
        }

        private static Vector2 GetFarthestPoint (List<Vector2> polygon, Vector2 direction)
        {
            float maxDot = float.NegativeInfinity;
            Vector2 farthestPoint = Vector2.Zero;

            foreach (Vector2 vertex in polygon)
            {
                float dot = Vector2.Dot (vertex, direction);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    farthestPoint = vertex;
                }
            }

            return farthestPoint;
        }
    }

    public class EPA
    {
        public static bool GetContactPoint (List<Vector2> polygonA, List<Vector2> polygonB, List<Vector2> simplex, out Vector2 contactPoint, out Vector2 normal, out float penetration)
        {
            contactPoint = Vector2.Zero;
            normal = Vector2.Zero;
            penetration = 0f;
            List<Edge> edges = new List<Edge> ();
            for (int i = 0; i < simplex.Count; i++)
            {
                Vector2 a = simplex[i];
                Vector2 b = simplex[(i + 1) % simplex.Count];
                edges.Add (new Edge (a, b));
            }
            while (true)
            {
                Edge closestEdge = FindClosestEdge (edges);
                Vector2 supportPoint = GJK.Support (polygonA, polygonB, closestEdge.Normal);
                float distance = Vector2.Dot (supportPoint - closestEdge.A, closestEdge.Normal);
                if (distance - closestEdge.Distance < 0.001f)
                {
                    contactPoint = supportPoint;
                    normal = closestEdge.Normal;
                    penetration = distance;
                    return true;
                }
                edges.Insert (edges.IndexOf (closestEdge), new Edge (closestEdge.A, supportPoint));
                edges.Insert (edges.IndexOf (closestEdge) + 1, new Edge (supportPoint, closestEdge.B));
                edges.Remove (closestEdge);
            }
        }

        private static Edge FindClosestEdge (List<Edge> edges)
        {
            Edge closestEdge = edges[0];
            float minDistance = closestEdge.Distance;
            foreach (Edge edge in edges)
            {
                if (edge.Distance < minDistance)
                {
                    minDistance = edge.Distance;
                    closestEdge = edge;
                }
            }
            return closestEdge;
        }
    }

    internal class Edge
    {
        public Vector2 A { get; }
        public Vector2 B { get; }
        public Vector2 Normal { get; }
        public float Distance { get; }
        public Edge (Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
            Vector2 edge = b - a;
            Normal = new Vector2 (-edge.Y, edge.X);
            Normal.Normalize ();
            Distance = Vector2.Dot (Normal, a);
        }
    }
}