using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace NeonShooter
{
    public class Enemy : Entity
    {
        static readonly Random m_Random = new ();

        IEnumerator<int> m_Behaviour;

        public Enemy (Texture2D _image, Vector2 _position, Vector2 _velocity, float _rotation)
        {
            m_Image = _image;
            m_Position = _position;
            m_Velocity = _velocity;
            m_Rotation = _rotation;
            m_Size = new Vector2 (_image.Width, _image.Height);
            m_Radius = MathF.Sqrt (m_Size.X * m_Size.X + m_Size.Y * m_Size.Y) / 2f;
        }

        public static Enemy CreateSeeker (Vector2 _position, Vector2 _velocity, float _rotation)
        {
            Enemy enemy = new (Art.Seeker, _position, _velocity, _rotation);
            enemy.m_Behaviour = enemy.ChasePlayer ();
            return enemy;
        }

        public static Enemy CreateWanderer (Vector2 _position, Vector2 _velocity, float _rotation)
        {
            Enemy enemy = new (Art.Wanderer, _position, _velocity, _rotation);
            enemy.m_Behaviour = enemy.RandomMove ();
            return enemy;
        }

        public override void Update ()
        {
            if (m_Behaviour != null)
            {
                if (!m_Behaviour.MoveNext ())
                {
                    m_Behaviour = null;
                }
            }

            m_Position += m_Velocity;

            m_Velocity *= 0.8f;
        }

        IEnumerator<int> ChasePlayer ()
        {
            while (true)
            {
                Vector2 vector = EntityManager.PlayerShip.Position - m_Position;
                vector.Normalize ();
                vector *= 0.9f;

                m_Velocity += vector;

                if (m_Velocity != Vector2.Zero)
                {
                    m_Rotation = MathF.Atan2 (m_Velocity.Y, m_Velocity.X);
                }

                yield return 0;
            }
        }

        IEnumerator<int> RandomMove ()
        {
            float direction = m_Random.NextSingle () * MathF.PI * 2;

            while (true)
            {
                Vector2 vector = new (MathF.Cos (direction), MathF.Sin (direction));
                vector *= 0.4f;

                m_Velocity += vector;

                m_Rotation += 0.05f;

                if (m_Position.X < 0 || m_Position.X > Game1.Width || m_Position.Y < 0 || m_Position.Y > Game1.Height)
                {
                    Vector2 toCenter = new (Game1.Width / 2f - m_Position.X, Game1.Height / 2f - m_Position.Y);
                    direction = MathF.Atan2 (toCenter.Y, toCenter.X) + (m_Random.NextSingle () - 0.5f) * MathF.PI / 2f;
                }

                yield return 0;
            }
        }

        public void Kill ()
        {
            m_IsExpired = true;
        }
    }
}
