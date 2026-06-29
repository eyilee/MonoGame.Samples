using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AutoPong
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager m_Graphics;
        private SpriteBatch m_SpriteBatch;

        private int m_Width = 1280;
        private int m_Height = 720;

        private Rectangle m_PaddleLeft;
        private Rectangle m_PaddleRight;
        private Rectangle m_Ball;
        private Vector2 m_BallPosition;
        private Vector2 m_BallVelocity;
        private float m_BallSpeed = 15.0f;

        private Texture2D m_Texture;

        public Game1 ()
        {
            m_Graphics = new GraphicsDeviceManager (this)
            {
                PreferredBackBufferWidth = m_Width,
                PreferredBackBufferHeight = m_Height
            };

            // 如果不是在初始化的時候就設定，還需要再呼叫
            // m_Graphics.ApplyChanges ();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize ()
        {
            base.Initialize ();
        }

        protected override void LoadContent ()
        {
            m_SpriteBatch = new SpriteBatch (GraphicsDevice);

            m_Texture = new Texture2D (m_Graphics.GraphicsDevice, 1, 1);
            m_Texture.SetData ([Color.White]);

            // 使用 Content.Load 讀取圖片
            // m_Texture = Content.Load<Texture2D> ("ball");

            m_PaddleLeft = new Rectangle (0 + 10, m_Height / 2 - 50, 20, 100);
            m_PaddleRight = new Rectangle (m_Width - 30, m_Height / 2 - 50, 20, 100);

            m_Ball = new Rectangle (m_Width / 2, m_Height / 2, 10, 10);
            m_BallPosition = new Vector2 (m_Ball.X, m_Ball.Y);
            m_BallVelocity = new Vector2 (1.0f, 0.1f);
        }

        protected override void Update (GameTime _gameTime)
        {
            if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
                Exit ();

            m_BallPosition.X += m_BallVelocity.X * m_BallSpeed;
            m_BallPosition.Y += m_BallVelocity.Y * m_BallSpeed;

            if (m_PaddleLeft.Intersects (m_Ball))
            {
                m_BallVelocity.X *= -1;
                m_BallPosition.X = m_PaddleLeft.X + m_PaddleLeft.Width;
            }

            if (m_PaddleRight.Intersects (m_Ball))
            {
                m_BallVelocity.X *= -1;
                m_BallPosition.X = m_PaddleRight.X - 10;
            }

            if (m_BallPosition.X < 0)
            {
                m_BallPosition.X = 1;
                m_BallVelocity.X *= -1;
            }
            else if (m_BallPosition.X > m_Width - 10)
            {
                m_BallPosition.X = m_Width - 11;
                m_BallVelocity.X *= -1;
            }

            if (m_BallPosition.Y < 0)
            {
                m_BallPosition.Y = 10 + 1;
                m_BallVelocity.Y *= -1;
            }
            else if (m_BallPosition.Y > m_Height - 10)
            {
                m_BallPosition.Y = m_Height - 11;
                m_BallVelocity.Y *= -1;
            }

            KeyboardState keyboardState = Keyboard.GetState ();
            if (keyboardState.IsKeyDown (Keys.Up))
            {
                m_PaddleLeft.Y -= 8;
            }
            else if (keyboardState.IsKeyDown (Keys.Down))
            {
                m_PaddleLeft.Y += 8;
            }

            int paddleCenter = m_PaddleRight.Y + m_PaddleRight.Height / 2;
            if (paddleCenter < m_BallPosition.Y - 10)
            {
                m_PaddleRight.Y -= (int)((paddleCenter - m_BallPosition.Y) * 0.1f);
            }
            else if (paddleCenter > m_BallPosition.Y + 30)
            {
                m_PaddleRight.Y += (int)((m_BallPosition.Y - paddleCenter) * 0.1f);
            }

            LimitPaddle (ref m_PaddleLeft);
            LimitPaddle (ref m_PaddleRight);

            base.Update (_gameTime);
        }

        protected override void Draw (GameTime _gameTime)
        {
            GraphicsDevice.Clear (Color.CornflowerBlue);

            m_SpriteBatch.Begin ();

            m_SpriteBatch.Draw (m_Texture, new Vector2 (m_PaddleLeft.X, m_PaddleLeft.Y), m_PaddleLeft, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.00001f);
            m_SpriteBatch.Draw (m_Texture, new Vector2 (m_PaddleRight.X, m_PaddleRight.Y), m_PaddleRight, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.00001f);

            m_Ball.X = (int)m_BallPosition.X;
            m_Ball.Y = (int)m_BallPosition.Y;
            m_SpriteBatch.Draw (m_Texture, new Vector2 (m_Ball.X, m_Ball.Y), m_Ball, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.00001f);

            m_SpriteBatch.End ();

            base.Draw (_gameTime);
        }

        private void LimitPaddle (ref Rectangle _paddle)
        {
            if (_paddle.Y < 0)
            {
                _paddle.Y = 0;
            }
            else if (_paddle.Y + _paddle.Height > m_Height)
            {
                _paddle.Y = m_Height - _paddle.Height;
            }
        }
    }
}
