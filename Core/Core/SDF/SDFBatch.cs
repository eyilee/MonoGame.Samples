using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library.SDF
{
    struct SDFQuad : IVertexType
    {
        public Vector2 Position;

        public static readonly VertexDeclaration VertexDeclaration = new (
                new VertexElement (0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)
            );

        readonly VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
    }

    struct SDFInstance : IVertexType
    {
        public Vector2 Position;
        public Vector2 Scale;
        public Vector4 ShapeData0;
        public Vector4 ShapeData1;
        public Vector4 ShapeMask0;
        public Color Color;

        public static readonly VertexDeclaration VertexDeclaration = new (
            new VertexElement (0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement (8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement (16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
            new VertexElement (32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4),
            new VertexElement (48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 5),
            new VertexElement (64, VertexElementFormat.Color, VertexElementUsage.Color, 1)
            );

        readonly VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
    }

    public class SDFBatch
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SDFEffect _effect;
        private readonly EffectPass _effectPass;
        private readonly SDFBatcher _batcher;

        private bool _beginCalled;

        public SDFBatch (GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _effect = new SDFEffect (graphicsDevice);
            _effectPass = _effect.CurrentTechnique.Passes[0];
            _batcher = new SDFBatcher (graphicsDevice);
        }

        public void Begin (Matrix? worldViewProjectionMatrix)
        {
            if (_beginCalled)
            {
                throw new InvalidOperationException ("Begin cannot be called again until End has been successfully called.");
            }

            _beginCalled = true;

            _effect.Parameters["WorldViewProjection"].SetValue (worldViewProjectionMatrix ?? Matrix.Identity);
            _effectPass.Apply ();
        }

        public void End ()
        {
            if (!_beginCalled)
            {
                throw new InvalidOperationException ("Begin must be called before calling End.");
            }

            _beginCalled = false;

            _graphicsDevice.BlendState = BlendState.AlphaBlend;
            _graphicsDevice.DepthStencilState = DepthStencilState.None;
            _graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            _graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            _batcher.DrawBatch ();
        }

        public void DrawCircle (Vector2 center, float radius, Color color, float thickness = 1f)
        {
            ref SDFInstance instance = ref _batcher.CreateInstance ();
            instance.Position = center;
            instance.Scale = new Vector2 (radius * 2f) + new Vector2 (thickness * 2f);
            instance.ShapeData0 = new Vector4 (radius, 0f, 0f, 0f);
            instance.ShapeData1 = new Vector4 (thickness, 0f, 0f, 0f);
            instance.ShapeMask0 = new Vector4 (1f, 0f, 0f, 0f);
            instance.Color = color;
        }

        public void DrawLine (Vector2 start, Vector2 end, Color color, float thickness = 1f)
        {
            ref SDFInstance instance = ref _batcher.CreateInstance ();
            instance.Position = (start + end) * 0.5f;
            instance.Scale = (end - start) + new Vector2 (thickness * 2f);
            instance.ShapeData0 = new Vector4 (start.X - instance.Position.X, start.Y - instance.Position.Y, end.X - instance.Position.X, end.Y - instance.Position.Y);
            instance.ShapeData1 = new Vector4 (thickness, 0f, 0f, 0f);
            instance.ShapeMask0 = new Vector4 (0f, 1f, 0f, 0f);
            instance.Color = color;
        }

        public void DrawParabora (Vector2 focus, Vector2 directrix, float minX, float maxX, float minY, float maxY, Color color, float thickness = 1)
        {
            Vector2 direction = focus - directrix;
            Vector2 topBound = RayClipping (directrix, direction, minX, maxX, minY, maxY);
            Vector2 leftBound = RayClipping (directrix, new Vector2 (direction.Y, -direction.X), minX, maxX, minY, maxY);
            Vector2 rightBound = RayClipping (directrix, new Vector2 (-direction.Y, direction.X), minX, maxX, minY, maxY);

            Vector2 min = Vector2.Min (Vector2.Min (topBound, leftBound), rightBound);
            Vector2 max = Vector2.Max (Vector2.Max (topBound, leftBound), rightBound);

            ref SDFInstance instance = ref _batcher.CreateInstance ();
            instance.Position = (min + max) * 0.5f;
            instance.Scale = (max - min);
            instance.ShapeData0 = new Vector4 (focus.X - instance.Position.X, focus.Y - instance.Position.Y, directrix.X - instance.Position.X, directrix.Y - instance.Position.Y);
            instance.ShapeData1 = new Vector4 (thickness, 0f, 0f, 0f);
            instance.ShapeMask0 = new Vector4 (0f, 0f, 1f, 0f);
            instance.Color = color;
        }

        public static Vector2 RayClipping (Vector2 point, Vector2 direction, float minX, float maxX, float minY, float maxY)
        {
            float max = float.MaxValue;

            if (Math.Abs (direction.X) < float.Epsilon)
            {
                if (point.X < minX || point.X > maxX)
                {
                    return point;
                }
            }
            else
            {
                max = MathF.Min (max, direction.X > 0
                    ? (maxX - point.X) / direction.X
                    : (minX - point.X) / direction.X);
            }

            if (Math.Abs (direction.Y) < float.Epsilon)
            {
                if (point.Y < minY || point.Y > maxY)
                {
                    return point;
                }
            }
            else
            {
                max = MathF.Min (max, direction.Y > 0
                    ? (maxY - point.Y) / direction.Y
                    : (minY - point.Y) / direction.Y);
            }

            return point + direction * max;
        }
    }
}
