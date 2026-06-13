using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Samples.Library.Graphics;

public struct VertexSdfInstance : IVertexType
{
    public Vector2 Position;
    public Vector4 RotationScaleThickness;
    public Vector4 ShapeData0;
    public Vector4 ShapeData1;
    public Color Color;

    public static readonly VertexDeclaration VertexDeclaration = new (
        new VertexElement (0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
        new VertexElement (8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
        new VertexElement (24, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
        new VertexElement (40, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
        new VertexElement (56, VertexElementFormat.Color, VertexElementUsage.Color, 0)
        );

    readonly VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
}
