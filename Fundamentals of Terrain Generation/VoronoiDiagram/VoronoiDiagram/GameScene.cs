using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Samples.Library;
using MonoGame.Samples.Library.Input;

namespace MonoGame.Samples.VoronoiDiagram;

public class GameScene : Scene
{
    private Camera? _camera;

    private VoronoiDiagram? _voronoiDiagram;

    private readonly float _stepTime = 0.06f;
    private float _nextStepTime = 0;
    private bool _isPaused = false;

    public override void LoadContent ()
    {
        _camera = new Camera (GraphicsDevice)
        {
            MinZoom = 0.1f,
            MaxZoom = 4f
        };

        _camera.LookAt (Vector2.Zero);

        _voronoiDiagram = new VoronoiDiagram (GraphicsDevice, size: 256, pointCount: 5);

        Input.Keyboard.SubscribePressed (Keys.N, Next);
        Input.Keyboard.SubscribePressed (Keys.P, Pause);
        Input.Keyboard.SubscribePressed (Keys.R, Redo);
        Input.Mouse.SubscribeDrag (MouseButtons.Left, Drag);
        Input.Mouse.SubscribeWheelMoved (WheelMoved);
    }

    public override void UnloadContent ()
    {
        Input.Keyboard.UnsubscribePressed (Keys.N, Next);
        Input.Keyboard.UnsubscribePressed (Keys.P, Pause);
        Input.Keyboard.UnsubscribePressed (Keys.R, Redo);
        Input.Mouse.UnsubscribeDrag (MouseButtons.Left, Drag);
        Input.Mouse.UnsubscribeWheelMoved (WheelMoved);
    }

    public override void Update (GameTime gameTime)
    {
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

    public override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear (Color.CornflowerBlue);

        SpriteBatch.Begin (transformMatrix: _camera?.GetViewMatrix ());
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
        _camera?.Translate (new Vector2 (-eventArgs.PositionDelta.X, -eventArgs.PositionDelta.Y));
    }

    private void WheelMoved (object? sender, MouseEventArgs eventArgs)
    {
        if (eventArgs.ScrollWheelDelta != 0)
        {
            _camera?.ZoomIn (eventArgs.ScrollWheelDelta / 1000f);
        }
    }
}
