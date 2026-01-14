using Microsoft.Xna.Framework;

namespace MonoGame.Samples.Library.Input;

public class InputManager
{
    public KeyboardListener Keyboard { get; init; }
    public MouseListener Mouse { get; init; }

    public InputManager ()
    {
        Keyboard = new KeyboardListener ();
        Mouse = new MouseListener ();
    }

    public void Update (GameTime gameTime)
    {
        Keyboard.Update (gameTime);
        Mouse.Update (gameTime);
    }
}
