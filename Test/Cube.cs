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

            if (PhysicsBody != null)
            {
                PhysicsBody.Collider.Size = value;
            }
        }
    }

    private readonly SdfRectangle _shape = new ();

    public Cube ()
    {
        _shape.Thickness = 3f;
        _shape.Color = Color.White;
        _shape.Size = new Vector2 (30f, 30f);
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
        _shape.Draw (render);
    }
}
