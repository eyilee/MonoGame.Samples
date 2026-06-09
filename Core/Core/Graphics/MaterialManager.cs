using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonoGame.Samples.Library.Graphics
{
    public static class MaterialManager
    {
        private static readonly Dictionary<string, Material> _materials = [];

        private static ushort _nextMaterialTemplateId = 1;

        public static Material CreateMaterial (string name, Effect effect,
            BlendState? blendState = null,
            int samplerSlot = 0,
            SamplerState? samplerState = null,
            DepthStencilState? depthStencilState = null,
            RasterizerState? rasterizerState = null)
        {
            if (_materials.ContainsKey (name))
            {
                throw new ArgumentException ($"A material with the name '{name}' already exists.");
            }

            Material material = new (_nextMaterialTemplateId++, effect,
                blendState ?? BlendState.AlphaBlend,
                samplerSlot,
                samplerState ?? SamplerState.LinearClamp,
                depthStencilState ?? DepthStencilState.None,
                rasterizerState ?? RasterizerState.CullCounterClockwise);

            _materials.Add (name, material);

            return material;
        }

        public static Material? GetMaterial (string name)
        {
            return _materials.TryGetValue (name, out Material? material) ? material : null;
        }
    }
}
