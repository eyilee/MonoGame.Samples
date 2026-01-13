using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGame.Samples.Library.Input
{
    /// <summary>
    /// Provides static methods for querying keyboard and mouse input states and subscribing to input events.
    /// </summary>
    public static class Input
    {
        private static readonly KeyboardListener _keyboardListener = new ();

        /// <summary>
        /// Determines whether the specified key is currently pressed.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>true if the key is pressed; otherwise, false.</returns>
        public static bool IsKeyDown (Keys key) => _keyboardListener.IsKeyDown (key);

        /// <summary>
        /// Determines whether the specified key is currently released.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>true if the key is up; otherwise, false.</returns>
        public static bool IsKeyUp (Keys key) => _keyboardListener.IsKeyUp (key);

        /// <summary>
        /// Subscribes to keyboard key press events with the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to invoke when a key is pressed.</param>
        public static void SubscribeKeyPressed (EventHandler<KeyboardEventArgs> handler) => _keyboardListener.SubscribePressed (handler);

        /// <summary>
        /// Unsubscribes the specified event handler from keyboard key pressed events.
        /// </summary>
        /// <param name="handler">The event handler to remove from the key pressed event notifications.</param>
        public static void UnsubscribeKeyPressed (EventHandler<KeyboardEventArgs> handler) => _keyboardListener.UnsubscribePressed (handler);

        /// <summary>
        /// Subscribes to the event triggered when a keyboard key is released.
        /// </summary>
        /// <param name="handler">The event handler to invoke when a key is released.</param>
        public static void SubscribeKeyReleased (EventHandler<KeyboardEventArgs> handler) => _keyboardListener.SubscribeReleased (handler);

        /// <summary>
        /// Unsubscribes the specified event handler from keyboard key released events.
        /// </summary>
        /// <param name="handler">The event handler to remove from the key released event notifications.</param>
        public static void UnsubscribeKeyReleased (EventHandler<KeyboardEventArgs> handler) => _keyboardListener.UnsubscribeReleased (handler);

        /// <summary>
        /// Subscribes a handler to be invoked when the specified key is pressed.
        /// </summary>
        /// <param name="key">The key to listen for.</param>
        /// <param name="handler">The event handler to invoke when the key is pressed.</param>
        public static void SubscribeKeyPressed (Keys key, EventHandler<KeyboardEventArgs> handler) => _keyboardListener.SubscribePressed (key, handler);

        /// <summary>
        /// Removes a handler for the specified key press event.
        /// </summary>
        /// <param name="key">The key to unsubscribe from.</param>
        /// <param name="handler">The event handler to remove.</param>
        public static void UnsubscribeKeyPressed (Keys key, EventHandler<KeyboardEventArgs> handler) => _keyboardListener.UnsubscribePressed (key, handler);

        /// <summary>
        /// Subscribes a handler to the event triggered when the specified key is released.
        /// </summary>
        /// <param name="key">The key to listen for release events.</param>
        /// <param name="handler">The event handler to invoke when the key is released.</param>
        public static void SubscribeKeyReleased (Keys key, EventHandler<KeyboardEventArgs> handler) => _keyboardListener.SubscribeReleased (key, handler);

        /// <summary>
        /// Unsubscribes a handler from the key released event for the specified key.
        /// </summary>
        /// <param name="key">The key to unsubscribe from the released event.</param>
        /// <param name="handler">The event handler to remove.</param>
        public static void UnsubscribeKeyReleased (Keys key, EventHandler<KeyboardEventArgs> handler) => _keyboardListener.UnsubscribeReleased (key, handler);

        private static readonly MouseListener _mouseListener = new ();

        /// <summary>
        /// Determines whether the specified mouse button is currently pressed.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>true if the specified mouse button is pressed; otherwise, false.</returns>
        public static bool IsMouseDown (MouseButtons button) => _mouseListener.IsButtonDown (button);

        /// <summary>
        /// Determines whether the specified mouse button is currently released.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>true if the specified mouse button is up; otherwise, false.</returns>
        public static bool IsMouseUp (MouseButtons button) => _mouseListener.IsButtonUp (button);

        /// <summary>
        /// Subscribes a handler to the mouse pressed event for the specified mouse button.
        /// </summary>
        /// <param name="button">The mouse button to listen for.</param>
        /// <param name="handler">The event handler to invoke when the mouse button is pressed.</param>
        public static void SubscribeMousePressed (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribePressed (button, handler);

        /// <summary>
        /// Unsubscribes a handler from mouse pressed events for the specified mouse button.
        /// </summary>
        /// <param name="button">The mouse button to unsubscribe from.</param>
        /// <param name="handler">The event handler to remove.</param>
        public static void UnsubscribeMousePressed (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribePressed (button, handler);

        /// <summary>
        /// Subscribes a handler to the mouse released event for the specified mouse button.
        /// </summary>
        /// <param name="button">The mouse button to listen for release events.</param>
        /// <param name="handler">The event handler to invoke when the mouse button is released.</param>
        public static void SubscribeMouseReleased (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribeReleased (button, handler);

        /// <summary>
        /// Unsubscribes a handler from the mouse released event for the specified mouse button.
        /// </summary>
        /// <param name="button">The mouse button to unsubscribe from.</param>
        /// <param name="handler">The event handler to remove.</param>
        public static void UnsubscribeMouseReleased (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribeReleased (button, handler);

        /// <summary>
        /// Subscribes a handler to mouse click events for the specified mouse button.
        /// </summary>
        /// <param name="button">The mouse button to listen for click events.</param>
        /// <param name="handler">The event handler to invoke when the specified mouse button is clicked.</param>
        public static void SubscribeMouseClicked (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribeClicked (button, handler);

        /// <summary>
        /// Unsubscribes a handler from mouse click events for the specified mouse button.
        /// </summary>
        /// <param name="button">The mouse button to unsubscribe from.</param>
        /// <param name="handler">The event handler to remove.</param>
        public static void UnsubscribeMouseClicked (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribeClicked (button, handler);

        /// <summary>
        /// Subscribes a handler to the double-click event for the specified mouse button.
        /// </summary>
        /// <param name="button">The mouse button to monitor for double-click events.</param>
        /// <param name="handler">The event handler to invoke when a double-click is detected.</param>
        public static void SubscribeDoubleClicked (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribeDoubleClicked (button, handler);

        /// <summary>
        /// Unsubscribes a handler from the double-click event for the specified mouse button.
        /// </summary>
        /// <param name="button">The mouse button to unsubscribe from double-click events.</param>
        /// <param name="handler">The event handler to remove.</param>
        public static void UnsubscribeDoubleClicked (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribeDoubleClicked (button, handler);

        /// <summary>
        /// Subscribes a handler to mouse movement events.
        /// </summary>
        /// <param name="handler">The event handler to invoke when the mouse is moved.</param>
        public static void SubscribeMouseMoved (EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribeMoved (handler);

        /// <summary>
        /// Unsubscribes the specified event handler from mouse movement events.
        /// </summary>
        /// <param name="handler">The event handler to remove from the mouse moved event notifications.</param>
        public static void UnsubscribeMouseMoved (EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribeMoved (handler);

        /// <summary>
        /// Subscribes a handler to the drag begin event for the specified mouse button.
        /// </summary>
        /// <param name="button">The mouse button to monitor for drag begin events.</param>
        /// <param name="handler">The event handler to invoke when a drag begin event occurs.</param>
        public static void SubscribeDragBegin (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribeDragBegin (button, handler);

        /// <summary>
        /// Removes a handler for the drag begin event associated with the specified mouse button.
        /// </summary>
        /// <param name="button">The mouse button for which to unsubscribe the drag begin event.</param>
        /// <param name="handler">The event handler to remove from the drag begin event.</param>
        public static void UnsubscribeDragBegin (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribeDragBegin (button, handler);

        /// <summary>
        /// Subscribes a handler to mouse drag events for the specified mouse button.
        /// </summary>
        /// <param name="button">The mouse button to monitor for drag events.</param>
        /// <param name="handler">The event handler to invoke when a drag event occurs.</param>
        public static void SubscribeDrag (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribeDrag (button, handler);

        /// <summary>
        /// Removes a handler for drag events associated with the specified mouse button.
        /// </summary>
        /// <param name="button">The mouse button for which to remove the drag event handler.</param>
        /// <param name="handler">The event handler to remove from the drag event.</param>
        public static void UnsubscribeDrag (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribeDrag (button, handler);

        /// <summary>
        /// Subscribes a handler to the drag end event for the specified mouse button.
        /// </summary>
        /// <param name="button">The mouse button to monitor for drag end events.</param>
        /// <param name="handler">The event handler to invoke when the drag end event occurs.</param>
        public static void SubscribeDragEnd (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribeDragEnd (button, handler);

        /// <summary>
        /// Removes a handler for the drag end event associated with the specified mouse button.
        /// </summary>
        /// <param name="button">The mouse button for which to unsubscribe the drag end event handler.</param>
        /// <param name="handler">The event handler to remove from the drag end event.</param>
        public static void UnsubscribeDragEnd (MouseButtons button, EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribeDragEnd (button, handler);

        /// <summary>
        /// Subscribes a handler to mouse wheel movement events.
        /// </summary>
        /// <param name="handler">The event handler to invoke when a mouse wheel movement occurs.</param>
        public static void SubscribeWheelMoved (EventHandler<MouseEventArgs> handler) => _mouseListener.SubscribeWheelMoved (handler);

        /// <summary>
        /// Removes a handler from the mouse wheel moved event subscription.
        /// </summary>
        /// <param name="handler">The event handler to remove.</param>
        public static void UnsubscribeWheelMoved (EventHandler<MouseEventArgs> handler) => _mouseListener.UnsubscribeWheelMoved (handler);

        /// <summary>
        /// Updates the keyboard and mouse listeners with the delta game time.
        /// </summary>
        /// <param name="gameTime">The delta game time used for updating input listeners.</param>
        /// <remarks>This method needs to be called in every game loop, calling it multiple times will result in an error state.</remarks>
        public static void Update (GameTime gameTime)
        {
            _keyboardListener.Update (gameTime);
            _mouseListener.Update (gameTime);
        }
    }
}
