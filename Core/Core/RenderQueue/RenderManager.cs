using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonoGame.Samples.Library.RenderQueue;

public sealed class RenderManager : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly List<RenderCommand> _commands = [];

    private DynamicVertexBuffer? _vertexBuffer;
    private IndexBuffer? _indexBuffer;
    private VertexDeclaration? _vertexDeclaration;
    private IRenderCommandGeometryBuilder? _geometryBuilder;
    private IRenderCommandGeometryBatch? _geometryBatch;
    private bool _disposed;

    public RenderManager (GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException (nameof (graphicsDevice));
    }

    public int CommandCount => _commands.Count;

    public void Enqueue (RenderCommand command)
    {
        _commands.Add (command);
    }

    public void Draw (
        Material material,
        MaterialPropertyBlock properties,
        Rectangle destination,
        Texture2D? texture = null,
        Rectangle? source = null,
        Color? color = null,
        float depth = 0f)
    {
        Enqueue (new RenderCommand (material, properties, destination, texture, source, color, depth));
    }

    public void Clear ()
    {
        _commands.Clear ();
        _geometryBatch?.Clear ();
    }

    public void Flush ()
    {
        ThrowIfDisposed ();

        if (_commands.Count == 0)
        {
            return;
        }

        int batchStartCommand = 0;
        while (batchStartCommand < _commands.Count)
        {
            RenderCommand firstCommand = _commands[batchStartCommand];
            int batchEndCommand = FindBatchEnd (batchStartCommand, firstCommand);

            BuildVertexData (firstCommand.Material, batchStartCommand, batchEndCommand);
            DrawBatch (firstCommand.Material, firstCommand.Properties, firstCommand.Texture);

            batchStartCommand = batchEndCommand;
        }

        Clear ();
    }

    private int FindBatchEnd (int startCommand, RenderCommand firstCommand)
    {
        int commandIndex = startCommand + 1;
        while (commandIndex < _commands.Count &&
            CanBatch (firstCommand, _commands[commandIndex]))
        {
            commandIndex++;
        }

        return commandIndex;
    }

    private static bool CanBatch (RenderCommand firstCommand, RenderCommand nextCommand)
    {
        return ReferenceEquals (nextCommand.Material, firstCommand.Material) &&
            ReferenceEquals (nextCommand.Properties, firstCommand.Properties) &&
            ReferenceEquals (nextCommand.Texture, firstCommand.Texture);
    }

    private void BuildVertexData (Material material, int startCommand, int endCommand)
    {
        EnsureGeometryBatch (material);
        _geometryBatch!.Clear ();

        for (int commandIndex = startCommand; commandIndex < endCommand; commandIndex++)
        {
            _geometryBatch.Add (_commands[commandIndex]);
        }
    }

    private void DrawBatch (Material material, MaterialPropertyBlock? properties, Texture2D? texture)
    {
        if (_geometryBatch == null || _geometryBatch.VertexCount == 0 || _geometryBatch.IndexCount == 0)
        {
            return;
        }

        EnsureBuffers (material.VertexDeclaration, _geometryBatch.VertexCount, _geometryBatch.IndexCount);

        _geometryBatch.SetVertexData (_vertexBuffer!);
        _geometryBatch.SetIndexData (_indexBuffer!);

        material.ApplyStates (_graphicsDevice);
        material.ApplyProperties (properties);

        if (material.SamplerSlot >= 0)
        {
            if (texture != null)
            {
                _graphicsDevice.Textures[material.SamplerSlot] = texture;
            }
        }

        _graphicsDevice.SetVertexBuffer (_vertexBuffer);
        _graphicsDevice.Indices = _indexBuffer;

        foreach (EffectPass pass in material.Effect.CurrentTechnique.Passes)
        {
            pass.Apply ();
            _graphicsDevice.DrawIndexedPrimitives (
                PrimitiveType.TriangleList,
                0,
                0,
                _geometryBatch.IndexCount / 3);
        }
    }

    private void EnsureGeometryBatch (Material material)
    {
        if (!ReferenceEquals (_geometryBuilder, material.GeometryBuilder))
        {
            _geometryBuilder = material.GeometryBuilder;
            _geometryBatch = _geometryBuilder.CreateBatch ();
        }
    }

    private void EnsureBuffers (VertexDeclaration vertexDeclaration, int vertexCount, int indexCount)
    {
        if (_vertexBuffer == null || _vertexBuffer.VertexCount < vertexCount || !ReferenceEquals (_vertexDeclaration, vertexDeclaration))
        {
            _vertexBuffer?.Dispose ();
            _vertexBuffer = new DynamicVertexBuffer (
                _graphicsDevice,
                vertexDeclaration,
                vertexCount,
                BufferUsage.WriteOnly);
            _vertexDeclaration = vertexDeclaration;
        }

        if (_indexBuffer == null || _indexBuffer.IndexCount < indexCount)
        {
            _indexBuffer?.Dispose ();
            _indexBuffer = new IndexBuffer (
                _graphicsDevice,
                IndexElementSize.SixteenBits,
                indexCount,
                BufferUsage.WriteOnly);
        }
    }

    private void ThrowIfDisposed ()
    {
        ObjectDisposedException.ThrowIf (_disposed, this);
    }

    public void Dispose ()
    {
        if (_disposed)
        {
            return;
        }

        _vertexBuffer?.Dispose ();
        _indexBuffer?.Dispose ();
        _disposed = true;
    }
}
