using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeonShooter
{
    public abstract class Entity
    {
        protected Texture2D m_Image;
        protected Vector2 m_Position = Vector2.Zero;
        protected Vector2 m_Velocity = Vector2.Zero;
        protected Color m_Color = Color.White;
        protected float m_Rotation = 0f;
        protected Vector2 m_Size = Vector2.Zero;
        protected float m_Radius = 0f;
        protected float m_Scale = 1f;
        protected bool m_IsExpired = false;

        public Vector2 Position { get { return m_Position; } }
        public float Rotation { get { return m_Rotation; } }
        public Vector2 Size { get { return m_Size; } }
        public float Radius { get { return m_Radius; } }
        public bool IsExpired { get { return m_IsExpired; } }

        public abstract void Update ();

        public virtual void Draw (SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw (m_Image, m_Position, null, m_Color, m_Rotation, m_Size / 2f, m_Scale, SpriteEffects.None, 0f);
        }
    }
}
