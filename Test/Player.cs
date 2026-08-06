using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Library.Graphics;
using MonoGame.Library.Input;

namespace Test;

public class Player : Entity
{
    private readonly SdfCircle _shape = new ();

    private readonly Indicator _indicator = new ();

    private readonly ViewPoint _viewPoint = new ();

    private readonly float _maxVelocity = 200f;

    private readonly float _acceleration = 500f;

    private readonly float _deceleration = 100f;

    private readonly float _angularVelocity = float.Pi;

    private Vector2 _velocity = Vector2.Zero;

    public Player ()
    {
        _shape.Thickness = 3f;
        _shape.Color = Color.Blue;
        _shape.Radius = 5f;
        _shape.Filled = true;
    }

    public void Initialize (Vector2 position, float rotation)
    {
        Position = position;
        Rotation = rotation;

        _shape.Position = position;
        _indicator.Initialize (position, rotation);
        _viewPoint.Initialize (_indicator.Target);
    }

    public void Update (InputManager input, float deltaTime)
    {
        ProcessPosition (input, deltaTime);
        ProcessRotation (input, deltaTime);

        _shape.Position = Position;
        _indicator.Position = Position;
        _indicator.Rotation = Rotation;
        _viewPoint.Update (_indicator.Target, deltaTime);
    }

    private void ProcessPosition (InputManager input, float deltaTime)
    {
        float acceleration = 0f;

        if (input.Keyboard.IsKeyDown (Keys.A))
        {
            acceleration = _acceleration;
        }

        float velocity = _velocity.Length ();

        if (velocity > 0f)
        {
            Vector2 forward = _velocity;
            forward.Normalize ();

            _velocity = forward * float.Max (velocity - _deceleration * deltaTime, 0f);
        }

        if (acceleration != 0f)
        {
            Vector2 direction = new (float.Cos (Rotation), float.Sin (Rotation));

            _velocity += direction * acceleration * deltaTime;

            Vector2 forward = _velocity;
            forward.Normalize ();

            _velocity = forward * float.Min (_velocity.Length (), _maxVelocity);
        }

        Position += _velocity * deltaTime;
    }

    private void ProcessRotation (InputManager input, float deltaTime)
    {
        float x = 0f;
        float y = 0f;

        if (input.Keyboard.IsKeyDown (Keys.Up))
        {
            y -= 1f;
        }

        if (input.Keyboard.IsKeyDown (Keys.Down))
        {
            y += 1f;
        }

        if (input.Keyboard.IsKeyDown (Keys.Left))
        {
            x -= 1f;
        }

        if (input.Keyboard.IsKeyDown (Keys.Right))
        {
            x += 1f;
        }

        if (x == 0f && y == 0f)
        {
            return;
        }

        float targetAngle = float.Atan2 (y, x);
        float delta = MathHelper.WrapAngle (targetAngle - Rotation);
        float maxStep = _angularVelocity * deltaTime;
        float step = float.Clamp (delta, -maxStep, maxStep);

        Rotation += step;
    }

    public void Draw (RenderManager render)
    {
        _shape.Draw (render);
        _indicator.Draw (render);
        _viewPoint.Draw (render);
    }
}
