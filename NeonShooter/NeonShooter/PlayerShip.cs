﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace NeonShooter
{
    class PlayerShip
    {
        private Texture2D m_Image;
        private Vector2 m_Position = Vector2.Zero;
        private Color m_Color = Color.White;
        private float m_Rotation = 0f;
        private Vector2 m_Size = Vector2.Zero;
        private float m_Scale = 1f;

        public Vector2 Position { get { return m_Position; } }
        public float Rotation { get { return m_Rotation; } }
        public Vector2 Size { get { return m_Size; } }

        public PlayerShip (Texture2D _image, Vector2 _position, float _rotation)
        {
            m_Image = _image;
            m_Position = _position;
            m_Rotation = _rotation;
            m_Size = new Vector2 (_image.Width, _image.Height);
        }

        public void Update ()
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
        }

        public void Draw (SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw (m_Image, m_Position, null, m_Color, m_Rotation, m_Size / 2f, m_Scale, SpriteEffects.None, 0f);
        }
    }
}
