using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Library;
using MonoGame.Library.Graphics;
using MonoGame.Library.Input;

namespace AstarPathFinding;

public class GameScene : Scene
{
    private Texture2D _pixelTexture = null!;

    private Texture2DResource _pixel = null!;

    private FontResource _font = null!;

    private AstarPathFinding _astarPathFinding = null!;

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
        _pixelTexture = new Texture2D (GraphicsDevice, 1, 1);
        _pixelTexture.SetData ([Color.White]);

        _pixel = new Texture2DResource ("Pixel", _pixelTexture);

        _font = new FontResource ("Font", Content.Load<SpriteFont> ("Font"));

        _astarPathFinding = new AstarPathFinding (16, 16, 32, 0.8f);

        base.LoadContent ();
    }

    public override void UnloadContent ()
    {
        _font.Dispose ();
        _pixel.Dispose ();
        _pixelTexture.Dispose ();

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

            _astarPathFinding.NextStep ();
        }

        base.Update (gameTime);
    }

    public override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear (Color.CornflowerBlue);

        _astarPathFinding.Draw (Render);

        base.Draw (gameTime);
    }

    private void NextMap (object? sender, KeyboardEventArgs eventArgs)
    {
        _nextStepTime = 0f;

        _astarPathFinding.Reset ();
    }

    private void Redo (object? sender, KeyboardEventArgs eventArgs)
    {
        _nextStepTime = 0f;

        _astarPathFinding.Redo ();
    }
}
