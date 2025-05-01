using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace NeonShooter
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager m_Graphics;
        private SpriteBatch m_SpriteBatch;

        public static int Width { get; private set; }
        public static int Height { get; private set; }

        private PlayerShip m_PlayerShip;

        public Game1 ()
        {
            Window.Title = "Neon Shooter";

            Width = 1280;
            Height = 720;

            m_Graphics = new GraphicsDeviceManager (this)
            {
                PreferredBackBufferWidth = Width,
                PreferredBackBufferHeight = Height
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize ()
        {
            // TODO: Add your initialization logic here

            base.Initialize ();
        }

        protected override void LoadContent ()
        {
            m_SpriteBatch = new SpriteBatch (GraphicsDevice);

            Art.Load (Content, GraphicsDevice);
            Sound.Load (Content);

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play (Sound.Music);

            m_PlayerShip = new PlayerShip (Art.Player, new Vector2 (Width / 2f, Height / 2f), 0);
        }

        protected override void Update (GameTime _gameTime)
        {
            if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
                Exit ();

            // TODO: Add your update logic here
            m_PlayerShip.Update ();
            BulletManager.Update ();

            base.Update (_gameTime);
        }

        protected override void Draw (GameTime _gameTime)
        {
            GraphicsDevice.Clear (Color.CornflowerBlue);

            m_SpriteBatch.Begin ();
            m_PlayerShip.Draw (m_SpriteBatch);
            BulletManager.Draw (m_SpriteBatch);
            m_SpriteBatch.End ();

            base.Draw (_gameTime);
        }
    }
}
