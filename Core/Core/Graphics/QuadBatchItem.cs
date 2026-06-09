using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Samples.Library.Graphics;

internal struct QuadBatchItem<TVertexType> where TVertexType : struct, IVertexType
{
    public TVertexType TL;
    public TVertexType TR;
    public TVertexType BL;
    public TVertexType BR;
}
