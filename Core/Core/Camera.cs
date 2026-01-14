using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library
{
    public class Camera
    {
        private readonly GraphicsDevice _graphicsDevice;

        public int Width => _graphicsDevice.Viewport.Width;

        public int Height => _graphicsDevice.Viewport.Height;

        private Vector2 _position = Vector2.Zero;

        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;

                if (WorldBoundsEnabled)
                {
                    ClampPositionToWorldBounds ();
                }
            }
        }

        public Vector2 Origin { get; set; } = Vector2.Zero;

        public float Rotation { get; set; }

        private float _zoom = 1f;
        private float _minZoom;
        private float _maxZoom = float.MaxValue;

        public float Zoom
        {
            get => _zoom;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero (value, nameof (value));
                _zoom = float.Clamp (value, _minZoom, _maxZoom);
            }
        }

        public float MinZoom
        {
            get => _minZoom;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero (value, nameof (value));
                _minZoom = float.Min (value, _maxZoom);
            }
        }

        public float MaxZoom
        {
            get => _maxZoom;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero (value, nameof (value));
                _maxZoom = float.Max (value, _minZoom);
            }
        }

        public bool WorldBoundsEnabled { get; private set; }

        public Rectangle WorldBounds { get; private set; } = Rectangle.Empty;

        public Camera (GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            Origin = new Vector2 (_graphicsDevice.Viewport.Width / 2f, _graphicsDevice.Viewport.Height / 2f);
        }

        public void LookAt (Vector2 target)
        {
            Position = target - Origin;
        }

        public void Translate (Vector2 offset)
        {
            Position += offset;
        }

        public void ZoomIn (float amount)
        {
            Zoom += amount;
        }

        public Matrix GetViewMatrix ()
        {
            return Matrix.CreateTranslation (-_position.X, -_position.Y, 0f) *
                Matrix.CreateTranslation (-Origin.X, -Origin.Y, 0f) *
                Matrix.CreateRotationZ (Rotation) *
                Matrix.CreateScale (_zoom, _zoom, 1f) *
                Matrix.CreateTranslation (Origin.X, Origin.Y, 0f);
        }

        public void EnableWorldBounds (Rectangle worldBounds)
        {
            WorldBoundsEnabled = true;
            WorldBounds = worldBounds;

            ClampPositionToWorldBounds ();
        }

        public void DisableWorldBounds ()
        {
            WorldBoundsEnabled = false;
            WorldBounds = Rectangle.Empty;
        }

        private void ClampPositionToWorldBounds ()
        {
            Vector2 cameraSize = new (_graphicsDevice.Viewport.Width / _zoom, _graphicsDevice.Viewport.Height / _zoom);

            if (WorldBounds.Width < cameraSize.X || WorldBounds.Height < cameraSize.Y)
            {
                _position = WorldBounds.Center.ToVector2 () - Origin;
                return;
            }

            Matrix inverseViewMatrix = Matrix.Invert (GetViewMatrix ());
            Vector2 cameraWorldMin = Vector2.Transform (Vector2.Zero, inverseViewMatrix);

            Vector2 worldBoundsMin = new (WorldBounds.Left, WorldBounds.Top);
            Vector2 worldBoundsMax = new (WorldBounds.Right, WorldBounds.Bottom);

            Vector2 positionOffset = _position - cameraWorldMin;

            _position = Vector2.Clamp (cameraWorldMin, worldBoundsMin, worldBoundsMax - cameraSize) + positionOffset;
        }
    }
}
