using Gum.Forms.Controls;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using MonoGame.Samples.Library.GumUI;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using System.Collections.Generic;

namespace MonoGame.Samples.PerlinNoiseBiome
{
    public class GameUI : GumUI
    {
        private readonly List<BiomeItem> _biomeItems = [];

        protected override void OnInstantiate ()
        {
            ColoredRectangleRuntime background = new ()
            {
                Color = new Color (1f, 1f, 1f, 0.6f),
            };

            background.Dock (Dock.Fill);
            rootObject.AddChild (background);

            StackPanel stackPanel = new ()
            {
                Spacing = 0f,
            };

            rootObject.AddChild (stackPanel);

            foreach ((BiomeType biomeType, BiomeDefinition biomeDefinition) in BiomeDefinition.Definitions)
            {
                _biomeItems.Add (Instantiate (new BiomeItem (biomeType, biomeDefinition), stackPanel.Visual));
            }
        }

        public class BiomeItem (BiomeType biomeType, BiomeDefinition biomeDefinition) : GumUI
        {
            protected override void OnInstantiate ()
            {
                ColoredRectangleRuntime color = new ()
                {
                    Width = 20,
                    WidthUnits = Gum.DataTypes.DimensionUnitType.Absolute,
                    Height = 20,
                    HeightUnits = Gum.DataTypes.DimensionUnitType.Absolute,
                    Color = biomeDefinition.Color
                };

                color.Anchor (Anchor.Left);
                color.X = 2f;
                rootObject.AddChild (color);

                ContainerRuntime textContainer = new ()
                {
                    Width = 2,
                    WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren,
                    Height = 24,
                    HeightUnits = Gum.DataTypes.DimensionUnitType.Absolute,
                };

                textContainer.Anchor (Anchor.Left);
                textContainer.X = 24;
                rootObject.AddChild (textContainer);

                TextRuntime text = new ()
                {
                    Text = biomeType.ToString (),
                    Color = Color.Gray,
                };

                text.Anchor (Anchor.Left);
                textContainer.AddChild (text);
            }
        }
    }
}
