using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
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

    internal class KeyboardObserver
    {
        public event EventHandler<KeyboardEventArgs>? Observers;

        public bool HasObservers => Observers != null && Observers.GetInvocationList ().Length > 0;

        public void Notify (object? sender, KeyboardEventArgs eventArgs) => Observers?.Invoke (sender, eventArgs);
    }

    internal class KeyboardListener
    {
        private readonly IEnumerable<Keys> _keys = Enum.GetValues (typeof (Keys)).Cast<Keys> ();

        private KeyboardState _currentState;
        private KeyboardState _previousState;

        public bool IsKeyDown (Keys key) => _currentState.IsKeyDown (key);

        public bool IsKeyUp (Keys key) => _currentState.IsKeyUp (key);

        private readonly KeyboardObserver _pressedObserver = new ();
        private readonly KeyboardObserver _releasedObserver = new ();
        private readonly Dictionary<Keys, KeyboardObserver> _pressedObservers = [];
        private readonly Dictionary<Keys, KeyboardObserver> _releasedObservers = [];

        public void SubscribePressed (EventHandler<KeyboardEventArgs> handler) => _pressedObserver.Observers += handler;

        public void UnsubscribePressed (EventHandler<KeyboardEventArgs> handler) => _pressedObserver.Observers -= handler;

        public void SubscribeReleased (EventHandler<KeyboardEventArgs> handler) => _releasedObserver.Observers += handler;

        public void UnsubscribeReleased (EventHandler<KeyboardEventArgs> handler) => _releasedObserver.Observers -= handler;

        public void SubscribePressed (Keys key, EventHandler<KeyboardEventArgs> handler) => Subscribe (_pressedObservers, key, handler);

        public void UnsubscribePressed (Keys key, EventHandler<KeyboardEventArgs> handler) => Unsubscribe (_pressedObservers, key, handler);

        public void SubscribeReleased (Keys key, EventHandler<KeyboardEventArgs> handler) => Subscribe (_releasedObservers, key, handler);

        public void UnsubscribeReleased (Keys key, EventHandler<KeyboardEventArgs> handler) => Unsubscribe (_releasedObservers, key, handler);

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

        private static void Notify (Dictionary<Keys, KeyboardObserver> observers, Keys key, object? sender, KeyboardEventArgs eventArgs)
        {
            if (observers.TryGetValue (key, out KeyboardObserver? value))
            {
                value.Notify (sender, eventArgs);
            }
        }

        public void Update (GameTime gameTime)
        {
            _currentState = Keyboard.GetState ();

            RaisePressedEvents ();
            RaiseReleasedEvents ();

            _previousState = _currentState;
        }

        private void RaisePressedEvents ()
        {
            IEnumerable<Keys> pressedKeys = _keys.Where (key => _currentState.IsKeyDown (key) && _previousState.IsKeyUp (key));

            foreach (Keys key in pressedKeys)
            {
                KeyboardEventArgs eventArgs = new (key, _currentState);
                _pressedObserver.Notify (this, eventArgs);

                Notify (_pressedObservers, key, this, eventArgs);
            }
        }

        private void RaiseReleasedEvents ()
        {
            IEnumerable<Keys> releasedKeys = _keys.Where (key => _currentState.IsKeyUp (key) && _previousState.IsKeyDown (key));

            foreach (Keys key in releasedKeys)
            {
                KeyboardEventArgs eventArgs = new (key, _currentState);
                _releasedObserver.Notify (this, eventArgs);

                Notify (_releasedObservers, key, this, eventArgs);
            }
        }
    }
}
