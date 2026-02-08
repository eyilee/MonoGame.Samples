using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library;

public class PrimitiveBatch : IDisposable
{
    private bool _disposed;

    private readonly GraphicsDevice _graphicsDevice;
    private readonly BasicEffect _basicEffect;
    private readonly EffectPass _effectPass;
    private readonly PrimitiveBatcher _batcher;

    private bool _beginCalled;

    public PrimitiveBatch (GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;

        _basicEffect = new BasicEffect (graphicsDevice)
        {
            LightingEnabled = false,
            TextureEnabled = false,
            VertexColorEnabled = true
        };

        _effectPass = _basicEffect.CurrentTechnique.Passes[0];

        _batcher = new PrimitiveBatcher (graphicsDevice);
    }

    ~PrimitiveBatch ()
    {
        Dispose (false);
    }

    public void Dispose ()
    {
        Dispose (true);
        GC.SuppressFinalize (this);
    }

    private void Dispose (bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _basicEffect?.Dispose ();
            }

            _disposed = true;
        }
    }

    public void Begin (Matrix? viewMatrix, Matrix? projectionMatrix)
    {
        if (_beginCalled)
        {
            throw new InvalidOperationException ("Begin cannot be called again until End has been successfully called.");
        }

        _beginCalled = true;

        _basicEffect.View = viewMatrix ?? Matrix.Identity;
        _basicEffect.Projection = projectionMatrix ?? Matrix.Identity;
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
        _graphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;

        _batcher.DrawBatch ();
    }

    public void DrawLine (Vector2 start, Vector2 end, Color color, float thickness = 1f)
    {
        float radian = MathF.Atan2 (end.Y - start.Y, end.X - start.X);
        float sin = MathF.Sin (radian);
        float cos = MathF.Cos (radian);
        float halfThickness = thickness * 0.5f;

        PrimitiveBatchItem batchItem = _batcher.CreateBatchItem ();
        batchItem.VertexTL.Position.X = end.X - sin * halfThickness;
        batchItem.VertexTL.Position.Y = end.Y + cos * halfThickness;
        batchItem.VertexTL.Color = color;
        batchItem.VertexTR.Position.X = end.X + sin * halfThickness;
        batchItem.VertexTR.Position.Y = end.Y - cos * halfThickness;
        batchItem.VertexTR.Color = color;
        batchItem.VertexBL.Position.X = start.X - sin * halfThickness;
        batchItem.VertexBL.Position.Y = start.Y + cos * halfThickness;
        batchItem.VertexBL.Color = color;
        batchItem.VertexBR.Position.X = start.X + sin * halfThickness;
        batchItem.VertexBR.Position.Y = start.Y - cos * halfThickness;
        batchItem.VertexBR.Color = color;
    }
}
