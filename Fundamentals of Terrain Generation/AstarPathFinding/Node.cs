using Microsoft.Xna.Framework;
using MonoGame.Library.Graphics;

namespace AstarPathFinding;

public class Node ()
{
    public bool Value;

    public required Sprite Sprite;

    public required Text Text;

    public Color Color
    {
        get => Sprite.Color;
        set => Sprite.Color = value;
    }

    public string TextValue
    {
        get => Text.Value;
        set => Text.Value = value;
    }

    public Color TextColor
    {
        get => Text.Color;
        set => Text.Color = value;
    }
}
