using Microsoft.Xna.Framework;
using MonoGame.Library;
using MonoGame.Library.Graphics;
using MonoGame.Library.Physics;

namespace Test;

public class Cube : Entity
{
    public Vector2 Size
    {
        get => _shape.Size;
        set
        {
            _shape.Size = value;
            _collider.Size = value;
        }
    }

    private readonly SdfRectangle _shape = new ();

    private readonly BoxCollider _collider = new ();

    private bool _initialized = false;

    public void Initialize (Vector2 position, float rotation)
    {
        if (_initialized)
        {
            return;
        }

        Position = position;
        Rotation = rotation;
        Size = new Vector2 (30f, 30f);

        _shape.Thickness = 3f;
        _shape.Color = Color.White;

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
    }

    public override void OnCollisionEnter (Collision collision)
    {
        _shape.Color = Color.Red;
    }

    public override void OnCollisionStay (Collision collision)
    {
        _shape.Color = Color.Red;
    }

    public override void OnCollisionExit (Collision collision)
    {
        _shape.Color = Color.White;
    }

    public void Draw (RenderManager render)
    {
        if (!_initialized)
        {
            return;
        }

        _shape.Draw (render);
    }
}
