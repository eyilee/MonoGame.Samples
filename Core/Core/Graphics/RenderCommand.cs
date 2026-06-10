using MonoGame.Samples.Library.Content;

namespace MonoGame.Samples.Library.Graphics;

public struct RenderCommand
{
    public MaterialInstance Material { get; }

    public MaterialPropertyBlock? Properties { get; }

    public Mesh Mesh { get; set; }

    public TextureHandle? Texture { get; }

    public float Depth { get; }

    public readonly ulong SortKey
    {
        get
        {
            ushort depthBits = (ushort)(float.Clamp (Depth, 0f, 1f) * ushort.MaxValue);

            return ((ulong)Material.Id << 32)
                | ((ulong)(Texture?.Id ?? 0) << 16)
                | depthBits;
        }
    }

    public RenderCommand (MaterialInstance material, MaterialPropertyBlock? properties, Mesh mesh, TextureHandle? texture, float depth = 0f)
    {
        Material = material;
        Properties = properties;
        Mesh = mesh;
        Texture = texture;
        Depth = depth;
    }

    public RenderCommand (MaterialInstance material, Mesh mesh, TextureHandle? texture, float depth = 0f)
    {
        Material = material;
        Mesh = mesh;
        Texture = texture;
        Depth = depth;
    }

    public RenderCommand (MaterialInstance material, Mesh mesh, float depth = 0f)
    {
        Material = material;
        Mesh = mesh;
        Depth = depth;
    }
}
