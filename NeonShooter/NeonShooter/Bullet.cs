using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace NeonShooter
{
    public class Bullet : Entity
    {
        static readonly Random m_Random = new ();

        public Bullet (Texture2D _image, Vector2 _position, Vector2 _velocity, float _rotation)
        {
            m_Image = _image;
            m_Position = _position;
            m_Velocity = _velocity;
            m_Rotation = _rotation;
            m_Size = new Vector2 (_image.Width, _image.Height);
            m_Radius = MathF.Sqrt (m_Size.X * m_Size.X + m_Size.Y * m_Size.Y) / 2f;
        }

        public override void Update ()
        {
            m_Position += m_Velocity;

            if (m_Position.X < 0 || m_Position.X > Game1.Width || m_Position.Y < 0 || m_Position.Y > Game1.Height)
            {
                Kill ();
            }
        }

        public void Kill ()
        {
            m_IsExpired = true;

            for (int i = 0; i < 30; i++)
            {
                double theta = m_Random.NextDouble () * 2 * Math.PI;
                float speed = m_Random.NextSingle () * 9f + 1f;
                Vector2 velocity = new (speed * (float)Math.Cos (theta), speed * (float)Math.Sin (theta));
                ParticleManager.CreateParticle (Art.LineParticle, Position, velocity, Color.LightBlue, Vector2.One, 50f);
            }
        }
    }
}
