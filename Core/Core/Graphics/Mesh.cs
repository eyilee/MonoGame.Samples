using Microsoft.Xna.Framework;

namespace MonoGame.Samples.Library.Graphics;

public class Mesh
{
    protected int[] _indices = [];

    protected Vector3[] _vertices = [];

    protected Color[]? _colors;

    protected Vector4[]? _uvs;

    protected Vector4[]? _uv1s;

    protected Vector4[]? _uv2s;

    protected Vector4[]? _uv3s;

    protected Vector4[]? _uv4s;

    protected Vector4[]? _uv5s;

    protected Vector4[]? _uv6s;

    protected Vector4[]? _uv7s;

    public int[] Indices { get { return _indices; } }

    public Vector3[] Vertices { get { return _vertices; } }

    public Color[]? Colors { get { return _colors; } }

    public Vector4[]? UVs { get { return _uvs; } }

    public Vector4[]? UV1s { get { return _uv1s; } }

    public Vector4[]? UV2s { get { return _uv2s; } }

    public Vector4[]? UV3s { get { return _uv3s; } }

    public Vector4[]? UV4s { get { return _uv4s; } }

    public Vector4[]? UV5s { get { return _uv5s; } }

    public Vector4[]? UV6s { get { return _uv6s; } }

    public Vector4[]? UV7s { get { return _uv7s; } }

    public void SetIndices (int[] indices) => _indices = indices;

    public void SetVertices (Vector3[] vertices) => _vertices = vertices;

    public void SetColors (Color[] colors) => _colors = colors;

    public void SetUVs (Vector2[] uvs) => SetUVs (ref uvs, ref _uvs);

    public void SetUVs (Vector3[] uvs) => SetUVs (ref uvs, ref _uvs);

    public void SetUVs (Vector4[] uvs) => _uvs = uvs;

    public void SetUV1s (Vector2[] uvs) => SetUVs (ref uvs, ref _uv1s);

    public void SetUV1s (Vector3[] uvs) => SetUVs (ref uvs, ref _uv1s);

    public void SetUV1s (Vector4[] uvs) => _uv1s = uvs;

    public void SetUV2s (Vector2[] uvs) => SetUVs (ref uvs, ref _uv2s);

    public void SetUV2s (Vector3[] uvs) => SetUVs (ref uvs, ref _uv2s);

    public void SetUV2s (Vector4[] uvs) => _uv2s = uvs;

    public void SetUV3s (Vector2[] uvs) => SetUVs (ref uvs, ref _uv3s);

    public void SetUV3s (Vector3[] uvs) => SetUVs (ref uvs, ref _uv3s);

    public void SetUV3s (Vector4[] uvs) => _uv3s = uvs;

    public void SetUV4s (Vector2[] uvs) => SetUVs (ref uvs, ref _uv4s);

    public void SetUV4s (Vector3[] uvs) => SetUVs (ref uvs, ref _uv4s);

    public void SetUV4s (Vector4[] uvs) => _uv4s = uvs;

    public void SetUV5s (Vector2[] uvs) => SetUVs (ref uvs, ref _uv5s);

    public void SetUV5s (Vector3[] uvs) => SetUVs (ref uvs, ref _uv5s);

    public void SetUV5s (Vector4[] uvs) => _uv5s = uvs;

    public void SetUV6s (Vector2[] uvs) => SetUVs (ref uvs, ref _uv6s);

    public void SetUV6s (Vector3[] uvs) => SetUVs (ref uvs, ref _uv6s);

    public void SetUV6s (Vector4[] uvs) => _uv6s = uvs;

    public void SetUV7s (Vector2[] uvs) => SetUVs (ref uvs, ref _uv7s);

    public void SetUV7s (Vector3[] uvs) => SetUVs (ref uvs, ref _uv7s);

    public void SetUV7s (Vector4[] uvs) => _uv7s = uvs;

    private static void SetUVs (ref Vector2[] from, ref Vector4[]? to)
    {
        to = new Vector4[from.Length];

        for (int i = 0; i < from.Length; i++)
        {
            ref Vector2 uv = ref from[i];
            to[i] = new Vector4 (uv.X, uv.Y, 0f, 0f);
        }
    }

    private static void SetUVs (ref Vector3[] from, ref Vector4[]? to)
    {
        to = new Vector4[from.Length];

        for (int i = 0; i < from.Length; i++)
        {
            ref Vector3 uv = ref from[i];
            to[i] = new Vector4 (uv.X, uv.Y, uv.Z, 0f);
        }
    }
}
