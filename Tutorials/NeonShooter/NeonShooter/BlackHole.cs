using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace NeonShooter
{
    public class BlackHole : Entity
    {
        private readonly Random m_Random = new ();

        private const int m_CooldownFrames = 15;
        private int m_CooldownRemaining = 0;
        private bool m_IsSpray = true;

        float m_SprayRotation = 0f;

        public BlackHole (Texture2D _image, Vector2 _position, Vector2 _velocity, float _rotation)
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
            IEnumerable<Entity> entities = EntityManager.GetNearbyEntities (Position, 250f);

            foreach (Entity entity in entities)
            {
                if (entity is Bullet)
                {
                    Vector2 velocity = entity.Position - Position;
                    velocity.Normalize ();
                    velocity *= 0.3f;
                    entity.Velocity += velocity;
                }
                else
                {
                    Vector2 velocity = Position - entity.Position;
                    float distance = velocity.Length ();
                    velocity.Normalize ();
                    velocity *= float.Lerp (2f, 0f, distance / 250f);
                    entity.Velocity += velocity;
                }
            }

            if (m_CooldownRemaining == 0)
            {
                m_CooldownRemaining = m_CooldownFrames;
                m_IsSpray = !m_IsSpray;
            }

            if (m_CooldownRemaining > 0)
            {
                m_CooldownRemaining--;
            }

            m_SprayRotation -= MathF.PI * 2f / 50f;

            if (m_IsSpray)
            {
                float length = 12f + m_Random.NextSingle () * 3f;
                Vector2 velocity = length * new Vector2 ((float)Math.Cos (m_SprayRotation), (float)Math.Sin (m_SprayRotation));
                Color color = ColorUtil.HSVToColor (5f, 0.5f, 0.8f);
                Vector2 offset = Vector2.One * (4f + m_Random.NextSingle () * 4f);
                Vector2 position = Position + 2f * new Vector2 (velocity.Y, -velocity.X) + offset;
                ParticleManager.CreateParticle (Art.LineParticle, position, velocity, color, new Vector2 (1.5f, 1.5f), 190f);
            }
        }

        public override void Draw (SpriteBatch _spriteBatch)
        {
            double time = Game1.Instance.GameTime.TotalGameTime.TotalSeconds;
            m_Scale = 1 + 0.1f * (float)Math.Sin (time * 10);
            _spriteBatch.Draw (m_Image, m_Position, null, m_Color, m_Rotation, m_Size / 2f, m_Scale, SpriteEffects.None, 0f);
        }

        public void Kill ()
        {
            m_IsExpired = true;
        }
    }
}
