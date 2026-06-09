using Microsoft.Xna.Framework;
using MonoGame.Samples.Library.Content;
using System;

namespace MonoGame.Samples.Library.Graphics;

public struct RenderCommand
{
    public MaterialInstance Material { get; }

    public MaterialPropertyBlock? Properties { get; }

    public Rectangle Destination { get; }

    public TextureHandle? Texture { get; }

    public Rectangle? Source { get; }

    public Color Color { get; }

    public Vector2 Origin { get; }

    public float Rotation { get; }

    public float Depth { get; }

    public ushort Sequence { get; set; }

    public readonly ulong SortKey
    {
        get
        {
            ushort depthBits = (ushort)(float.Clamp (Depth, 0f, 1f) * ushort.MaxValue);

            return ((ulong)Material.Id << 48)
                | ((ulong)(Texture?.Id ?? 0) << 32)
                | ((ulong)depthBits << 16)
                | Sequence;
        }
    }

    public RenderCommand (MaterialInstance material,
        MaterialPropertyBlock? properties,
        Rectangle destination,
        TextureHandle texture,
        Rectangle source,
        Color color,
        Vector2? origin,
        float rotation = 0f,
        float depth = 0f,
        ushort sequence = 0)
    {
        Material = material;
        Properties = properties;
        Destination = destination;
        Texture = texture;
        Source = source;
        Color = color;
        Origin = origin ?? Vector2.Zero;
        Rotation = rotation;
        Depth = depth;
        Sequence = sequence;
    }

    public RenderCommand (MaterialInstance material,
        Rectangle destination,
        TextureHandle texture,
        Rectangle source,
        Color color,
        Vector2? origin,
        float rotation = 0f,
        float depth = 0f,
        ushort sequence = 0)
    {
        Material = material;
        Destination = destination;
        Texture = texture;
        Source = source;
        Color = color;
        Origin = origin ?? Vector2.Zero;
        Rotation = rotation;
        Depth = depth;
        Sequence = sequence;
    }

    public RenderCommand (MaterialInstance material,
        MaterialPropertyBlock? properties,
        Rectangle destination,
        Color color,
        Vector2? origin,
        float rotation = 0f,
        float depth = 0f,
        ushort sequence = 0)
    {
        Material = material;
        Properties = properties;
        Destination = destination;
        Color = color;
        Origin = origin ?? Vector2.Zero;
        Rotation = rotation;
        Depth = depth;
        Sequence = sequence;
    }
}
