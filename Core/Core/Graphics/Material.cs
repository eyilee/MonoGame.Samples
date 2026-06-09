using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGame.Samples.Library.Graphics
{
    public class Material
    {
        public ushort Id { get; }

        public Effect Effect { get; }

        public BlendState BlendState { get; }

        public int SamplerSlot { get; }

        public SamplerState SamplerState { get; }

        public DepthStencilState DepthStencilState { get; }

        public RasterizerState RasterizerState { get; }

        private readonly Dictionary<int, EffectParameter?> _parameters = [];

        internal Material (ushort id, Effect effect,
            BlendState? blendState = null,
            int samplerSlot = 0,
            SamplerState? samplerState = null,
            DepthStencilState? depthStencilState = null,
            RasterizerState? rasterizerState = null)
        {
            Id = id;
            Effect = effect;
            BlendState = blendState ?? BlendState.AlphaBlend;
            SamplerSlot = samplerSlot;
            SamplerState = samplerState ?? SamplerState.LinearClamp;
            DepthStencilState = depthStencilState ?? DepthStencilState.None;
            RasterizerState = rasterizerState ?? RasterizerState.CullCounterClockwise;

            foreach (EffectParameter? parameter in Effect.Parameters)
            {
                _parameters[MaterialPropertyIds.GetId (parameter.Name)] = parameter;
            }
        }

        public EffectParameter? GetParameter (int propertyId)
        {
            if (_parameters.TryGetValue (propertyId, out EffectParameter? parameter))
            {
                return parameter;
            }

            parameter = MaterialPropertyIds.TryGetName (propertyId, out string name) ? Effect.Parameters[name] : null;
            _parameters[propertyId] = parameter;

            return parameter;
        }

        public MaterialInstance CreateInstance () => new (this);
    }
}
