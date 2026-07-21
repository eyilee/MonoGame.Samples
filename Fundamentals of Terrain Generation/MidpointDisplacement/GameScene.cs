using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Library;
using MonoGame.Library.Input;

namespace MidpointDisplacement;

public class GameScene : Scene
{
    private MidpointDisplacement _midpointDisplacement = null!;

    private readonly float _stepTime = 1f / 4f;

    private float _nextStepTime = 0f;

    public override void Initialize ()
    {
        Input.Keyboard.SubscribePressed (Keys.N, NextMap);

        base.Initialize ();
    }

    public override void LoadContent ()
    {
        _midpointDisplacement = new MidpointDisplacement (GraphicsDevice, 6, 8, 1f, 0.75f);

        base.LoadContent ();
    }

    public override void UnloadContent ()
    {
        _midpointDisplacement.Dispose ();

        base.UnloadContent ();
    }

    protected override void Dispose (bool disposing)
    {
        if (disposing)
        {
            Input.Keyboard.UnsubscribePressed (Keys.N, NextMap);
        }

        base.Dispose (disposing);
    }

    public override void Update (GameTime gameTime)
    {
        _nextStepTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_nextStepTime >= _stepTime)
        {
            _nextStepTime -= _stepTime;

            _midpointDisplacement.NextStep ();
        }

        base.Update (gameTime);
    }

    public override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear (Color.CornflowerBlue);

        _midpointDisplacement.Draw (Render);

        base.Draw (gameTime);
    }

    private void NextMap (object? sender, KeyboardEventArgs eventArgs)
    {
        _nextStepTime = 0f;

        _midpointDisplacement.Reset ();
    }
}
