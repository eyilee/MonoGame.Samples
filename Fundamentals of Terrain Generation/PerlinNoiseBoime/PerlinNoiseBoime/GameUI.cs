using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using MonoGameGum;
using MonoGameGum.GueDeriving;

namespace MonoGame.Samples.PerlinNoiseBiome
{
    public class GameUI
    {
        public static void Initialize ()
        {
            Panel panel = new ();
            panel.AddToRoot ();

            ColoredRectangleRuntime background = new ()
            {
                Color = new Color (1f, 1f, 1f, 0.6f)
            };

            background.Dock (Gum.Wireframe.Dock.Fill);
            panel.AddChild (background);

            StackPanel stackPanel = new ()
            {
                Spacing = 0f,
            };

            panel.AddChild (stackPanel);

            foreach ((BiomeType biomeType, BiomeDefinition biomeDefinition) in BiomeDefinition.Definitions)
            {
                stackPanel.AddChild (CreateBiomeLabel (biomeType, biomeDefinition));
            }
        }

        private static Panel CreateBiomeLabel (BiomeType biomeType, BiomeDefinition biomeDefinition)
        {
            Panel panel = new ();

            ColoredRectangleRuntime color = new ()
            {
                Width = 20,
                Height = 20,
                Color = biomeDefinition.Color
            };

            color.Anchor (Gum.Wireframe.Anchor.Left);
            color.X = 2f;
            panel.AddChild (color);

            ContainerRuntime textContainer = new ()
            {
                Width = 4,
                WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren,
                Height = 24
            };

            textContainer.Anchor (Gum.Wireframe.Anchor.Left);
            textContainer.X = 24;
            panel.AddChild (textContainer);

            TextRuntime text = new ()
            {
                Text = biomeType.ToString (),
                Color = Color.Gray,
            };

            text.Anchor (Gum.Wireframe.Anchor.Left);
            textContainer.AddChild (text);

            return panel;
        }
    }
}
