using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Samples.Library.SDF;

struct SDFInstance : IVertexType
{
    public Vector2 Position;
    public float Rotation;
    public Vector2 Scale;
    public Vector4 ShapeData0;
    public Vector4 ShapeData1;
    public Vector4 ShapeMask0;
    public Color Color;

    public static readonly VertexDeclaration VertexDeclaration = new (
        new VertexElement (0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
        new VertexElement (8, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2),
        new VertexElement (12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 3),
        new VertexElement (20, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4),
        new VertexElement (36, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 5),
        new VertexElement (52, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 6),
        new VertexElement (68, VertexElementFormat.Color, VertexElementUsage.Color, 1)
        );

    readonly VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
}
