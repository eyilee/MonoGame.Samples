using Microsoft.Xna.Framework;

namespace Test;

public class Entity
{
    public Vector2 Position
    {
        get => _position;
        set => _position = value;
    }

    public float Rotation
    {
        get => _rotation;
        set => _rotation = value;
    }

    private Vector2 _position;

    private float _rotation;
}
