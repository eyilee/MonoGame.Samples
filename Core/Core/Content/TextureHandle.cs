using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Samples.Library.Content;

public sealed class TextureHandle (Texture texture, ushort id)
{
    public Texture Texture { get; } = texture;

    public ushort Id { get; } = id;
}
