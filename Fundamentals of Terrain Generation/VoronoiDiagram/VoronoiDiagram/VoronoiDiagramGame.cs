using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Samples.Library;
using MonoGame.Samples.Library.Input;
using System;

namespace MonoGame.Samples.VoronoiDiagram
{
    public class VoronoiDiagramGame : Core
    {
        private Texture2D? _texture;
        private VoronoiDiagram? _voronoiDiagram;

        private readonly float _stepTime = 0.06f;
        private float _nextStepTime = 0;
        private int _translationX = 0;
        private int _translationY = 0;
        private float _scale = 1f;
        private bool _isPaused = false;

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

            Input.SubscribeKeyPressed (Keys.N, Next);
            Input.SubscribeKeyPressed (Keys.P, Pause);
            Input.SubscribeKeyPressed (Keys.R, Redo);
            Input.SubscribeDrag (MouseButtons.Left, Drag);
            Input.SubscribeWheelMoved (WheelMoved);
        }

        protected override void Update (GameTime gameTime)
        {
            if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
            {
                Exit ();
            }

            Input.Update (gameTime);

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
                * Matrix.CreateTranslation (halfWidth + _translationX, halfHeight + _translationY, 0f);

            SpriteBatch.Begin (transformMatrix: transformMatrix);
            _voronoiDiagram?.Draw (SpriteBatch);
            SpriteBatch.End ();

            base.Draw (gameTime);
        }

        private void Next (object? sender, KeyboardEventArgs eventArgs)
        {
            _nextStepTime = 0f;

            _voronoiDiagram?.Reset ();
        }

        private void Pause (object? sender, KeyboardEventArgs eventArgs)
        {
            _isPaused = !_isPaused;
        }

        private void Redo (object? sender, KeyboardEventArgs eventArgs)
        {
            _nextStepTime = 0f;

            _voronoiDiagram?.Redo ();
        }

        private void Drag (object? sender, MouseEventArgs eventArgs)
        {
            _translationX = Math.Clamp (_translationX + eventArgs.PositionDelta.X, -50, 50);
            _translationY = Math.Clamp (_translationY + eventArgs.PositionDelta.Y, -50, 50);
        }

        private void WheelMoved (object? sender, MouseEventArgs eventArgs)
        {
            if (eventArgs.ScrollWheelDelta != 0)
            {
                _scale = Math.Clamp (_scale + eventArgs.ScrollWheelDelta / 1000f, 0.1f, 4f);
            }
        }
    }
}
