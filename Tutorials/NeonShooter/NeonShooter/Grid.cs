using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace NeonShooter
{
    public class Grid
    {
        class Point
        {
            public Vector3 Position;
            public Vector3 Velocity;
            public Vector3 Acceleration;
            public float Damping;
            public float Mass;

            public Point (Vector3 _position, float _mass)
            {
                Position = _position;
                Damping = 0.98f;
                Mass = _mass;
            }

            public void ApplyForce (Vector3 _force)
            {
                Acceleration += _force * Mass;
            }

            public void Update ()
            {
                Velocity += Acceleration;
                Position += Velocity;

                Acceleration = Vector3.Zero;

                if (Velocity.LengthSquared () < 0.001f * 0.001f)
                {
                    Velocity = Vector3.Zero;
                }

                Velocity *= Damping;
            }
        }

        class Spring
        {
            public Point End1;
            public Point End2;
            public float TargetLength;
            public float SpringRate;
            public float Damping;

            public Spring (Point _end1, Point _end2)
            {
                End1 = _end1;
                End2 = _end2;
                TargetLength = Vector3.Distance (End1.Position, End2.Position);
                SpringRate = 0.1f;
                Damping = 0.1f;
            }

            public void Update ()
            {
                Vector3 vector = End1.Position - End2.Position;

                float length = vector.Length ();
                if (length <= TargetLength)
                {
                    return;
                }

                vector = (vector / length) * (length - TargetLength);
                Vector3 diffVelocity = End1.Velocity - End2.Velocity;
                Vector3 force = SpringRate * vector + diffVelocity * Damping;

                End1.ApplyForce (-force);
                End2.ApplyForce (force);
            }
        }

        readonly Point[,] m_Points;
        readonly Spring[] m_Springs;

        public Grid (int _spacing)
        {
            int columns = Game1.Width / _spacing + 1;
            int rows = Game1.Height / _spacing + 1;
            m_Points = new Point[columns, rows];

            Point[,] fixPoints = new Point[columns, rows];
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    m_Points[x, y] = new Point (new Vector3 (x * _spacing, y * _spacing, 0), 1);
                    fixPoints[x, y] = new Point (new Vector3 (x * _spacing, y * _spacing, 0), 0);
                }
            }

            List<Spring> springs = [];
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (x == 0 || x == columns - 1 || y == 0 || y == rows - 1)
                    {
                        springs.Add (new Spring (m_Points[x, y], fixPoints[x, y]));
                    }

                    if (x > 0)
                    {
                        springs.Add (new Spring (m_Points[x - 1, y], m_Points[x, y]));
                    }

                    if (y > 0)
                    {
                        springs.Add (new Spring (m_Points[x, y - 1], m_Points[x, y]));
                    }
                }
            }

            m_Springs = springs.ToArray ();
        }

        public void ApplyForce (float _force, Vector2 _position, float _radius)
        {
            ApplyForce (_force, new Vector3 (_position, 0), _radius);
        }

        public void ApplyForce (float _force, Vector3 _position, float _radius)
        {
            foreach (Point point in m_Points)
            {
                float dist2 = Vector3.DistanceSquared (_position, point.Position);
                if (dist2 < _radius * _radius)
                {
                    point.ApplyForce (10 * _force * (_position - point.Position) / (100 + dist2));
                }
            }
        }

        public void Update ()
        {
            foreach (Spring spring in m_Springs)
            {
                spring.Update ();
            }

            foreach (Point point in m_Points)
            {
                point.Update ();
            }
        }

        public void Draw (SpriteBatch _spriteBatch)
        {
            int columns = m_Points.GetLength (0);
            int rows = m_Points.GetLength (1);
            Color color = new (30, 30, 139, 85);

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (x > 0)
                    {
                        DrawLine (_spriteBatch, ToVec2 (m_Points[x - 1, y].Position), ToVec2 (m_Points[x, y].Position), color);
                    }

                    if (y > 0)
                    {
                        DrawLine (_spriteBatch, ToVec2 (m_Points[x, y - 1].Position), ToVec2 (m_Points[x, y].Position), color);
                    }
                }
            }
        }

        public static Vector2 ToVec2 (Vector3 _vector3)
        {
            float factor = (_vector3.Z + 2000) / 2000;
            return (new Vector2 (_vector3.X - Game1.Width / 2f, _vector3.Y - Game1.Height / 2f)) * factor + new Vector2 (Game1.Width / 2f, Game1.Height / 2f);
        }

        public static void DrawLine (SpriteBatch _spriteBatch, Vector2 _start, Vector2 _end, Color _color, float _thickness = 2f)
        {
            Vector2 delta = _end - _start;
            float rotation = MathF.Atan2 (delta.Y, delta.X);
            _spriteBatch.Draw (Art.Pixel, _start, null, _color, rotation, new Vector2 (0, 0.5f), new Vector2 (delta.Length (), _thickness), SpriteEffects.None, 0f);
        }
    }
}
