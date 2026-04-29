using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals.V3;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using MonoGame.Samples.Library.GumUI;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using System;

namespace MonoGame.Samples.PerlinNoise
{
    public class GameUI (GameScene gameScene) : GumUI
    {
        private readonly GameScene _gameScene = gameScene;

        private Label _frequencyLabel = null!;
        private Slider _frequencySlider = null!;
        private CheckBox _domainWarpCheckBox = null!;
        private CheckBox _fractalBrownianMotionCheckBox = null!;

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

            CreateFrequencyLabelAndSlider ();
            stackPanel.AddChild (_frequencyLabel);
            stackPanel.AddChild (_frequencySlider);

            CreatedDomainWarpCheckBox ();
            stackPanel.AddChild (_domainWarpCheckBox);

            CreateFractalBrownianMotionCheckBox ();
            stackPanel.AddChild (_fractalBrownianMotionCheckBox);
        }

        private void CreateFrequencyLabelAndSlider ()
        {
            _frequencyLabel = new Label ()
            {
                X = 4f,
                Y = 4f,
                Text = $"Frequency: {_gameScene.Frequency:F3}",
            };

            if (_frequencyLabel.Visual is LabelVisual labelVisual)
            {
                labelVisual.Color = new Color (0.4f, 0.4f, 0.4f, 1f);
            }

            _frequencySlider = new Slider ()
            {
                X = 4f,
                Y = 4f,
                Width = -8f,
                WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToParent,
                Minimum = _gameScene.MinFrequency,
                Maximum = _gameScene.MaxFrequency,
                Value = _gameScene.Frequency,
                TicksFrequency = _gameScene.FrequencyStep,
                IsSnapToTickEnabled = true,
            };

            _frequencySlider.ValueChanged += (sender, eventArgse) =>
            {
                _frequencyLabel.Text = $"Frequency: {_frequencySlider.Value:F3}";
                _gameScene.SetFrequency ((float)_frequencySlider.Value);
            };
        }

        private void CreatedDomainWarpCheckBox ()
        {
            _domainWarpCheckBox = new CheckBox ()
            {
                X = 0f,
                Width = 4f,
                WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren,
                Height = 4f,
                HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren,
                Text = "Domain Warp",
                IsChecked = _gameScene.IsDomainWarpEnabled,
            };

            if (_domainWarpCheckBox.Visual is CheckBoxVisual checkBoxVisual)
            {
                checkBoxVisual.ForegroundColor = new Color (0.4f, 0.4f, 0.4f, 1f);
                checkBoxVisual.CheckBoxBackground.X = 4f;
                checkBoxVisual.TextInstance.Anchor (Anchor.Left);
                checkBoxVisual.TextInstance.Dock (Dock.SizeToChildren);
                checkBoxVisual.TextInstance.X = 32f;
            }

            _domainWarpCheckBox.Checked += (sender, eventArgs) => _gameScene.EnableDomainWarp (true);
            _domainWarpCheckBox.Unchecked += (sender, eventArgs) => _gameScene.EnableDomainWarp (false);
        }

        private void CreateFractalBrownianMotionCheckBox ()
        {
            _fractalBrownianMotionCheckBox = new CheckBox ()
            {
                X = 0f,
                Width = 4f,
                WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren,
                Height = 4f,
                HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren,
                Text = "Fractal Brownian Motion",
                IsChecked = _gameScene.IsFractalBrownianMotionEnabled,
            };

            if (_fractalBrownianMotionCheckBox.Visual is CheckBoxVisual checkBoxVisual)
            {
                checkBoxVisual.ForegroundColor = new Color (0.4f, 0.4f, 0.4f, 1f);
                checkBoxVisual.CheckBoxBackground.X = 4f;
                checkBoxVisual.TextInstance.Anchor (Anchor.Left);
                checkBoxVisual.TextInstance.Dock (Dock.SizeToChildren);
                checkBoxVisual.TextInstance.X = 32f;
            }

            _fractalBrownianMotionCheckBox.Checked += (sender, eventArgs) => _gameScene.EnableFractalBrownianMotion (true);
            _fractalBrownianMotionCheckBox.Unchecked += (sender, eventArgs) => _gameScene.EnableFractalBrownianMotion (false);
        }

        public void SetFrequency (float frequency)
        {
            if (_frequencySlider.Value != frequency)
            {
                _frequencyLabel.Text = $"Frequency: {frequency:F3}";
                _frequencySlider.Value = frequency;
            }
        }

        public void EnableDomainWarp (bool enabled)
        {
            if (_domainWarpCheckBox.IsChecked != enabled)
            {
                _domainWarpCheckBox.IsChecked = enabled;
            }
        }

        public void EnableFractalBrownianMotion (bool enabled)
        {
            if (_fractalBrownianMotionCheckBox.IsChecked != enabled)
            {
                _fractalBrownianMotionCheckBox.IsChecked = enabled;
            }
        }
    }
}
