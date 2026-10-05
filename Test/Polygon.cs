using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Library;
using MonoGame.Library.Graphics;
using MonoGame.Library.Graphics.Shapes;
using MonoGame.Library.Physics;

namespace Test;

public class Polygon : Entity
{
    private readonly PolygonShape _shape = new ();

    private readonly PolygonCollider _collider = new ();

    private bool _initialized = false;

    public void Initialize (Vector2 position, float rotation, List<Vector2> vertices)
    {
        if (_initialized)
        {
            return;
        }

        Position = position;
        Rotation = rotation;

        _shape.Color = Color.White;
        _shape.SetVertices (vertices);

        _collider.Position = _shape.Position;
        _collider.SetVertices (_shape.Vertices);

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
