using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library.Graphics;

internal abstract class RenderBatcher (GraphicsDevice graphicsDevice) : IDisposable
{
    private bool _disposed;

    protected readonly GraphicsDevice _graphicsDevice = graphicsDevice;

    ~RenderBatcher ()
    {
        Dispose (false);
    }

    public abstract void DrawBatch (MaterialInstance material, MaterialPropertyBlock? properties, Texture? texture);

    protected virtual void Dispose (bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
            }

            _disposed = true;
        }
    }

    public void Dispose ()
    {
        Dispose (true);
        GC.SuppressFinalize (this);
    }
}
