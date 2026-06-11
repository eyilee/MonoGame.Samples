using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Samples.Library.Graphics;

public class VertexDeclarationCache<T> where T : struct, IVertexType
{
    private static VertexDeclaration? _cache;

    public static VertexDeclaration VertexDeclaration
    {
        get
        {
            if (_cache == null)
            {
                _cache = default (T).VertexDeclaration;
            }

            return _cache;
        }
    }
}