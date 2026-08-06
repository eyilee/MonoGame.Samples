using Microsoft.Xna.Framework;
using MonoGame.Library;
using MonoGame.Library.Graphics;

namespace Test;

public class ViewPoint
{
    private SdfCircle _shape = null!;

    private Vector2 _position = Vector2.Zero;

    private Vector2 _velocity = Vector2.Zero;

    private float _targetLength = 5f;

    private float _damping = 0.1f;

    public void Initialize (Vector2 position, float targetLength = 5f, float damping = 0.1f)
    {
        _position = position;
        _targetLength = targetLength;
        _damping = damping;

        _shape = new SdfCircle ()
        {
            Position = _position,
            Radius = 3f,
            Color = Color.Yellow
        };

        Camera.Main.LookAt (_position);
    }

    public void Update (Vector2 target, float deltaTime)
    {
        float length = Vector2.Distance (_position, target);

        if (length <= _targetLength)
        {
            _velocity = Vector2.Zero;
            return;
        }

        Vector2 acceleration = (target - _position) / length * (length - _targetLength) * 4f;
        _velocity *= _damping;
        _velocity += acceleration;
        _position += _velocity * deltaTime;

        _shape.Position = _position;
        Camera.Main.LookAt (_position);
    }

    public void Draw (RenderManager render)
    {
        _shape.Draw (render);
    }
}
