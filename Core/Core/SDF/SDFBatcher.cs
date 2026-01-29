using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library.SDF
{
    internal class SDFBatcher
    {
        private const int InitBatchSize = 128;
        private const int MaxBatchSize = 1024;

        private readonly GraphicsDevice _graphicsDevice;
        private readonly IndexBuffer _indexBuffer;
        private readonly VertexBuffer _vertexBuffer;
        private readonly DynamicVertexBuffer _instanceBuffer;

        private int _batchCount;

        private SDFInstance[] _instances;

        public SDFBatcher (GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            _indexBuffer = new IndexBuffer (graphicsDevice, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);
            _indexBuffer.SetData (new ushort[]
            {
                0, 1, 2,
                2, 1, 3
            });

            _vertexBuffer = new VertexBuffer (graphicsDevice, SDFQuad.VertexDeclaration, 4, BufferUsage.WriteOnly);
            _vertexBuffer.SetData ([
                new SDFQuad { Position = new Vector2 (-0.5f, -0.5f) },
                new SDFQuad { Position = new Vector2 ( 0.5f, -0.5f) },
                new SDFQuad { Position = new Vector2 (-0.5f,  0.5f) },
                new SDFQuad { Position = new Vector2 ( 0.5f,  0.5f) }
                ]);

            _instanceBuffer = new DynamicVertexBuffer (graphicsDevice, SDFInstance.VertexDeclaration, MaxBatchSize, BufferUsage.WriteOnly);

            _batchCount = 0;

            _instances = [];
            Array.Resize (ref _instances, InitBatchSize);
        }

        public ref SDFInstance CreateInstance ()
        {
            if (_batchCount >= _instances.Length)
            {
                int oldSize = _instances.Length;
                int newSize = oldSize * 2;
                newSize = (newSize + 63) & (~63); // grow in chunks of 64.
                Array.Resize (ref _instances, newSize);
            }

            return ref _instances[_batchCount++];
        }

        public void DrawBatch ()
        {
            if (_batchCount == 0)
            {
                return;
            }

            int batchIndex = 0;
            int batchCount = _batchCount;

            while (batchCount > 0)
            {
                int startIndex = batchIndex * 4;

                int batchCountToProcess = batchCount;
                if (batchCountToProcess > MaxBatchSize)
                {
                    batchCountToProcess = MaxBatchSize;
                }

                FlushInstanceArray (startIndex, startIndex + batchCountToProcess);

                batchIndex += batchCountToProcess;
                batchCount -= batchCountToProcess;
            }

            _batchCount = 0;
        }

        private void FlushInstanceArray (int startIndex, int endIndex)
        {
            if (startIndex == endIndex)
            {
                return;
            }

            int instanceCount = endIndex - startIndex;
            _instanceBuffer.SetData (_instances.AsSpan ().Slice (startIndex, instanceCount).ToArray ());

            _graphicsDevice.Indices = _indexBuffer;
            _graphicsDevice.SetVertexBuffers (new VertexBufferBinding (_vertexBuffer, 0, 0), new VertexBufferBinding (_instanceBuffer, 0, 1));
            _graphicsDevice.DrawInstancedPrimitives (PrimitiveType.TriangleStrip, 0, 0, 2, instanceCount);
        }
    }
}
