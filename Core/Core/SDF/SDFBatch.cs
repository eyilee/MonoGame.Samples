using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library.SDF;

public class SDFBatch : IDisposable
{
    private bool _disposed;

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

    ~SDFBatch ()
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
                _effect?.Dispose ();
            }

            _disposed = true;
        }
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
        instance.Rotation = 0f;
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
        instance.Rotation = 0f;
        instance.Scale = new Vector2 (MathF.Abs (end.X - start.X) + thickness * 2f, MathF.Abs (end.Y - start.Y) + thickness * 2f);
        instance.ShapeData0 = new Vector4 (start.X - instance.Position.X, start.Y - instance.Position.Y, end.X - instance.Position.X, end.Y - instance.Position.Y);
        instance.ShapeData1 = new Vector4 (thickness, 0f, 0f, 0f);
        instance.ShapeMask0 = new Vector4 (0f, 1f, 0f, 0f);
        instance.Color = color;
    }

    public void DrawParabora (Vector2 focus, Vector2 directrix, Vector2 min, Vector2 max, Color color, float thickness = 1)
    {
        Vector2 direction = focus - directrix;
        Vector2 vertex = (focus + directrix) * 0.5f;
        Vector2 center = (min + max) * 0.5f;
        Vector2 offset = center - vertex;

        ref SDFInstance instance = ref _batcher.CreateInstance ();
        instance.Position = center;
        instance.Rotation = MathF.Atan2 (direction.Y, direction.X) - MathF.PI / 2f;
        instance.Scale = new Vector2 (float.Abs (max.X - min.X), float.Abs (max.Y - min.Y));
        instance.ShapeData0 = new Vector4 (1f / (4f * Vector2.Distance (focus, vertex)), offset.X, offset.Y, 0f);
        instance.ShapeData1 = new Vector4 (thickness, 0f, 0f, 0f);
        instance.ShapeMask0 = new Vector4 (0f, 0f, 1f, 0f);
        instance.Color = color;
    }
}
