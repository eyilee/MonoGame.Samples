using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace VoronoiDiagram
{
    public class Core : Game
    {
        internal static Core s_instance;

        public static Core Instance => s_instance;

        public static GraphicsDeviceManager Graphics { get; private set; }

        public static new GraphicsDevice GraphicsDevice { get; private set; }

        public static SpriteBatch SpriteBatch { get; private set; }

        public static new ContentManager Content { get; private set; }

        private VoronoiDiagram _voronoiDiagram;

        private readonly float _stepTime = 0.06f;
        private float _nextStepTime = 0;
        private float _scale = 1f;
        private bool _isPaused = false;

        private KeyboardState _prevKeyboardState;
        private KeyboardState _currentKeyboardState;
        private MouseState _prevMouseState;
        private MouseState _currentMouseState;

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

            _voronoiDiagram = new VoronoiDiagram (size: 256, pointCount: 5);
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

            _prevKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState ();
            _prevMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState ();

            if (_prevKeyboardState.IsKeyUp (Keys.N) && _currentKeyboardState.IsKeyDown (Keys.N))
            {
                _nextStepTime = 0f;

                _voronoiDiagram.Reset ();
            }

            if (_prevKeyboardState.IsKeyUp (Keys.P) && _currentKeyboardState.IsKeyDown (Keys.P))
            {
                _isPaused = !_isPaused;
            }

            if (_prevKeyboardState.IsKeyUp (Keys.R) && _currentKeyboardState.IsKeyDown (Keys.R))
            {
                _nextStepTime = 0f;

                _voronoiDiagram.Redo ();
            }

            float scrollWheel = _currentMouseState.ScrollWheelValue - _prevMouseState.ScrollWheelValue;
            if (scrollWheel != 0)
            {
                _scale += scrollWheel / 1000f;
            }

            if (!_isPaused)
            {
                _nextStepTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_nextStepTime >= _stepTime)
                {
                    _nextStepTime -= _stepTime;

                    _voronoiDiagram.NextStep ();
                }
            }

            base.Update (gameTime);
        }

        protected override void Draw (GameTime gameTime)
        {
            GraphicsDevice.Clear (Color.CornflowerBlue);

            int halfWidth = GraphicsDevice.PresentationParameters.BackBufferWidth / 2;
            int halfHeight = GraphicsDevice.PresentationParameters.BackBufferHeight / 2;
            Matrix transformMatrix = Matrix.CreateScale (_scale, _scale, 1f)
                * Matrix.CreateTranslation (halfWidth, halfHeight, 0f);

            SpriteBatch.Begin (transformMatrix: transformMatrix);
            _voronoiDiagram.Draw (SpriteBatch);
            SpriteBatch.End ();

            base.Draw (gameTime);
        }
    }
}
