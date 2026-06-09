using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonoGame.Samples.Library.RenderQueue;

public delegate void RenderCommandGeometryBuilder<TVertex> (
    RenderCommand command,
    IList<TVertex> vertices,
    IList<short> indices)
    where TVertex : struct, IVertexType;

internal interface IRenderCommandGeometryBuilder
{
    Type VertexType { get; }

    VertexDeclaration VertexDeclaration { get; }

    IRenderCommandGeometryBatch CreateBatch ();
}

internal interface IRenderCommandGeometryBatch
{
    int VertexCount { get; }

    int IndexCount { get; }

    void Clear ();

    void Add (RenderCommand command);

    void SetVertexData (DynamicVertexBuffer vertexBuffer);

    void SetIndexData (IndexBuffer indexBuffer);
}

internal sealed class TypedRenderCommandGeometryBuilder<TVertex> : IRenderCommandGeometryBuilder
    where TVertex : struct, IVertexType
{
    private readonly RenderCommandGeometryBuilder<TVertex> _build;

    public TypedRenderCommandGeometryBuilder (
        VertexDeclaration vertexDeclaration,
        RenderCommandGeometryBuilder<TVertex> build)
    {
        VertexDeclaration = vertexDeclaration ?? throw new ArgumentNullException (nameof (vertexDeclaration));
        _build = build ?? throw new ArgumentNullException (nameof (build));
    }

    public Type VertexType => typeof (TVertex);

    public VertexDeclaration VertexDeclaration { get; }

    public IRenderCommandGeometryBatch CreateBatch ()
    {
        return new RenderCommandGeometryBatch<TVertex> (_build);
    }
}

internal sealed class RenderCommandGeometryBatch<TVertex> : IRenderCommandGeometryBatch
    where TVertex : struct, IVertexType
{
    private readonly RenderCommandGeometryBuilder<TVertex> _build;
    private readonly List<TVertex> _vertices = [];
    private readonly List<short> _indices = [];

    public RenderCommandGeometryBatch (RenderCommandGeometryBuilder<TVertex> build)
    {
        _build = build;
    }

    public int VertexCount => _vertices.Count;

    public int IndexCount => _indices.Count;

    public void Clear ()
    {
        _vertices.Clear ();
        _indices.Clear ();
    }

    public void Add (RenderCommand command)
    {
        _build (command, _vertices, _indices);
    }

    public void SetVertexData (DynamicVertexBuffer vertexBuffer)
    {
        vertexBuffer.SetData (_vertices.ToArray (), 0, _vertices.Count, SetDataOptions.Discard);
    }

    public void SetIndexData (IndexBuffer indexBuffer)
    {
        indexBuffer.SetData (_indices.ToArray (), 0, _indices.Count);
    }
}

public static class RenderQueueVertexBuilders
{
    public static VertexDeclaration GetVertexDeclaration<TVertex> ()
        where TVertex : struct, IVertexType
    {
        return default (TVertex).VertexDeclaration;
    }

    public static void BuildPositionColorTextureQuad (
        RenderCommand command,
        IList<VertexPositionColorTexture> vertices,
        IList<short> indices)
    {
        int vertexOffset = vertices.Count;
        if (vertexOffset > short.MaxValue - 4)
        {
            throw new InvalidOperationException ("A render batch contains too many vertices for 16-bit indices.");
        }

        Rectangle destination = command.Destination;
        Vector2 topLeftUv = Vector2.Zero;
        Vector2 bottomRightUv = Vector2.One;

        if (command.Texture != null && command.Source.HasValue)
        {
            Rectangle source = command.Source.Value;
            topLeftUv = new Vector2 (
                source.X / (float)command.Texture.Width,
                source.Y / (float)command.Texture.Height);
            bottomRightUv = new Vector2 (
                source.Right / (float)command.Texture.Width,
                source.Bottom / (float)command.Texture.Height);
        }

        float left = destination.Left;
        float right = destination.Right;
        float top = destination.Top;
        float bottom = destination.Bottom;
        float depth = command.Depth;
        Color color = command.Color;

        vertices.Add (new VertexPositionColorTexture (new Vector3 (left, top, depth), color, topLeftUv));
        vertices.Add (new VertexPositionColorTexture (new Vector3 (right, top, depth), color, new Vector2 (bottomRightUv.X, topLeftUv.Y)));
        vertices.Add (new VertexPositionColorTexture (new Vector3 (left, bottom, depth), color, new Vector2 (topLeftUv.X, bottomRightUv.Y)));
        vertices.Add (new VertexPositionColorTexture (new Vector3 (right, bottom, depth), color, bottomRightUv));

        indices.Add ((short)(vertexOffset + 0));
        indices.Add ((short)(vertexOffset + 1));
        indices.Add ((short)(vertexOffset + 2));
        indices.Add ((short)(vertexOffset + 1));
        indices.Add ((short)(vertexOffset + 3));
        indices.Add ((short)(vertexOffset + 2));
    }
}
