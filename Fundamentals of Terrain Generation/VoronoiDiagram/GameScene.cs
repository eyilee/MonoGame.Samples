using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Library;
using MonoGame.Library.Input;

namespace VoronoiDiagram;

public class GameScene : Scene
{
    private VoronoiDiagram _voronoiDiagram = null!;

    private readonly float _stepTime = 1f / 60f;

    private float _nextStepTime = 0f;

    private bool _isPaused = false;

    public override void Initialize ()
    {
        Input.Keyboard.SubscribePressed (Keys.N, Next);
        Input.Keyboard.SubscribePressed (Keys.P, Pause);
        Input.Keyboard.SubscribePressed (Keys.R, Redo);

        base.Initialize ();
    }

    public override void LoadContent ()
    {
        _voronoiDiagram = new VoronoiDiagram (512, 16);

        base.LoadContent ();
    }

    public override void UnloadContent ()
    {
        base.UnloadContent ();
    }

    protected override void Dispose (bool disposing)
    {
        if (disposing)
        {
            Input.Keyboard.UnsubscribePressed (Keys.N, Next);
            Input.Keyboard.UnsubscribePressed (Keys.P, Pause);
            Input.Keyboard.UnsubscribePressed (Keys.R, Redo);
        }

        base.Dispose (disposing);
    }

    public override void Update (GameTime gameTime)
    {
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

    public override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear (Color.CornflowerBlue);

        _voronoiDiagram.Draw (Render);

        base.Draw (gameTime);
    }

    private void Next (object? sender, KeyboardEventArgs eventArgs)
    {
        _nextStepTime = 0f;

        _voronoiDiagram.Reset ();
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
