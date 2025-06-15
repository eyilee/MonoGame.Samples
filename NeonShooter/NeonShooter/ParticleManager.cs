using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace NeonShooter
{
    public class CircularParticleArray
    {
        private readonly List<Particle> m_Particles;
        private int m_Start;
        private int m_Count;

        public int Start
        {
            get { return m_Start; }
            set { m_Start = value % m_Particles.Count; }
        }

        public int Count
        {
            get { return m_Count; }
            set { m_Count = value; }
        }

        public int Capacity { get { return m_Particles.Count; } }

        public CircularParticleArray (int _capacity)
        {
            m_Particles = new (_capacity);

            for (int i = 0; i < _capacity; i++)
            {
                m_Particles.Add (new Particle ());
            }
        }

        public Particle this[int _index]
        {
            get { return m_Particles[(m_Start + _index) % m_Particles.Count]; }
            set { m_Particles[(m_Start + _index) % m_Particles.Count] = value; }
        }

        public void Swap (int _lhs, int _rhs)
        {
            (this[_rhs], this[_lhs]) = (this[_lhs], this[_rhs]);
        }
    }

    public static class ParticleManager
    {
        private static readonly CircularParticleArray m_Particles = new (1024 * 20);

        public static void Update ()
        {
            int removalCount = 0;

            for (int i = 0; i < m_Particles.Count; i++)
            {
                Particle particle = m_Particles[i];

                particle.Update ();

                particle.NormalizeTime += 1f / particle.Duration;
                if (particle.NormalizeTime >= 1)
                {
                    m_Particles.Swap (removalCount, i);

                    removalCount++;
                }
            }

            m_Particles.Start += removalCount;
            m_Particles.Count -= removalCount;
        }

        public static void Draw (SpriteBatch _spriteBatch)
        {
            for (int i = 0; i < m_Particles.Count; i++)
            {
                Particle particle = m_Particles[i];
                _spriteBatch.Draw (particle.Texture, particle.Position, null, particle.Tint, particle.Rotation, particle.Origin, particle.Scale, SpriteEffects.None, 0);
            }
        }

        public static void CreateParticle (Texture2D _texture, Vector2 _position, Vector2 _velocity, Color _tint, Vector2 _scale, float _duration)
        {
            Particle particle;
            if (m_Particles.Count == m_Particles.Capacity)
            {
                particle = m_Particles[0];
                m_Particles.Start++;
            }
            else
            {
                particle = m_Particles[m_Particles.Count];
                m_Particles.Count++;
            }

            particle.Texture = _texture;
            particle.Position = _position;
            particle.Velocity = _velocity;
            particle.Tint = _tint;
            particle.Rotation = MathF.Atan2 (_velocity.Y, _velocity.X);
            particle.Origin = new Vector2 (_texture.Width / 2f, _texture.Height / 2f);
            particle.Scale = _scale;
            particle.Duration = _duration;
            particle.NormalizeTime = 0f;
        }
    }
}
