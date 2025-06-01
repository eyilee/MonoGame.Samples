using BloomPostprocess;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace NeonShooter
{
    public class Game1 : Game
    {
        public static Game1 Instance { get; private set; }

        private GraphicsDeviceManager m_Graphics;
        private SpriteBatch m_SpriteBatch;
        private BloomComponent m_BloomComponent;

        public static int Width { get; private set; }
        public static int Height { get; private set; }

        public Game1 ()
        {
            Instance = this;

            Window.Title = "Neon Shooter";

            Width = 1280;
            Height = 720;

            m_Graphics = new GraphicsDeviceManager (this)
            {
                PreferredBackBufferWidth = Width,
                PreferredBackBufferHeight = Height
            };

            m_BloomComponent = new BloomComponent (this);
            Components.Add (m_BloomComponent);
            m_BloomComponent.Settings = new BloomSettings (null, 0.25f, 4, 2, 1, 1.5f, 1);
            m_BloomComponent.Visible = true;

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

            EntityManager.AddEntity (new PlayerShip (Art.Player, new Vector2 (Width / 2f, Height / 2f), 0));
        }

        protected override void Update (GameTime _gameTime)
        {
            if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
                Exit ();

            // TODO: Add your update logic here
            EntityManager.Update ();
            EnemySpawner.Update ();

            base.Update (_gameTime);
        }

        protected override void Draw (GameTime _gameTime)
        {
            m_BloomComponent.BeginDraw ();

            GraphicsDevice.Clear (Color.Black);

            m_SpriteBatch.Begin ();
            EntityManager.Draw (m_SpriteBatch);
            m_SpriteBatch.End ();

            base.Draw (_gameTime);
        }
    }
}
