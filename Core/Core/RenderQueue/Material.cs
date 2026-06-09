using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonoGame.Samples.Library.RenderQueue;

public class Material
{
    private MaterialPropertyBlock _properties = new ();

    public Material (Effect effect)
        : this (
            effect,
            new TypedRenderCommandGeometryBuilder<VertexPositionColorTexture> (
                VertexPositionColorTexture.VertexDeclaration,
                RenderQueueVertexBuilders.BuildPositionColorTextureQuad))
    {
    }

    internal Material (Effect effect, IRenderCommandGeometryBuilder geometryBuilder)
    {
        Effect = effect ?? throw new ArgumentNullException (nameof (effect));
        GeometryBuilder = geometryBuilder ?? throw new ArgumentNullException (nameof (geometryBuilder));
    }

    private readonly Dictionary<int, EffectParameter?> _parameterCache = [];

    public Effect Effect { get; }

    public Type VertexType => GeometryBuilder.VertexType;

    public VertexDeclaration VertexDeclaration => GeometryBuilder.VertexDeclaration;

    internal IRenderCommandGeometryBuilder GeometryBuilder { get; }

    internal EffectParameter? GetParameter (int propertyId)
    {
        if (_parameterCache.TryGetValue (propertyId, out EffectParameter? parameter))
        {
            return parameter;
        }

        parameter = MaterialPropertyIds.TryGetName (propertyId, out string name)
            ? Effect.Parameters[name]
            : null;

        _parameterCache[propertyId] = parameter;
        return parameter;
    }

    public BlendState BlendState { get; set; } = BlendState.AlphaBlend;

    public DepthStencilState DepthStencilState { get; set; } = DepthStencilState.None;

    public RasterizerState RasterizerState { get; set; } = RasterizerState.CullCounterClockwise;

    public SamplerState SamplerState { get; set; } = SamplerState.LinearClamp;

    public int SamplerSlot { get; set; }

    public void ApplyStates (GraphicsDevice graphicsDevice)
    {
        ArgumentNullException.ThrowIfNull (graphicsDevice);

        graphicsDevice.BlendState = BlendState;
        graphicsDevice.DepthStencilState = DepthStencilState;
        graphicsDevice.RasterizerState = RasterizerState;
        graphicsDevice.SamplerStates[SamplerSlot] = SamplerState;
    }

    public void ApplyProperties (MaterialPropertyBlock? properties = null)
    {
        _properties.ApplyTo (this);
        properties?.ApplyTo (this);
    }
}

public sealed class Material<TVertex> : Material
    where TVertex : struct, IVertexType
{
    public Material (Effect effect, RenderCommandGeometryBuilder<TVertex> build)
        : this (effect, RenderQueueVertexBuilders.GetVertexDeclaration<TVertex> (), build)
    {
    }

    public Material (
        Effect effect,
        VertexDeclaration vertexDeclaration,
        RenderCommandGeometryBuilder<TVertex> build)
        : base (effect, new TypedRenderCommandGeometryBuilder<TVertex> (vertexDeclaration, build))
    {
    }
}
