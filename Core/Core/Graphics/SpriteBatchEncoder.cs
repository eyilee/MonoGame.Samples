using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Samples.Library.Graphics;

public class SpriteBatchEncoder : IBatchEncoder<VertexPositionColorTexture>
{
    public int VertexCount => 4;

    public void Encode (VertexPositionColorTexture[] batchVertices, int index, Mesh mesh)
    {
        Vector3[] vertices = mesh.Vertices;
        Color[]? colors = mesh.Colors;
        Vector4[]? uvs = mesh.UVs;

        bool hasColors = colors != null;
        bool hasUVs = uvs != null;

        for (int i = 0; i < VertexCount; i++)
        {
            ref VertexPositionColorTexture vertex = ref batchVertices[index + i];
            vertex.Position = vertices[i];

            if (hasColors)
            {
                vertex.Color = colors![i];
            }

            if (hasUVs)
            {
                vertex.TextureCoordinate.X = uvs![i].X;
                vertex.TextureCoordinate.Y = uvs![i].Y;
            }
        }
    }
}
