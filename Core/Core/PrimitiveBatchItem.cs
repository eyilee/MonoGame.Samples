using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Samples.Library;

internal class PrimitiveBatchItem
{
    public VertexPositionColor VertexTL;
    public VertexPositionColor VertexTR;
    public VertexPositionColor VertexBL;
    public VertexPositionColor VertexBR;

    public PrimitiveBatchItem ()
    {
        VertexTL = new VertexPositionColor ();
        VertexTR = new VertexPositionColor ();
        VertexBL = new VertexPositionColor ();
        VertexBR = new VertexPositionColor ();
    }
}
