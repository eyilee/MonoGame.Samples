using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Samples.Library.Graphics;

public interface IBatchEncoder<TVertexType> where TVertexType : struct, IVertexType
{
    public abstract int VertexCount { get; }

    public abstract void Encode (TVertexType[] vertex, int index, Mesh mesh);
}
