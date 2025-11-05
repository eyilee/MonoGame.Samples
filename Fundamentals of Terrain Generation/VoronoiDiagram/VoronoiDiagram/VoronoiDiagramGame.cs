using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Samples.Library;

namespace MonoGame.Samples.VoronoiDiagram
{
    public class VoronoiDiagramGame : Core
    {
        private Texture2D? _texture;
        private VoronoiDiagram? _voronoiDiagram;

        private readonly float _stepTime = 0.06f;
        private float _nextStepTime = 0;
        private float _scale = 1f;
        private bool _isPaused = false;

        private KeyboardState _prevKeyboardState;
        private KeyboardState _currentKeyboardState;
        private MouseState _prevMouseState;
        private MouseState _currentMouseState;

        public VoronoiDiagramGame ()
            : base ("VoronoiDiagram", 1280, 720, false)
        {
            IsMouseVisible = true;
        }

        protected override void Initialize ()
        {
            base.Initialize ();
        }

        protected override void LoadContent ()
        {
            _texture = new Texture2D (GraphicsDevice, 1, 1);
            _texture.SetData ([Color.White]);

            _voronoiDiagram = new VoronoiDiagram (_texture, size: 256, pointCount: 5);
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

                _voronoiDiagram?.Reset ();
            }

            if (_prevKeyboardState.IsKeyUp (Keys.P) && _currentKeyboardState.IsKeyDown (Keys.P))
            {
                _isPaused = !_isPaused;
            }

            if (_prevKeyboardState.IsKeyUp (Keys.R) && _currentKeyboardState.IsKeyDown (Keys.R))
            {
                _nextStepTime = 0f;

                _voronoiDiagram?.Redo ();
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

                    _voronoiDiagram?.NextStep ();
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
            _voronoiDiagram?.Draw (SpriteBatch);
            SpriteBatch.End ();

            base.Draw (gameTime);
        }
    }
}
