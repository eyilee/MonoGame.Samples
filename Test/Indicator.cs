using Microsoft.Xna.Framework;
using MonoGame.Library.Graphics;

namespace Test;

public class Indicator
{
    public Vector2 Position
    {
        get => _position;
        set
        {
            if (_position != value)
            {
                _position = value;
                _dirty = true;
            }
        }
    }

    public float Rotation
    {
        get => _rotation;
        set
        {
            if (_rotation != value)
            {
                _rotation = value;
                _dirty = true;
            }
        }
    }

    public Vector2 Target => _position + new Vector2 (float.Cos (_rotation), float.Sin (_rotation)) * _length;

    private readonly SdfCircle _shape = new ();

    private readonly float _length = 50f;

    private Vector2 _position = Vector2.Zero;

    private float _rotation = 0f;

    private bool _dirty = true;

    public Indicator ()
    {
        _shape.Color = Color.Red;
        _shape.Radius = 3f;
    }

    public void Initialize (Vector2 position, float rotation)
    {
        Position = position;
        Rotation = rotation;
    }

    public void Draw (RenderManager render)
    {
        if (_dirty)
        {
            _shape.Position = Target;
            _dirty = false;
        }

        _shape.Draw (render);
    }
}
