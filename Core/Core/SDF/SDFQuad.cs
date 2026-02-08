using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Samples.Library.SDF;

struct SDFQuad : IVertexType
{
    public Vector2 Position;

    public static readonly VertexDeclaration VertexDeclaration = new (
            new VertexElement (0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)
        );

    readonly VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
}
