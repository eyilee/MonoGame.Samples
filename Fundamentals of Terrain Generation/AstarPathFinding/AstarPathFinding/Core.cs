using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace AstarPathFinding
{
    public class Core : Game
    {
        internal static Core s_instance;

        public static Core Instance => s_instance;

        public static GraphicsDeviceManager Graphics { get; private set; }

        public static new GraphicsDevice GraphicsDevice { get; private set; }

        public static SpriteBatch SpriteBatch { get; private set; }

        public static new ContentManager Content { get; private set; }

        public static SpriteFont Font { get; private set; }

        private AstarPathFinding _astarPathFinding;

        private float _stepTime = 0.25f;
        private float _nextStepTime = 0;

        private KeyboardState _prevKeyboardState;
        private KeyboardState _currentKeyboardState;

        public Core ()
        {
            if (s_instance != null)
            {
                throw new InvalidOperationException ($"Only a single Core instance can be created");
            }

            s_instance = this;

            Graphics = new GraphicsDeviceManager (this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720
            };

            Content = base.Content;
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        protected override void Initialize ()
        {
            base.Initialize ();

            GraphicsDevice = base.GraphicsDevice;

            SpriteBatch = new SpriteBatch (GraphicsDevice);

            _astarPathFinding = new AstarPathFinding (size: 16, aliveRate: 0.8f);
        }

        protected override void LoadContent ()
        {
            Font = Content.Load<SpriteFont> ("DefaultFont");
        }

        protected override void Update (GameTime gameTime)
        {
            if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
            {
                Exit ();
            }

            _nextStepTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            _prevKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState ();

            if (_prevKeyboardState.IsKeyUp (Keys.N) && _currentKeyboardState.IsKeyDown (Keys.N))
            {
                _nextStepTime = 0f;

                _astarPathFinding.Reset ();
            }

            if (_nextStepTime >= _stepTime)
            {
                _nextStepTime -= _stepTime;

                _astarPathFinding.NextStep ();
            }

            base.Update (gameTime);
        }

        protected override void Draw (GameTime gameTime)
        {
            GraphicsDevice.Clear (Color.CornflowerBlue);

            int halfWidth = GraphicsDevice.PresentationParameters.BackBufferWidth / 2;
            int halfHeight = GraphicsDevice.PresentationParameters.BackBufferHeight / 2;
            Matrix transformMatrix = Matrix.CreateTranslation (halfWidth, halfHeight, 0.0f);

            SpriteBatch.Begin (transformMatrix: transformMatrix);
            _astarPathFinding.Draw (SpriteBatch);
            SpriteBatch.End ();

            base.Draw (gameTime);
        }
    }
}
