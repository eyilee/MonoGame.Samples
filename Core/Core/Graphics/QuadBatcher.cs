using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library.Graphics;

internal class QuadBatcher<TVertexType> : RenderBatcher where TVertexType : struct, IVertexType
{
    private const int IndexCount = 6;
    private const int VertexCount = 4;
    private const int InitBatchSize = 256;
    private const int MaxBatchSize = 1024;

    private readonly IBatchEncoder<TVertexType> _batchEncoder;
    private readonly VertexDeclaration _vertexDeclaration;

    private int _batchCount;
    private TVertexType[] _batchVertices;

    private readonly IndexBuffer _indexBuffer;
    private readonly DynamicVertexBuffer _vertexBuffer;

    public QuadBatcher (GraphicsDevice graphicsDevice, IBatchEncoder<TVertexType> batchEncoder)
        : base (graphicsDevice)
    {
        _batchEncoder = batchEncoder;
        _vertexDeclaration = VertexDeclarationCache<TVertexType>.VertexDeclaration;

        _batchCount = 0;
        _batchVertices = new TVertexType[InitBatchSize * VertexCount];

        short[] indices = new short[MaxBatchSize * IndexCount];

        for (int i = 0; i < indices.Length / IndexCount; i++)
        {
            int indexIndex = i * IndexCount;
            int vertexIndex = i * VertexCount;
            indices[indexIndex + 0] = (short)(vertexIndex + 0);
            indices[indexIndex + 1] = (short)(vertexIndex + 1);
            indices[indexIndex + 2] = (short)(vertexIndex + 2);
            indices[indexIndex + 3] = (short)(vertexIndex + 1);
            indices[indexIndex + 4] = (short)(vertexIndex + 3);
            indices[indexIndex + 5] = (short)(vertexIndex + 2);
        }

        _indexBuffer = new IndexBuffer (graphicsDevice, IndexElementSize.SixteenBits, MaxBatchSize * IndexCount, BufferUsage.WriteOnly);
        _indexBuffer.SetData (indices);

        _vertexBuffer = new DynamicVertexBuffer (graphicsDevice, _vertexDeclaration, MaxBatchSize * VertexCount, BufferUsage.WriteOnly);
    }

    public void Batch (Mesh mesh)
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
            if (batchCountToProcess > MaxBatchSize)
            {
                batchCountToProcess = MaxBatchSize;
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
