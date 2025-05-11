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

        public static int Width { get; private set; }
        public static int Height { get; private set; }

        private PlayerShip m_PlayerShip;
        public PlayerShip PlayerShip { get { return m_PlayerShip; } }

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
            HandleCollision ();

            m_PlayerShip.Update ();
            EnemySpawner.Update ();
            EnemyManager.Update ();
            BulletManager.Update ();

            base.Update (_gameTime);
        }

        protected override void Draw (GameTime _gameTime)
        {
            GraphicsDevice.Clear (Color.CornflowerBlue);

            m_SpriteBatch.Begin ();
            m_PlayerShip.Draw (m_SpriteBatch);
            EnemyManager.Draw (m_SpriteBatch);
            BulletManager.Draw (m_SpriteBatch);
            m_SpriteBatch.End ();

            base.Draw (_gameTime);
        }

        void HandleCollision ()
        {
            foreach (Enemy enemy in EnemyManager.EnemyList)
            {
                if (!enemy.IsExpired)
                {
                    float radius = enemy.Radius + m_PlayerShip.Radius;
                    if (Vector2.DistanceSquared (enemy.Position, m_PlayerShip.Position) < radius * radius)
                    {
                        m_PlayerShip.Kill ();
                    }
                }
            }

            foreach (Enemy enemy in EnemyManager.EnemyList)
            {
                foreach (Bullet bullet in BulletManager.BulletList)
                {
                    if (!enemy.IsExpired && !bullet.IsExpired)
                    {
                        float radius = enemy.Radius + bullet.Radius;
                        if (Vector2.DistanceSquared (enemy.Position, bullet.Position) < radius * radius)
                        {
                            enemy.Kill ();
                            bullet.Kill ();
                        }
                    }
                }
            }
        }

        public void Reset ()
        {
            m_PlayerShip.Reset ();

            foreach (Enemy enemy in EnemyManager.EnemyList)
            {
                enemy.Kill ();
            }

            foreach (Bullet bullet in BulletManager.BulletList)
            {
                bullet.Kill ();
            }
        }
    }
}
