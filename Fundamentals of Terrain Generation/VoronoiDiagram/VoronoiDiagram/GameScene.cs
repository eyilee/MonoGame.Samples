using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Samples.Library;
using MonoGame.Samples.Library.Input;

namespace MonoGame.Samples.VoronoiDiagram;

public class GameScene : Scene
{
    private VoronoiDiagram? _voronoiDiagram;

    private readonly float _stepTime = 1f / 60f;
    private float _nextStepTime = 0;
    private bool _isPaused = false;

    public override void LoadContent ()
    {
        _voronoiDiagram = new VoronoiDiagram (GraphicsDevice, size: 512, pointCount: 10);

        Input.Keyboard.SubscribePressed (Keys.N, Next);
        Input.Keyboard.SubscribePressed (Keys.P, Pause);
        Input.Keyboard.SubscribePressed (Keys.R, Redo);
    }

    public override void UnloadContent ()
    {
        Input.Keyboard.UnsubscribePressed (Keys.N, Next);
        Input.Keyboard.UnsubscribePressed (Keys.P, Pause);
        Input.Keyboard.UnsubscribePressed (Keys.R, Redo);
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

        _voronoiDiagram?.Draw ();

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
}
