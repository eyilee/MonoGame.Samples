using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;

namespace MonoGame.Samples.Library.Input
{
    /// <summary>
    /// Specifies keyboard modifier keys that can be combined to indicate the state of Control, Shift, and Alt keys.
    /// </summary>
    [Flags]
    public enum KeyboardModifiers
    {
        /// <summary>
        /// Indicates that no options are set.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies the Control modifier key.
        /// </summary>
        Control = 1,

        /// <summary>
        /// Specifies the Shift modifier key.
        /// </summary>
        Shift = 2,

        /// <summary>
        /// Specifies the Alt modifier key.
        /// </summary>
        Alt = 4,
    }

    /// <summary>
    /// Provides data for keyboard-related events, including information about the key pressed and any active modifier
    /// keys.
    /// </summary>
    public class KeyboardEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the keyboard key associated with this instance.
        /// </summary>
        public Keys Key { get; init; }

        /// <summary>
        /// Gets the set of keyboard modifier keys (such as Shift, Ctrl, or Alt) that are active.
        /// </summary>
        public KeyboardModifiers Modifiers { get; init; }

        /// <summary>
        /// Initializes a new instance of the KeyboardEventArgs class with the specified key and keyboard state.
        /// </summary>
        /// <param name="key">The key that triggered the keyboard event.</param>
        /// <param name="keyboardState">The current state of the keyboard, used to determine which modifier keys are pressed.</param>
        public KeyboardEventArgs (Keys key, KeyboardState keyboardState)
        {
            Key = key;

            Modifiers = KeyboardModifiers.None;

            if (keyboardState.IsKeyDown (Keys.LeftControl) || keyboardState.IsKeyDown (Keys.RightControl))
            {
                Modifiers |= KeyboardModifiers.Control;
            }

            if (keyboardState.IsKeyDown (Keys.LeftShift) || keyboardState.IsKeyDown (Keys.RightShift))
            {
                Modifiers |= KeyboardModifiers.Shift;
            }

            if (keyboardState.IsKeyDown (Keys.LeftAlt) || keyboardState.IsKeyDown (Keys.RightAlt))
            {
                Modifiers |= KeyboardModifiers.Alt;
            }
        }
    }

    public class KeyboardObserver
    {
        public event EventHandler<KeyboardEventArgs>? Observers;

        public bool HasObservers => Observers != null && Observers.GetInvocationList ().Length > 0;

        public void Notify (object? sender, KeyboardEventArgs eventArgs) => Observers?.Invoke (sender, eventArgs);
    }

    /// <summary>
    /// Listens for keyboard input and raises events when keys are pressed or released while the control has focus.
    /// </summary>
    /// <remarks>Call the Update method once per game update cycle to ensure that keyboard events are detected
    /// and handled correctly. The KeyboardListener raises the KeyPressed and KeyReleased events in response to changes
    /// in keyboard state. Failing to call Update regularly may result in missed input events.</remarks>
    public class KeyboardListener
    {
        private readonly IEnumerable<Keys> _keys = Enum.GetValues (typeof (Keys)).Cast<Keys> ();

        private KeyboardState _currentState;
        private KeyboardState _previousState;

        private readonly Dictionary<Keys, KeyboardObserver> _pressedObservers = [];
        private readonly Dictionary<Keys, KeyboardObserver> _releasedObservers = [];

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        public event EventHandler<KeyboardEventArgs>? KeyPressed;

        /// <summary>
        /// Occurs when a key is released while the control has focus.
        /// </summary>
        public event EventHandler<KeyboardEventArgs>? KeyReleased;

        /// <summary>
        /// Determines whether the specified key is currently pressed.
        /// </summary>
        /// <param name="key">The key to check for a pressed state.</param>
        /// <returns>true if the specified key is currently pressed; otherwise, false.</returns>
        public bool IsKeyDown (Keys key) => _currentState.IsKeyDown (key);

        /// <summary>
        /// Determines whether the specified key is currently in the up (not pressed) state.
        /// </summary>
        /// <param name="key">The key to check for the up state.</param>
        /// <returns>true if the specified key is not pressed; otherwise, false.</returns>
        public bool IsKeyUp (Keys key) => _currentState.IsKeyUp (key);

        /// <summary>
        /// Updates the keyboard input state and raises events for key presses and releases since the last update.
        /// </summary>
        /// <remarks>Call this method once per game loop iteration to ensure that keyboard input events
        /// are processed accurately. Failing to call this method regularly may result in missed or delayed input
        /// events.</remarks>
        /// <param name="gameTime">The current game time, typically used to synchronize input updates with the game loop.</param>
        public void Update (GameTime gameTime)
        {
            _currentState = Keyboard.GetState ();

            RaisePressedEvents (_currentState);
            RaiseReleasedEvents (_currentState);

            _previousState = _currentState;
        }

        private void RaisePressedEvents (KeyboardState currentState)
        {
            IEnumerable<Keys> pressedKeys = _keys.Where (key => currentState.IsKeyDown (key) && _previousState.IsKeyUp (key));

            foreach (Keys key in pressedKeys)
            {
                KeyboardEventArgs eventArgs = new (key, currentState);

                KeyPressed?.Invoke (this, eventArgs);

                if (_pressedObservers.TryGetValue (key, out KeyboardObserver? value))
                {
                    value.Notify (this, eventArgs);
                }
            }
        }

        private void RaiseReleasedEvents (KeyboardState currentState)
        {
            IEnumerable<Keys> releasedKeys = _keys.Where (key => currentState.IsKeyUp (key) && _previousState.IsKeyDown (key));

            foreach (Keys key in releasedKeys)
            {
                KeyboardEventArgs eventArgs = new (key, currentState);

                KeyReleased?.Invoke (this, eventArgs);

                if (_releasedObservers.TryGetValue (key, out KeyboardObserver? value))
                {
                    value.Notify (this, eventArgs);
                }
            }
        }

        public void SubscribeKeyPressed (Keys key, EventHandler<KeyboardEventArgs> handler) => Subscribe (_pressedObservers, key, handler);

        public void UnsubscribeKeyPressed (Keys key, EventHandler<KeyboardEventArgs> handler) => Unsubscribe (_pressedObservers, key, handler);

        public void SubscribeKeyReleased (Keys key, EventHandler<KeyboardEventArgs> handler) => Subscribe (_releasedObservers, key, handler);

        public void UnsubscribeKeyReleased (Keys key, EventHandler<KeyboardEventArgs> handler) => Unsubscribe (_releasedObservers, key, handler);

        private static void Subscribe (Dictionary<Keys, KeyboardObserver> observers, Keys key, EventHandler<KeyboardEventArgs> handler)
        {
            if (!observers.TryGetValue (key, out KeyboardObserver? value))
            {
                value = new KeyboardObserver ();

                observers[key] = value;
            }

            value.Observers += handler;
        }

        private static void Unsubscribe (Dictionary<Keys, KeyboardObserver> observers, Keys key, EventHandler<KeyboardEventArgs> handler)
        {
            if (observers.TryGetValue (key, out KeyboardObserver? value))
            {
                value.Observers -= handler;

                if (!value.HasObservers)
                {
                    observers.Remove (key);
                }
            }
        }
    }
}
