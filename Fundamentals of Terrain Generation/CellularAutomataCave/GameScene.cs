using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Library;
using MonoGame.Library.Input;

namespace CellularAutomataCave;

public class GameScene : Scene
{
    private CellularAutomataCave _cellularAutomataCave = null!;

    private readonly float _stepTime = 1f / 4f;

    private float _nextStepTime = 0f;

    public override void Initialize ()
    {
        Input.Keyboard.SubscribePressed (Keys.N, NextMap);
        Input.Keyboard.SubscribePressed (Keys.R, Redo);

        base.Initialize ();
    }

    public override void LoadContent ()
    {
        _cellularAutomataCave = new CellularAutomataCave (GraphicsDevice, 128, 128, 4, 0.38f, 8);

        base.LoadContent ();
    }

    public override void UnloadContent ()
    {
        _cellularAutomataCave.Dispose ();

        base.UnloadContent ();
    }

    protected override void Dispose (bool disposing)
    {
        if (disposing)
        {
            Input.Keyboard.UnsubscribePressed (Keys.N, NextMap);
            Input.Keyboard.UnsubscribePressed (Keys.R, Redo);
        }

        base.Dispose (disposing);
    }

    public override void Update (GameTime gameTime)
    {
        _nextStepTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_nextStepTime >= _stepTime)
        {
            _nextStepTime -= _stepTime;

            _cellularAutomataCave.NextStep ();
        }

        base.Update (gameTime);
    }

    public override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear (Color.CornflowerBlue);

        _cellularAutomataCave.Draw (Render);

        base.Draw (gameTime);
    }

    private void NextMap (object? sender, KeyboardEventArgs eventArgs)
    {
        _nextStepTime = 0f;

        _cellularAutomataCave.Reset ();
    }

    private void Redo (object? sender, KeyboardEventArgs eventArgs)
    {
        _nextStepTime = 0f;

        _cellularAutomataCave.Redo ();
    }
}
