using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library.Graphics;

internal class QuadBatcher<TVertexType> : RenderBatcher where TVertexType : struct, IVertexType
{
    private const int IndexCount = 6;
    private const int VertexCount = 4;

    public static VertexDeclaration VertexDeclaration => VertexDeclarationCache<TVertexType>.VertexDeclaration;

    private readonly IBatchEncoder<TVertexType> _batchEncoder;
    private readonly int _batchSize;

    private int _batchCount;
    private TVertexType[] _batchVertices;

    private readonly IndexBuffer _indexBuffer;
    private readonly DynamicVertexBuffer _vertexBuffer;

    public QuadBatcher (GraphicsDevice graphicsDevice, string name, IBatchEncoder<TVertexType> batchEncoder, int batchSize = ushort.MaxValue / IndexCount)
        : base (graphicsDevice, name)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan (batchSize, ushort.MaxValue / IndexCount);

        _batchEncoder = batchEncoder;
        _batchSize = batchSize;

        _batchCount = 0;
        _batchVertices = new TVertexType[32 * VertexCount];

        ushort[] indices = new ushort[_batchSize * IndexCount];

        for (int i = 0; i < indices.Length / IndexCount; i++)
        {
            int indexIndex = i * IndexCount;
            int vertexIndex = i * VertexCount;
            indices[indexIndex + 0] = (ushort)(vertexIndex + 0);
            indices[indexIndex + 1] = (ushort)(vertexIndex + 1);
            indices[indexIndex + 2] = (ushort)(vertexIndex + 2);
            indices[indexIndex + 3] = (ushort)(vertexIndex + 1);
            indices[indexIndex + 4] = (ushort)(vertexIndex + 3);
            indices[indexIndex + 5] = (ushort)(vertexIndex + 2);
        }

        _indexBuffer = new IndexBuffer (graphicsDevice, IndexElementSize.SixteenBits, _batchSize * IndexCount, BufferUsage.WriteOnly);
        _indexBuffer.SetData (indices);

        _vertexBuffer = new DynamicVertexBuffer (graphicsDevice, VertexDeclaration, _batchSize * VertexCount, BufferUsage.WriteOnly);
    }

    public override void Batch (Mesh mesh)
    {
        int index = _batchCount * VertexCount;
        if (index >= _batchVertices.Length)
        {
            Array.Resize (ref _batchVertices, _batchVertices.Length * 2);
        }

        _batchEncoder.Encode (_batchVertices, index, mesh);

        _batchCount++;
    }

    public override void DrawBatch (MaterialInstance material, MaterialPropertyBlock? properties, Texture? texture)
    {
        if (_batchCount == 0)
        {
            return;
        }

        material.ApplyStates (_graphicsDevice);
        material.ApplyProperties (properties);

        int batchIndex = 0;
        int batchCount = _batchCount;

        while (batchCount > 0)
        {
            int batchCountToProcess = batchCount;
            if (batchCountToProcess > _batchSize)
            {
                batchCountToProcess = _batchSize;
            }

            FlushArray (material, texture, batchIndex * VertexCount, batchCountToProcess);

            batchIndex += batchCountToProcess;
            batchCount -= batchCountToProcess;
        }

        _batchCount = 0;
    }

    private void FlushArray (MaterialInstance material, Texture? texture, int startIndex, int batchCount)
    {
        if (batchCount <= 0)
        {
            return;
        }

        _vertexBuffer.SetData (_batchVertices, startIndex, batchCount * VertexCount, SetDataOptions.Discard);

        _graphicsDevice.Indices = _indexBuffer;
        _graphicsDevice.SetVertexBuffer (_vertexBuffer);

        foreach (EffectPass pass in material.Effect.CurrentTechnique.Passes)
        {
            pass.Apply ();

            _graphicsDevice.Textures[0] = texture;
            _graphicsDevice.DrawIndexedPrimitives (PrimitiveType.TriangleList, 0, 0, batchCount * 2);
        }
    }
}
