using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library.Graphics;

internal class QuadInstanceBatcher<TVertexType> : RenderBatcher where TVertexType : struct, IVertexType
{
    private const int IndexCount = 6;
    private const int VertexCount = 4;

    public static VertexDeclaration VertexDeclaration => VertexDeclarationCache<TVertexType>.VertexDeclaration;

    private readonly IBatchEncoder<TVertexType> _batchEncoder;
    private readonly int _batchSize;

    private int _batchCount;
    private TVertexType[] _batchVertices;

    private readonly IndexBuffer _indexBuffer;
    private readonly VertexBuffer _vertexBuffer;
    private readonly DynamicVertexBuffer _instanceBuffer;

    public QuadInstanceBatcher (GraphicsDevice graphicsDevice, IBatchEncoder<TVertexType> batchEncoder, int batchSize = ushort.MaxValue / IndexCount)
        : base (graphicsDevice)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan (batchSize, ushort.MaxValue / IndexCount);

        _batchEncoder = batchEncoder;
        _batchSize = batchSize;

        _batchCount = 0;
        _batchVertices = new TVertexType[32];

        _indexBuffer = new IndexBuffer (graphicsDevice, IndexElementSize.SixteenBits, IndexCount, BufferUsage.WriteOnly);
        _indexBuffer.SetData (new ushort[] { 0, 1, 2, 1, 3, 2 });

        _vertexBuffer = new VertexBuffer (graphicsDevice, VertexDeclarationCache<VertexPosition>.VertexDeclaration, VertexCount, BufferUsage.WriteOnly);
        _vertexBuffer.SetData ([
            new VertexPosition (new Vector3 (-0.5f, -0.5f, 0f)),
            new VertexPosition (new Vector3 (0.5f, -0.5f, 0f)),
            new VertexPosition (new Vector3 (-0.5f, 0.5f, 0f)),
            new VertexPosition (new Vector3 (0.5f, 0.5f, 0f))
            ]);

        _instanceBuffer = new DynamicVertexBuffer (graphicsDevice, VertexDeclaration, _batchSize, BufferUsage.WriteOnly);
    }

    public void Batch (Mesh mesh)
    {
        int index = _batchCount;
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

            FlushArray (material, texture, batchIndex, batchCountToProcess);

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

        _instanceBuffer.SetData (_batchVertices, startIndex, batchCount, SetDataOptions.Discard);

        _graphicsDevice.Indices = _indexBuffer;
        _graphicsDevice.SetVertexBuffers (new VertexBufferBinding (_vertexBuffer, 0, 0), new VertexBufferBinding (_instanceBuffer, 0, 1));

        foreach (EffectPass pass in material.Effect.CurrentTechnique.Passes)
        {
            pass.Apply ();

            _graphicsDevice.Textures[0] = texture;
            _graphicsDevice.DrawInstancedPrimitives (PrimitiveType.TriangleList, 0, 0, 2, batchCount);
        }
    }
}
