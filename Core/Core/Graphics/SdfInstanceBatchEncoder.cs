using Microsoft.Xna.Framework;

namespace MonoGame.Samples.Library.Graphics;

public class SdfInstanceBatchEncoder : IBatchEncoder<VertexSdfInstance>
{
    public int VertexCount => 1;

    public void Encode (VertexSdfInstance[] batchVertices, int index, Mesh mesh)
    {
        Vector4[]? uvs = mesh.UVs;
        Vector4[]? uv1s = mesh.UV1s;
        Vector4[]? uv2s = mesh.UV2s;
        Vector4[]? uv3s = mesh.UV3s;
        Color[]? colors = mesh.Colors;

        ref VertexSdfInstance vertex = ref batchVertices[index];

        if (uvs != null)
        {
            vertex.Position.X = uvs[0].X;
            vertex.Position.Y = uvs[0].Y;
        }

        if (uv1s != null)
        {
            vertex.RotationScaleThickness = uv1s![0];
        }

        if (uv2s != null)
        {
            vertex.ShapeData0 = uv2s![0];
        }

        if (uv3s != null)
        {
            vertex.ShapeData1 = uv3s![0];
        }

        if (colors != null)
        {
            vertex.Color = colors![0];
        }
    }
}
