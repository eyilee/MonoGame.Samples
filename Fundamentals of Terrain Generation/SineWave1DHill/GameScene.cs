using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Library;
using MonoGame.Library.Graphics;
using MonoGame.Library.Input;

namespace SineWave1DHill;

public class GameScene : Scene
{
    private Texture2D _pixelTexture = null!;

    private Texture2DResource _pixel = null!;

    private SineWave1DHill _sineWave1DHill = null!;

    private readonly float _stepTime = 1f / 4f;

    private float _nextStepTime = 0f;

    public override void Initialize ()
    {
        Input.Keyboard.SubscribePressed (Keys.N, NextMap);

        base.Initialize ();
    }

    public override void LoadContent ()
    {
        _pixelTexture = new Texture2D (GraphicsDevice, 1, 1);
        _pixelTexture.SetData ([Color.White]);

        _pixel = new Texture2DResource ("Pixel", _pixelTexture);

        _sineWave1DHill = new SineWave1DHill (128, 4, 512, 6);

        base.LoadContent ();
    }

    public override void UnloadContent ()
    {
        _pixel.Dispose ();
        _pixelTexture.Dispose ();

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

            _sineWave1DHill.NextStep ();
        }

        base.Update (gameTime);
    }

    public override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear (Color.CornflowerBlue);

        _sineWave1DHill.Draw (Render);

        base.Draw (gameTime);
    }

    private void NextMap (object? sender, KeyboardEventArgs eventArgs)
    {
        _nextStepTime = 0f;

        _sineWave1DHill.Reset ();
    }
}
