using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library.Graphics
{
    public class MaterialInstance (Material material)
    {
        public Material Material { get; } = material;

        public ushort Id => Material.Id;

        public Effect Effect => Material.Effect;

        public BlendState BlendState => Material.BlendState;

        public int SamplerSlot => Material.SamplerSlot;

        public SamplerState SamplerState => Material.SamplerState;

        public DepthStencilState DepthStencilState => Material.DepthStencilState;

        public RasterizerState RasterizerState => Material.RasterizerState;

        public EffectParameter? GetParameter (int propertyId) => Material.GetParameter (propertyId);

        public MaterialPropertyBlock PropertyBlock { get; } = new ();

        public void ApplyStates (GraphicsDevice graphicsDevice)
        {
            ArgumentNullException.ThrowIfNull (graphicsDevice);

            graphicsDevice.BlendState = Material.BlendState;
            graphicsDevice.DepthStencilState = Material.DepthStencilState;
            graphicsDevice.RasterizerState = Material.RasterizerState;
            graphicsDevice.SamplerStates[Material.SamplerSlot] = Material.SamplerState;
        }

        public void ApplyProperties (MaterialPropertyBlock? propertyBlock = null)
        {
            PropertyBlock.ApplyTo (Material);
            propertyBlock?.ApplyTo (Material);
        }
    }
}
