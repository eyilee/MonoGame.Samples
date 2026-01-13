using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Samples.Library.Input
{
    /// <summary>
    /// Specifies constants that define mouse buttons.
    /// </summary>
    public enum MouseButtons
    {
        /// <summary>
        /// Indicates that no options or values are set.
        /// </summary>
        None,

        /// <summary>
        /// Specifies the left mouse button.
        /// </summary>
        Left,

        /// <summary>
        /// Specifies the middle mouse button.
        /// </summary>
        Middle,

        /// <summary>
        /// Specifies the right mouse button.
        /// </summary>
        Right,

        /// <summary>
        /// Represents the first extended mouse button (typically XButton1).
        /// </summary>
        XButton1,

        /// <summary>
        /// Represents the second extended mouse button (typically 'XButton2').
        /// </summary>
        XButton2,
    }

    /// <summary>
    /// Provides data for mouse-related events, including button state, position, movement delta, and scroll wheel
    /// changes.
    /// </summary>
    public class MouseEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the mouse button associated with the event.
        /// </summary>
        public MouseButtons Button { get; init; }

        /// <summary>
        /// Gets the position as a Point.
        /// </summary>
        public Point Position { get; init; }

        /// <summary>
        /// Gets the change in position as a Point.
        /// </summary>
        public Point PositionDelta { get; init; }

        /// <summary>
        /// Gets the amount the scroll wheel has moved.
        /// </summary>
        public int ScrollWheelDelta { get; init; }

        /// <summary>
        /// Initializes a new instance of the MouseEventArgs class with the specified mouse button and mouse states.
        /// </summary>
        /// <param name="button">The mouse button associated with the event.</param>
        /// <param name="currentState">The current state of the mouse.</param>
        /// <param name="previousState">The previous state of the mouse.</param>
        public MouseEventArgs (MouseButtons button, MouseState currentState, MouseState previousState)
        {
            Button = button;

            Position = new Point (currentState.X, currentState.Y);

            PositionDelta = new Point (currentState.X - previousState.X, currentState.Y - previousState.Y);

            ScrollWheelDelta = currentState.ScrollWheelValue - previousState.ScrollWheelValue;
        }
    }

    internal class MouseObserver
    {
        public event EventHandler<MouseEventArgs>? Observers;

        public bool HasObservers => Observers != null && Observers.GetInvocationList ().Length > 0;

        public void Notify (object? sender, MouseEventArgs eventArgs) => Observers?.Invoke (sender, eventArgs);
    }

    internal class ExtendedButtonState
    {
        public Point PressedPosition { get; private set; } = Point.Zero;

        public TimeSpan ReleasedTime { get; private set; } = TimeSpan.Zero;

        public bool IsDoubleClick { get; private set; } = false;

        public bool IsDragBegin { get; private set; } = false;

        public bool IsDrag { get; private set; } = false;

        public bool IsDragEnd { get; private set; } = false;

        public void Pressed (Point poition)
        {
            PressedPosition = poition;

            IsDragBegin = false;

            IsDrag = false;

            IsDragEnd = false;
        }

        public void Released (TimeSpan gameTime, int doubleClickedMilliseconds)
        {
            TimeSpan previousReleasedTime = ReleasedTime;

            ReleasedTime = gameTime;

            IsDoubleClick = (ReleasedTime - previousReleasedTime).TotalMilliseconds <= doubleClickedMilliseconds;

            if (IsDragBegin || IsDrag)
            {
                IsDragBegin = false;

                IsDrag = false;

                IsDragEnd = true;
            }
        }

        public void Moved (Point position, int dragThreshold)
        {
            if (!IsDrag)
            {
                if (!IsDragBegin)
                {
                    int deltaPosition = Math.Abs (position.X - PressedPosition.X) + Math.Abs (position.Y - PressedPosition.Y);
                    if (deltaPosition >= dragThreshold)
                    {
                        IsDragBegin = true;
                    }
                }
                else
                {
                    IsDrag = true;
                }
            }
        }
    }

    internal class MouseListener
    {
        private readonly IEnumerable<MouseButtons> _buttons = Enum.GetValues (typeof (MouseButtons)).Cast<MouseButtons> ();

        private GameTime _currentTime = new ();
        private MouseState _currentState;
        private MouseState _previousState;

        // TODO: Make configurable
        public int DoubleClickMilliseconds { get; set; } = 300;
        public int DragThreshold { get; set; } = 5;

        private readonly Dictionary<MouseButtons, ExtendedButtonState> _extendedButtonStates = [];

        private ExtendedButtonState GetExtendedButtonState (MouseButtons button)
        {
            if (!_extendedButtonStates.TryGetValue (button, out ExtendedButtonState? buttonState))
            {
                buttonState = new ExtendedButtonState ();

                _extendedButtonStates[button] = buttonState;
            }

            return buttonState;
        }

        public bool IsButtonDown (MouseButtons button) => GetButtonState (_currentState, button) == ButtonState.Pressed;

        public bool IsButtonUp (MouseButtons button) => GetButtonState (_currentState, button) == ButtonState.Released;

        private static ButtonState GetButtonState (MouseState currentState, MouseButtons button) => button switch
        {
            MouseButtons.None => ButtonState.Released,
            MouseButtons.Left => currentState.LeftButton,
            MouseButtons.Middle => currentState.MiddleButton,
            MouseButtons.Right => currentState.RightButton,
            MouseButtons.XButton1 => currentState.XButton1,
            MouseButtons.XButton2 => currentState.XButton2,
            _ => ButtonState.Released,
        };

        private readonly Dictionary<MouseButtons, MouseObserver> _pressedObservers = [];
        private readonly Dictionary<MouseButtons, MouseObserver> _releasedObservers = [];
        private readonly Dictionary<MouseButtons, MouseObserver> _clickedObservers = [];
        private readonly Dictionary<MouseButtons, MouseObserver> _doubleClickedObservers = [];
        private readonly MouseObserver _movedObserver = new ();
        private readonly Dictionary<MouseButtons, MouseObserver> _dragBeginObservers = [];
        private readonly Dictionary<MouseButtons, MouseObserver> _dragObservers = [];
        private readonly Dictionary<MouseButtons, MouseObserver> _dragEndObservers = [];
        private readonly MouseObserver _wheelMovedObserver = new ();

        public void SubscribePressed (MouseButtons button, EventHandler<MouseEventArgs> handler) => Subscribe (_pressedObservers, button, handler);

        public void UnsubscribePressed (MouseButtons button, EventHandler<MouseEventArgs> handler) => Unsubscribe (_pressedObservers, button, handler);

        public void SubscribeReleased (MouseButtons button, EventHandler<MouseEventArgs> handler) => Subscribe (_releasedObservers, button, handler);

        public void UnsubscribeReleased (MouseButtons button, EventHandler<MouseEventArgs> handler) => Unsubscribe (_releasedObservers, button, handler);

        public void SubscribeClicked (MouseButtons button, EventHandler<MouseEventArgs> handler) => Subscribe (_clickedObservers, button, handler);

        public void UnsubscribeClicked (MouseButtons button, EventHandler<MouseEventArgs> handler) => Unsubscribe (_clickedObservers, button, handler);

        public void SubscribeDoubleClicked (MouseButtons button, EventHandler<MouseEventArgs> handler) => Subscribe (_doubleClickedObservers, button, handler);

        public void UnsubscribeDoubleClicked (MouseButtons button, EventHandler<MouseEventArgs> handler) => Unsubscribe (_doubleClickedObservers, button, handler);

        public void SubscribeDragBegin (MouseButtons button, EventHandler<MouseEventArgs> handler) => Subscribe (_dragBeginObservers, button, handler);

        public void UnsubscribeDragBegin (MouseButtons button, EventHandler<MouseEventArgs> handler) => Unsubscribe (_dragBeginObservers, button, handler);

        public void SubscribeDrag (MouseButtons button, EventHandler<MouseEventArgs> handler) => Subscribe (_dragObservers, button, handler);

        public void UnsubscribeDrag (MouseButtons button, EventHandler<MouseEventArgs> handler) => Unsubscribe (_dragObservers, button, handler);

        public void SubscribeDragEnd (MouseButtons button, EventHandler<MouseEventArgs> handler) => Subscribe (_dragEndObservers, button, handler);

        public void UnsubscribeDragEnd (MouseButtons button, EventHandler<MouseEventArgs> handler) => Unsubscribe (_dragEndObservers, button, handler);

        public void SubscribeMoved (EventHandler<MouseEventArgs> handler) => _movedObserver.Observers += handler;

        public void UnsubscribeMoved (EventHandler<MouseEventArgs> handler) => _movedObserver.Observers -= handler;

        public void SubscribeWheelMoved (EventHandler<MouseEventArgs> handler) => _wheelMovedObserver.Observers += handler;

        public void UnsubscribeWheelMoved (EventHandler<MouseEventArgs> handler) => _wheelMovedObserver.Observers -= handler;

        private static void Subscribe (Dictionary<MouseButtons, MouseObserver> observers, MouseButtons button, EventHandler<MouseEventArgs> handler)
        {
            if (!observers.TryGetValue (button, out MouseObserver? value))
            {
                value = new MouseObserver ();

                observers[button] = value;
            }

            value.Observers += handler;
        }

        private static void Unsubscribe (Dictionary<MouseButtons, MouseObserver> observers, MouseButtons button, EventHandler<MouseEventArgs> handler)
        {
            if (observers.TryGetValue (button, out MouseObserver? value))
            {
                value.Observers -= handler;

                if (!value.HasObservers)
                {
                    observers.Remove (button);
                }
            }
        }

        private static void Notify (Dictionary<MouseButtons, MouseObserver> observers, MouseButtons button, object? sender, MouseEventArgs eventArgs)
        {
            if (observers.TryGetValue (button, out MouseObserver? value))
            {
                value.Notify (sender, eventArgs);
            }
        }

        public void Update (GameTime gameTime)
        {
            _currentTime = gameTime;
            _currentState = Mouse.GetState ();

            RaisePressedEvents ();
            RaiseReleasedEvents ();
            RaiseMovedEvents ();
            RaiseWheelMovedEvents ();

            _previousState = _currentState;
        }

        private void RaisePressedEvents ()
        {
            IEnumerable<MouseButtons> pressedBottons = _buttons.Where (button => GetButtonState (_currentState, button) == ButtonState.Pressed && GetButtonState (_previousState, button) == ButtonState.Released);

            foreach (MouseButtons button in pressedBottons)
            {
                ExtendedButtonState buttonState = GetExtendedButtonState (button);
                buttonState.Pressed (_currentState.Position);

                MouseEventArgs eventArgs = new (button, _currentState, _previousState);
                Notify (_pressedObservers, button, this, eventArgs);

                if (buttonState.IsDragEnd)
                {
                    Notify (_dragEndObservers, button, this, eventArgs);
                }
            }
        }

        private void RaiseReleasedEvents ()
        {
            IEnumerable<MouseButtons> releasedBottons = _buttons.Where (button => GetButtonState (_currentState, button) == ButtonState.Released && GetButtonState (_previousState, button) == ButtonState.Pressed);

            foreach (MouseButtons button in releasedBottons)
            {
                ExtendedButtonState buttonState = GetExtendedButtonState (button);
                buttonState.Released (_currentTime.TotalGameTime, DoubleClickMilliseconds);

                MouseEventArgs eventArgs = new (button, _currentState, _previousState);
                Notify (_releasedObservers, button, this, eventArgs);

                if (!buttonState.IsDoubleClick)
                {
                    Notify (_clickedObservers, button, this, eventArgs);
                }
                else
                {
                    Notify (_doubleClickedObservers, button, this, eventArgs);
                }
            }
        }

        private void RaiseMovedEvents ()
        {
            if (_currentState.Position.X != _previousState.Position.X || _currentState.Position.Y != _previousState.Position.Y)
            {
                _movedObserver.Notify (this, new MouseEventArgs (MouseButtons.None, _currentState, _previousState));

                IEnumerable<MouseButtons> draggedButtons = _buttons.Where (button => GetButtonState (_currentState, button) == ButtonState.Pressed);
                foreach (MouseButtons button in draggedButtons)
                {
                    ExtendedButtonState buttonState = GetExtendedButtonState (button);
                    buttonState.Moved (_currentState.Position, DragThreshold);

                    if (buttonState.IsDrag)
                    {
                        Notify (_dragObservers, button, this, new MouseEventArgs (button, _currentState, _previousState));
                    }
                    else if (buttonState.IsDragBegin)
                    {
                        Notify (_dragBeginObservers, button, this, new MouseEventArgs (button, _currentState, _previousState));
                    }
                }
            }
        }

        private void RaiseWheelMovedEvents ()
        {
            if (_currentState.ScrollWheelValue != _previousState.ScrollWheelValue)
            {
                _wheelMovedObserver.Notify (this, new MouseEventArgs (MouseButtons.None, _currentState, _previousState));
            }
        }
    }
}
