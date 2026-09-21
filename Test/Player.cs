using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Library;
using MonoGame.Library.Graphics;
using MonoGame.Library.Input;
using MonoGame.Library.Physics;

namespace Test;

public class Player : Entity
{
    public float Radius
    {
        get => _shape.Radius;
        set
        {
            _shape.Radius = value;
            _collider.Radius = value;
        }
    }

    private readonly SdfCircle _shape = new ();

    private readonly CircleCollider _collider = new ();

    private readonly Indicator _indicator = new ();

    private readonly ViewPoint _viewPoint = new ();

    private readonly float _maxVelocity = 400f;

    private readonly float _acceleration = 400f;

    private readonly float _deceleration = 80f;

    private readonly float _maxAngularVelocity = float.Pi;

    private readonly float _angularAcceleration = float.Pi * 4f;

    private Vector2 _velocity = Vector2.Zero;

    private float _angularVelocity = 0f;

    private bool _initialized = false;

    public void Initialize (Vector2 position, float rotation)
    {
        if (_initialized)
        {
            return;
        }

        Position = position;
        Rotation = rotation;
        Radius = 5f;

        _shape.Thickness = 3f;
        _shape.Color = Color.Blue;
        _shape.Filled = true;

        _indicator.Initialize (position, rotation);
        _viewPoint.Initialize (_indicator.Target);

        _initialized = true;
    }

    public void AttachPhysics (PhysicsWorld physicsWorld)
    {
        AddPhysics (physicsWorld);

        PhysicsBody?.AttachCollider (_collider);
    }

    public void DetachPhysics (PhysicsWorld physicsWorld)
    {
        RemovePhysics (physicsWorld);
    }

    public override void OnTransformChanged ()
    {
        _shape.Position = Position;
        _shape.Rotation = Rotation;
        _indicator.Position = Position;
        _indicator.Rotation = Rotation;
    }

    public void Update (InputManager input, float deltaTime)
    {
        ProcessPosition (input, deltaTime);
        ProcessRotation (input, deltaTime);

        _viewPoint.Update (_indicator.Target, deltaTime);
    }

    private void ProcessPosition (InputManager input, float deltaTime)
    {
        float acceleration = 0f;

        if (input.Keyboard.IsKeyDown (Keys.A))
        {
            acceleration = _acceleration;
        }

        if (acceleration != 0f)
        {
            float deltaRotation = MathHelper.WrapAngle (Rotation - float.Atan2 (_velocity.Y, _velocity.X));

            if (float.Abs (deltaRotation) < float.Pi / 6f)
            {
                _velocity.Rotate (deltaRotation);
            }

            Vector2 direction = new (float.Cos (Rotation), float.Sin (Rotation));

            _velocity += direction * acceleration * deltaTime;

            if (_velocity.LengthSquared () > 0f)
            {
                direction = Vector2.Normalize (_velocity);
            }

            _velocity = direction * float.Min (_velocity.Length (), _maxVelocity);
        }
        else
        {
            float velocity = float.Max (_velocity.Length () - _deceleration * deltaTime, 0f);

            Vector2 direction = _velocity;

            if (direction.LengthSquared () > 0f)
            {
                direction.Normalize ();
            }

            _velocity = direction * float.Min (velocity, _maxVelocity);
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
            _angularVelocity = 0f;
            return;
        }

        float targetAngle = float.Atan2 (y, x);
        float delta = MathHelper.WrapAngle (targetAngle - Rotation);

        _angularVelocity += _angularAcceleration * deltaTime * float.Sign (delta);
        _angularVelocity = float.Clamp (_angularVelocity, -_maxAngularVelocity, _maxAngularVelocity);

        float maxStep = float.Abs (_angularVelocity * deltaTime);
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
