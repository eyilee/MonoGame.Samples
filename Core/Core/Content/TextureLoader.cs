using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonoGame.Samples.Library.Content;

public sealed class TextureLoader
{
    private readonly Dictionary<string, TextureHandle> _textures = [];

    private bool[] _textureIds = new bool[32];

    private ushort _nextTextureId = 0;

    public TextureHandle Load<TTexture> (string assetName) where TTexture : Texture
    {
        if (_textures.TryGetValue (assetName, out TextureHandle? handle))
        {
            return handle;
        }

        TTexture texture = Core.Content.Load<TTexture> (assetName);
        ushort textureId = AccquireTextureId ();
        handle = new TextureHandle (texture, textureId);
        _textures.Add (assetName, handle);

        return handle;
    }

    public void Unload (string assetName)
    {
        if (!_textures.TryGetValue (assetName, out TextureHandle? handle))
        {
            return;
        }

        _textures.Remove (assetName);
        ReleaseTextureId (handle.Id);
        Core.Content.UnloadAsset (assetName);
    }

    private ushort AccquireTextureId ()
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan (_textures.Count, ushort.MaxValue + 1);

        unchecked
        {
            while (_textureIds[_nextTextureId])
            {
                _nextTextureId++;

                if (_nextTextureId >= _textureIds.Length)
                {
                    Array.Resize (ref _textureIds, _textureIds.Length * 2);
                }
            }
        }

        return _nextTextureId;
    }

    private void ReleaseTextureId (ushort id)
    {
        if (id >= _textureIds.Length)
        {
            return;
        }

        _textureIds[id] = false;
    }
}
