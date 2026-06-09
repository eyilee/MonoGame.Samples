using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library.Graphics;

internal class QuadBatcher<TVertexType> : RenderBatcher where TVertexType : struct, IVertexType
{
    public static Type VertexType => typeof (TVertexType);

    private const int InitBatchSize = 256;
    private const int MaxBatchSize = short.MaxValue / 6;

    private int _batchCount;
    private QuadBatchItem<TVertexType>[] _batchItems;

    private short[] _indexes;
    private TVertexType[] _vertices;

    public QuadBatcher (GraphicsDevice graphicsDevice)
        : base (graphicsDevice)
    {
        _batchCount = 0;
        _batchItems = new QuadBatchItem<TVertexType>[InitBatchSize];

        _indexes = [];
        _vertices = [];

        EnsureArrayCapacity (InitBatchSize);
    }

    private void EnsureArrayCapacity (int batchCount)
    {
        if (_indexes.Length >= batchCount * 6)
        {
            return;
        }

        short[] newIndexes = new short[batchCount * 6];
        _indexes.CopyTo (newIndexes, 0);

        for (int i = _indexes.Length / 6; i < newIndexes.Length / 6; i++)
        {
            int indexIndex = i * 6;
            int vertexIndex = i * 4;
            newIndexes[indexIndex + 0] = (short)(vertexIndex + 0);
            newIndexes[indexIndex + 1] = (short)(vertexIndex + 1);
            newIndexes[indexIndex + 2] = (short)(vertexIndex + 2);
            newIndexes[indexIndex + 3] = (short)(vertexIndex + 1);
            newIndexes[indexIndex + 4] = (short)(vertexIndex + 3);
            newIndexes[indexIndex + 5] = (short)(vertexIndex + 2);
        }

        _indexes = newIndexes;
        _vertices = new TVertexType[batchCount * 4];
    }

    public ref QuadBatchItem<TVertexType> CreateBatchItem ()
    {
        if (_batchCount >= _batchItems.Length)
        {
            Array.Resize (ref _batchItems, _batchItems.Length * 2);

            EnsureArrayCapacity (int.Min (_batchItems.Length, MaxBatchSize));
        }

        return ref _batchItems[_batchCount++];
    }

    public override void DrawBatch (MaterialInstance material, MaterialPropertyBlock? properties, Texture? texture)
    {
        if (_batchCount == 0)
        {
            return;
        }

        material.ApplyStates (_graphicsDevice);
        material.ApplyProperties (properties);
        _graphicsDevice.Textures[0] = texture;

        int batchIndex = 0;
        int batchCount = _batchCount;

        while (batchCount > 0)
        {
            int startIndex = batchIndex;

            int batchCountToProcess = batchCount;
            if (batchCountToProcess > MaxBatchSize)
            {
                batchCountToProcess = MaxBatchSize;
            }

            for (int i = 0; i < batchCountToProcess; i++)
            {
                int batchItemIndex = startIndex + i;
                QuadBatchItem<TVertexType> batchItem = _batchItems[batchItemIndex];

                int vertexIndex = i * 4;
                _vertices[vertexIndex + 0] = batchItem.TL;
                _vertices[vertexIndex + 1] = batchItem.TR;
                _vertices[vertexIndex + 2] = batchItem.BL;
                _vertices[vertexIndex + 3] = batchItem.BR;
            }

            FlushArray (material, startIndex, startIndex + batchCountToProcess);

            batchIndex += batchCountToProcess;
            batchCount -= batchCountToProcess;
        }

        _batchCount = 0;
    }

    private void FlushArray (MaterialInstance material, int startIndex, int endIndex)
    {
        if (startIndex == endIndex)
        {
            return;
        }

        int batchItemCount = endIndex - startIndex;

        foreach (EffectPass pass in material.Effect.CurrentTechnique.Passes)
        {
            pass.Apply ();

            _graphicsDevice.DrawUserIndexedPrimitives (PrimitiveType.TriangleList,
                _vertices,
                0,
                batchItemCount * 4,
                _indexes,
                0,
                batchItemCount * 2);
        }
    }
}
