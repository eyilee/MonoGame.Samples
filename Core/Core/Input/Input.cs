using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGame.Samples.Library.Input
{
    public static class Input
    {
        private static readonly KeyboardListener _keyboardListener = new ();

        public static bool IsKeyDown (Keys key) => _keyboardListener.IsKeyDown (key);

        public static bool IsKeyUp (Keys key) => _keyboardListener.IsKeyUp (key);

        public static void SubscribeKeyPressed (EventHandler<KeyboardEventArgs> handler) => _keyboardListener.SubscribePressed (handler);

        public static void UnsubscribeKeyPressed (EventHandler<KeyboardEventArgs> handler) => _keyboardListener.UnsubscribePressed (handler);

        public static void SubscribeKeyReleased (EventHandler<KeyboardEventArgs> handler) => _keyboardListener.SubscribeReleased (handler);

        public static void UnsubscribeKeyReleased (EventHandler<KeyboardEventArgs> handler) => _keyboardListener.UnsubscribeReleased (handler);

        public static void SubscribeKeyPressed (Keys key, EventHandler<KeyboardEventArgs> handler) => _keyboardListener.SubscribePressed (key, handler);

        public static void UnsubscribeKeyPressed (Keys key, EventHandler<KeyboardEventArgs> handler) => _keyboardListener.UnsubscribePressed (key, handler);

        public static void SubscribeKeyReleased (Keys key, EventHandler<KeyboardEventArgs> handler) => _keyboardListener.SubscribeReleased (key, handler);

        public static void UnsubscribeKeyReleased (Keys key, EventHandler<KeyboardEventArgs> handler) => _keyboardListener.UnsubscribeReleased (key, handler);

        private static readonly MouseListener _mouseListener = new ();

        public static bool IsMouseDown (MouseButtons button) => _mouseListener.IsButtonDown (button);

        public static bool IsMouseUp (MouseButtons button) => _mouseListener.IsButtonUp (button);

        public static void SubscribeMousePressed (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribePressed (button, handler);

        public static void UnsubscribeMousePressed (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribePressed (button, handler);

        public static void SubscribeMouseReleased (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribeReleased (button, handler);

        public static void UnsubscribeMouseReleased (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribeReleased (button, handler);

        public static void SubscribeMouseClicked (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribeClicked (button, handler);

        public static void UnsubscribeMouseClicked (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribeClicked (button, handler);

        public static void SubscribeDoubleClicked (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribeDoubleClicked (button, handler);

        public static void UnsubscribeDoubleClicked (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribeDoubleClicked (button, handler);

        public static void SubscribeMouseMoved (EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribeMoved (handler);

        public static void UnsubscribeMouseMoved (EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribeMoved (handler);

        public static void SubscribeDragBegin (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribeDragBegin (button, handler);

        public static void UnsubscribeDragBegin (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribeDragBegin (button, handler);

        public static void SubscribeDrag (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribeDrag (button, handler);

        public static void UnsubscribeDrag (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribeDrag (button, handler);

        public static void SubscribeDragEnd (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribeDragEnd (button, handler);

        public static void UnsubscribeDragEnd (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribeDragEnd (button, handler);

        public static void SubscribeWheelMoved (EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribeWheelMoved (handler);

        public static void UnsubscribeWheelMoved (EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribeWheelMoved (handler);

        public static void Update (GameTime gameTime)
        {
            _keyboardListener.Update (gameTime);
            _mouseListener.Update (gameTime);
        }
    }
}
