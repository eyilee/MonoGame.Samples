using Microsoft.Xna.Framework;
using MonoGame.Library.Graphics;

namespace SineWave1DHill;

public class Node ()
{
    public float Value;

    public required Sprite Sprite;

    public Vector2 Size
    {
        get => Sprite.Size;
        set => Sprite.Size = value;
    }

    public Vector2 Position
    {
        get => Sprite.Position;
        set => Sprite.Position = value;
    }

    public Color Color
    {
        get => Sprite.Color;
        set => Sprite.Color = value;
    }

    public Vector2 Origin
    {
        get => Sprite.Origin;
        set => Sprite.Origin = value;
    }
}
