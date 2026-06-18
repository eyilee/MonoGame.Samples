using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library.Graphics;

public abstract class RenderBatcher : INamedResource, IDisposable
{
    private bool _disposed;

    protected readonly GraphicsDevice _graphicsDevice;

    public ushort Id { get; }

    public string Name { get; }

    public RenderBatcher (GraphicsDevice graphicsDevice, string name)
    {
        _graphicsDevice = graphicsDevice;

        Id = RenderBatcherRegistry.Regist (name, this);
        Name = name;
    }

    ~RenderBatcher ()
    {
        Dispose (false);
    }

    public abstract void Batch (Mesh mesh);

    public abstract void DrawBatch (MaterialInstance material, MaterialPropertyBlock? properties, Texture? texture);

    protected virtual void Dispose (bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                RenderBatcherRegistry.UnRegist (this);
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
