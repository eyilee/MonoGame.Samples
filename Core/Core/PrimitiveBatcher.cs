using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library;

internal class PrimitiveBatcher
{
    private const int InitialBatchSize = 256;

    private const int MaxBatchSize = short.MaxValue / 6;

    private readonly GraphicsDevice _graphicsDevice;

    private PrimitiveBatchItem[] _batchItemList;

    private int _batchItemCount;

    private VertexPositionColor[] _vertices;
    private short[] _indexes;

    public PrimitiveBatcher (GraphicsDevice graphicsDevice, int capacity = 0)
    {
        _graphicsDevice = graphicsDevice;

        if (capacity <= 0)
        {
            capacity = InitialBatchSize;
        }
        else
        {
            capacity = (capacity + 63) & (~63); // ensure chunks of 64.
        }

        _batchItemList = new PrimitiveBatchItem[capacity];
        for (int i = 0; i < capacity; i++)
        {
            _batchItemList[i] = new PrimitiveBatchItem ();
        }

        _batchItemCount = 0;

        _vertices = [];
        _indexes = [];

        EnsureArrayCapacity (capacity);
    }

    public PrimitiveBatchItem CreateBatchItem ()
    {
        if (_batchItemCount >= _batchItemList.Length)
        {
            int oldSize = _batchItemList.Length;
            int newSize = oldSize * 2;
            newSize = (newSize + 63) & (~63); // grow in chunks of 64.
            Array.Resize (ref _batchItemList, newSize);
            for (int i = oldSize; i < newSize; i++)
            {
                _batchItemList[i] = new PrimitiveBatchItem ();
            }

            EnsureArrayCapacity (Math.Min (newSize, MaxBatchSize));
        }

        return _batchItemList[_batchItemCount++];
    }

    private void EnsureArrayCapacity (int batchItemCount)
    {
        if (_vertices == null || _vertices.Length < 4 * batchItemCount)
        {
            _vertices = new VertexPositionColor[4 * batchItemCount];
        }

        if (_indexes == null || _indexes.Length < 6 * batchItemCount)
        {
            short[] newIndexes = new short[6 * batchItemCount];

            int start = 0;
            if (_indexes != null)
            {
                _indexes.CopyTo (newIndexes, 0);
                start = _indexes.Length / 6;
            }

            for (int i = start; i < batchItemCount; i++)
            {
                int index = i * 6;

                /*
                *  TL    TR
                *   0----1 0, 1, 2, 3 = index offsets for vertex indices
                *   |   /| TL, TR, BL, BR are vertex references in PrivimiteBatchItem.
                *   |  / |
                *   | /  |
                *   |/   |
                *   2----3
                *  BL    BR
                */
                // Triangle 1
                newIndexes[index] = (short)(i * 4);
                newIndexes[index + 1] = (short)(i * 4 + 1);
                newIndexes[index + 2] = (short)(i * 4 + 2);
                // Triangle 2
                newIndexes[index + 3] = (short)(i * 4 + 1);
                newIndexes[index + 4] = (short)(i * 4 + 3);
                newIndexes[index + 5] = (short)(i * 4 + 2);
            }

            _indexes = newIndexes;
        }
    }

    public void DrawBatch ()
    {
        if (_batchItemCount == 0)
        {
            return;
        }

        int batchIndex = 0;
        int batchCount = _batchItemCount;

        while (batchCount > 0)
        {
            int startIndex = batchIndex * 4;

            int batchCountToProcess = batchCount;
            if (batchCountToProcess > MaxBatchSize)
            {
                batchCountToProcess = MaxBatchSize;
            }

            for (int i = 0; i < batchCountToProcess; i++)
            {
                PrimitiveBatchItem batchItem = _batchItemList[batchIndex + i];

                int index = i * 4;
                _vertices[startIndex + index] = batchItem.VertexTL;
                _vertices[startIndex + index + 1] = batchItem.VertexTR;
                _vertices[startIndex + index + 2] = batchItem.VertexBL;
                _vertices[startIndex + index + 3] = batchItem.VertexBR;
            }

            FlushVertexArray (startIndex, startIndex + batchCountToProcess * 4);

            batchIndex += batchCountToProcess;
            batchCount -= batchCountToProcess;
        }

        _batchItemCount = 0;
    }

    private void FlushVertexArray (int startIndex, int endIndex)
    {
        if (startIndex == endIndex)
        {
            return;
        }

        int vertexCount = endIndex - startIndex;

        _graphicsDevice.RasterizerState = RasterizerState.CullNone;

        _graphicsDevice.DrawUserIndexedPrimitives (
            PrimitiveType.TriangleList,
            _vertices,
            0,
            vertexCount,
            _indexes,
            0,
            (vertexCount / 4) * 2,
            VertexPositionColor.VertexDeclaration);
    }
}
