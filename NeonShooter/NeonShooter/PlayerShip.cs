using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace NeonShooter
{
    public class PlayerShip : Entity
    {
        private readonly Random m_Random = new ();

        private const int m_CooldownFrames = 6;
        private int m_CooldownRemaining = 0;

        public PlayerShip (Texture2D _image, Vector2 _position, float _rotation)
        {
            m_Image = _image;
            m_Position = _position;
            m_Rotation = _rotation;
            m_Size = new Vector2 (_image.Width, _image.Height);
            m_Radius = MathF.Sqrt (m_Size.X * m_Size.X + m_Size.Y * m_Size.Y) / 2f;
        }

        public override void Update ()
        {
            KeyboardState keyboardState = Keyboard.GetState ();

            Vector2 direction = Vector2.Zero;
            if (keyboardState.IsKeyDown (Keys.A))
            {
                direction.X -= 1f;
            }

            if (keyboardState.IsKeyDown (Keys.D))
            {
                direction.X += 1f;
            }

            if (keyboardState.IsKeyDown (Keys.W))
            {
                direction.Y -= 1f;
            }

            if (keyboardState.IsKeyDown (Keys.S))
            {
                direction.Y += 1f;
            }

            if (direction != Vector2.Zero && direction.LengthSquared () != 1f)
            {
                direction.Normalize ();
            }

            m_Velocity += direction * 8;
            m_Position += m_Velocity;
            m_Velocity = Vector2.Zero;
            m_Position.X = float.Clamp (m_Position.X, Size.X / 2, (Game1.Width - Size.X / 2));
            m_Position.Y = float.Clamp (m_Position.Y, Size.Y / 2, (Game1.Height - Size.Y / 2));

            CreateExhaustFire ();

            MouseState mouseState = Mouse.GetState ();

            Vector2 aimDirection = new Vector2 (mouseState.X, mouseState.Y) - m_Position;
            if (aimDirection != Vector2.Zero && aimDirection.LengthSquared () != 1f)
            {
                aimDirection.Normalize ();
            }

            m_Rotation = (float)Math.Atan2 (aimDirection.Y, aimDirection.X);

            if (m_CooldownRemaining == 0)
            {
                Quaternion aimQuaternion = Quaternion.CreateFromYawPitchRoll (0, 0, m_Rotation);
                Vector2 velocity = 11f * new Vector2 ((float)Math.Cos (m_Rotation), (float)Math.Sin (m_Rotation));
                EntityManager.AddEntity (new Bullet (Art.Bullet, m_Position + Vector2.Transform (new Vector2 (35, -8), aimQuaternion), velocity, m_Rotation));
                EntityManager.AddEntity (new Bullet (Art.Bullet, m_Position + Vector2.Transform (new Vector2 (35, 8), aimQuaternion), velocity, m_Rotation));

                m_CooldownRemaining = m_CooldownFrames;
            }

            if (m_CooldownRemaining > 0)
            {
                m_CooldownRemaining--;
            }
        }

        public void Kill ()
        {
            EntityManager.Reset ();
        }

        public void Reset ()
        {
            m_Position = new Vector2 (Game1.Width / 2f, Game1.Height / 2f);
            m_Rotation = 0f;
            m_CooldownRemaining = 0;
        }

        private void CreateExhaustFire ()
        {
            if (m_Velocity.LengthSquared () > 0f)
            {
                float rotation = MathF.Atan2 (m_Velocity.Y, m_Velocity.X);
                Quaternion quaternion = Quaternion.CreateFromYawPitchRoll (0f, 0f, rotation);
                Vector2 position = Position + Vector2.Transform (new Vector2 (-25, 0), quaternion);

                Vector2 baseVelocity = m_Velocity;
                baseVelocity.Normalize ();
                baseVelocity *= -3;

                const float alpha = 0.7f;

                double theta = m_Random.NextDouble () * 2 * Math.PI;
                float length = m_Random.NextSingle ();
                Vector2 middleVelocity = baseVelocity + length * new Vector2 ((float)Math.Cos (theta), (float)Math.Sin (theta));
                ParticleManager.CreateParticle (Art.LineParticle, position, middleVelocity, Color.White * alpha, new Vector2 (0.5f, 1), 60f);
                ParticleManager.CreateParticle (Art.Glow, position, middleVelocity, new Color (255, 187, 30) * alpha, new Vector2 (0.5f, 1), 60f);

                double time = Game1.Instance.GameTime.TotalGameTime.TotalSeconds;
                Vector2 perpVel = new Vector2 (baseVelocity.Y, -baseVelocity.X) * (0.6f * (float)Math.Sin (time * 10d));

                theta = m_Random.NextDouble () * 2 * Math.PI;
                length = m_Random.NextSingle () * 0.3f;
                Vector2 sideVelocity1 = baseVelocity + perpVel + length * new Vector2 ((float)Math.Cos (theta), (float)Math.Sin (theta));
                Vector2 sideVelocity2 = baseVelocity - perpVel + length * new Vector2 ((float)Math.Cos (theta), (float)Math.Sin (theta));
                ParticleManager.CreateParticle (Art.LineParticle, position, sideVelocity1, Color.White * alpha, new Vector2 (0.5f, 1), 60f);
                ParticleManager.CreateParticle (Art.LineParticle, position, sideVelocity2, Color.White * alpha, new Vector2 (0.5f, 1), 60f);
                ParticleManager.CreateParticle (Art.Glow, position, sideVelocity1, new Color (200, 38, 9) * alpha, new Vector2 (0.5f, 1), 60f);
                ParticleManager.CreateParticle (Art.Glow, position, sideVelocity2, new Color (200, 38, 9) * alpha, new Vector2 (0.5f, 1), 60f);
            }
        }
    }
}
