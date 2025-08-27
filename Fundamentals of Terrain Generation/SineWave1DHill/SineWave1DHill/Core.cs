using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SineWave1DHill
{
    public class Core : Game
    {
        internal static Core s_instance;

        public static Core Instance => s_instance;

        public static GraphicsDeviceManager Graphics { get; private set; }

        public static new GraphicsDevice GraphicsDevice { get; private set; }

        public static SpriteBatch SpriteBatch { get; private set; }

        public static new ContentManager Content { get; private set; }

        private SineWave1DHill _sineWave1DHill;

        private readonly float _stepTime = 0.6f;
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

            _sineWave1DHill = new SineWave1DHill (size: 1024, iteration: 8);
        }

        protected override void LoadContent ()
        {
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

                _sineWave1DHill.Reset ();
            }

            if (_nextStepTime >= _stepTime)
            {
                _nextStepTime -= _stepTime;

                _sineWave1DHill.NextStep ();
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
            _sineWave1DHill.Draw (SpriteBatch);
            SpriteBatch.End ();

            base.Draw (gameTime);
        }
    }
}
