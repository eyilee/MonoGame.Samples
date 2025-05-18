using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace NeonShooter
{
    public class PlayerShip : Entity
    {
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

            m_Position += direction * 8;
            m_Position.X = float.Clamp (m_Position.X, Size.X / 2, (Game1.Width - Size.X / 2));
            m_Position.Y = float.Clamp (m_Position.Y, Size.Y / 2, (Game1.Height - Size.Y / 2));

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
    }
}
