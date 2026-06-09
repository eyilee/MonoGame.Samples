using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library.RenderQueue;

public readonly struct RenderCommand
{
    public RenderCommand (
        Material material,
        MaterialPropertyBlock properties,
        Rectangle destination,
        Texture2D? texture = null,
        Rectangle? source = null,
        Color? color = null,
        float depth = 0f)
    {
        Material = material ?? throw new ArgumentNullException (nameof (material));
        Properties = properties ?? throw new ArgumentNullException (nameof (properties));
        Destination = destination;
        Texture = texture;
        Source = source;
        Color = color ?? Color.White;
        Depth = depth;
    }

    public Material Material { get; }

    public MaterialPropertyBlock Properties { get; }

    public Rectangle Destination { get; }

    public Texture2D? Texture { get; }

    public Rectangle? Source { get; }

    public Color Color { get; }

    public float Depth { get; }
}
