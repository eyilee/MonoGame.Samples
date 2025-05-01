using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeonShooter
{
    class Bullet
    {
        private Texture2D m_Image;
        private Vector2 m_Position = Vector2.Zero;
        private Vector2 m_Velocity = Vector2.Zero;
        private Color m_Color = Color.White;
        private float m_Rotation = 0f;
        private Vector2 m_Size = Vector2.Zero;
        private float m_Scale = 1f;
        private bool m_IsExpired = false;

        public Vector2 Position { get { return m_Position; } }
        public float Rotation { get { return m_Rotation; } }
        public Vector2 Size { get { return m_Size; } }
        public bool IsExpired { get { return m_IsExpired; } }

        public Bullet (Texture2D _image, Vector2 _position, Vector2 _velocity, float _rotation)
        {
            m_Image = _image;
            m_Position = _position;
            m_Velocity = _velocity;
            m_Rotation = _rotation;
            m_Size = new Vector2 (_image.Width, _image.Height);
        }

        public void Update ()
        {
            m_Position += m_Velocity;

            if (m_Position.X < 0 || m_Position.X > Game1.Width || m_Position.Y < 0 || m_Position.Y > Game1.Height)
            {
                m_IsExpired = true;
            }
        }

        public void Draw (SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw (m_Image, m_Position, null, m_Color, m_Rotation, m_Size / 2f, m_Scale, SpriteEffects.None, 0f);
        }
    }
}
