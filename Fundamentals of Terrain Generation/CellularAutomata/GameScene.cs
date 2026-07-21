using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Library;
using MonoGame.Library.Input;

namespace CellularAutomata;

public class GameScene : Scene
{
    private CellularAutomata _cellularAutomata = null!;

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
        _cellularAutomata = new CellularAutomata (GraphicsDevice, 128, 128, 4, 0.3f);

        base.LoadContent ();
    }

    public override void UnloadContent ()
    {
        _cellularAutomata.Dispose ();

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

            _cellularAutomata.NextStep ();
        }

        base.Update (gameTime);
    }

    public override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear (Color.CornflowerBlue);

        _cellularAutomata.Draw (Render);

        base.Draw (gameTime);
    }

    private void NextMap (object? sender, KeyboardEventArgs eventArgs)
    {
        _nextStepTime = 0f;

        _cellularAutomata.Reset ();
    }

    private void Redo (object? sender, KeyboardEventArgs eventArgs)
    {
        _nextStepTime = 0f;

        _cellularAutomata.Redo ();
    }
}
