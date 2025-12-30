using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGame.Samples.Library.Input
{
    public static class Input
    {
        private static readonly KeyboardListener _keyboardListener = new ();

        public static event EventHandler<KeyboardEventArgs>? KeyPressed
        {
            add => _keyboardListener.KeyPressed += value;
            remove => _keyboardListener.KeyPressed -= value;
        }

        public static event EventHandler<KeyboardEventArgs>? KeyReleased
        {
            add => _keyboardListener.KeyReleased += value;
            remove => _keyboardListener.KeyReleased -= value;
        }

        public static bool IsKeyDown (Keys key) => _keyboardListener.IsKeyDown (key);

        public static bool IsKeyUp (Keys key) => _keyboardListener.IsKeyUp (key);

        public static void Update (GameTime gameTime)
        {
            _keyboardListener.Update (gameTime);
        }

        public static void SubscribeKeyPressed (Keys key, EventHandler<KeyboardEventArgs> handler) => _keyboardListener.SubscribeKeyPressed (key, handler);

        public static void UnsubscribeKeyPressed (Keys key, EventHandler<KeyboardEventArgs> handler) => _keyboardListener.UnsubscribeKeyPressed (key, handler);

        public static void SubscribeKeyReleased (Keys key, EventHandler<KeyboardEventArgs> handler) => _keyboardListener.SubscribeKeyReleased (key, handler);

        public static void UnsubscribeKeyReleased (Keys key, EventHandler<KeyboardEventArgs> handler) => _keyboardListener.UnsubscribeKeyReleased (key, handler);
    }
}
