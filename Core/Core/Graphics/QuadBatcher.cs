using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library.Graphics;

internal class QuadBatcher<TVertexType> : RenderBatcher where TVertexType : struct, IVertexType
{
    private const int IndexCount = 6;
    private const int InitBatchSize = 256;
    private const int MaxBatchSize = short.MaxValue / IndexCount;

    private readonly IBatchEncoder<TVertexType> _batchEncoder;
    private int VertexCount => _batchEncoder.VertexCount;

    private int _batchCount;
    private TVertexType[] _batchVertices;

    private short[] _indices;
    private TVertexType[] _vertices;

    public QuadBatcher (GraphicsDevice graphicsDevice, IBatchEncoder<TVertexType> batchEncoder)
        : base (graphicsDevice)
    {
        _batchEncoder = batchEncoder;

        _batchCount = 0;
        _batchVertices = new TVertexType[InitBatchSize * _batchEncoder.VertexCount];

        _indices = [];
        _vertices = [];

        EnsureArrayCapacity (InitBatchSize);
    }

    private void EnsureArrayCapacity (int batchCount)
    {
        if (_indices.Length >= batchCount * IndexCount)
        {
            return;
        }

        short[] newIndices = new short[batchCount * IndexCount];
        _indices.CopyTo (newIndices, 0);

        for (int i = _indices.Length / IndexCount; i < newIndices.Length / IndexCount; i++)
        {
            int indexIndex = i * IndexCount;
            int vertexIndex = i * VertexCount;
            newIndices[indexIndex + 0] = (short)(vertexIndex + 0);
            newIndices[indexIndex + 1] = (short)(vertexIndex + 1);
            newIndices[indexIndex + 2] = (short)(vertexIndex + 2);
            newIndices[indexIndex + 3] = (short)(vertexIndex + 1);
            newIndices[indexIndex + 4] = (short)(vertexIndex + 3);
            newIndices[indexIndex + 5] = (short)(vertexIndex + 2);
        }

        _indices = newIndices;
        _vertices = new TVertexType[batchCount * VertexCount];
    }

    public void Batch (Mesh mesh)
    {
        int index = _batchCount * _batchEncoder.VertexCount;
        if (index >= _batchVertices.Length)
        {
            Array.Resize (ref _batchVertices, _batchVertices.Length * 2);

            EnsureArrayCapacity (int.Min (_batchVertices.Length / _batchEncoder.VertexCount, MaxBatchSize));
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

            Array.Copy (_batchVertices, batchIndex * _batchEncoder.VertexCount, _vertices, 0, batchCountToProcess * _batchEncoder.VertexCount);

            FlushArray (material, texture, batchCountToProcess);

            batchIndex += batchCountToProcess;
            batchCount -= batchCountToProcess;
        }

        _batchCount = 0;
    }

    private void FlushArray (MaterialInstance material, Texture? texture, int batchCount)
    {
        if (batchCount <= 0)
        {
            return;
        }

        foreach (EffectPass pass in material.Effect.CurrentTechnique.Passes)
        {
            pass.Apply ();

            _graphicsDevice.Textures[0] = texture;

            _graphicsDevice.DrawUserIndexedPrimitives (PrimitiveType.TriangleList,
                _vertices,
                0,
                batchCount * VertexCount,
                _indices,
                0,
                batchCount * 2);
        }
    }
}
