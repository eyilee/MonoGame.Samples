using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace MonoGame.Samples.Library.Graphics;

public class Mesh
{
    protected int[] _indices = [];

    protected Vector3[] _vertices = [];

    protected Color[]? _colors;

    protected float[]? _uvs;

    protected float[]? _uv1s;

    protected float[]? _uv2s;

    protected float[]? _uv3s;

    protected float[]? _uv4s;

    protected float[]? _uv5s;

    protected float[]? _uv6s;

    protected float[]? _uv7s;

    public int[] Indices { get { return _indices; } }

    public Vector3[] Vertices { get { return _vertices; } }

    public Color[]? Colors { get { return _colors; } }

    public float[]? UVs { get { return _uvs; } }

    public float[]? UV1s { get { return _uv1s; } }

    public float[]? UV2s { get { return _uv2s; } }

    public float[]? UV3s { get { return _uv3s; } }

    public float[]? UV4s { get { return _uv4s; } }

    public float[]? UV5s { get { return _uv5s; } }

    public float[]? UV6s { get { return _uv6s; } }

    public float[]? UV7s { get { return _uv7s; } }

    public void SetIndices (int[] indices) => _indices = indices;

    public void SetVertices (Vector3[] vertices) => _vertices = vertices;

    public void SetColors (Color[] colors) => _colors = colors;

    public void SetUVs (Vector2[] uvs) => _uvs = MemoryMarshal.Cast<Vector2, float> (uvs).ToArray ();

    public void SetUVs (Vector3[] uvs) => _uvs = MemoryMarshal.Cast<Vector3, float> (uvs).ToArray ();

    public void SetUVs (Vector4[] uvs) => _uvs = MemoryMarshal.Cast<Vector4, float> (uvs).ToArray ();

    // TODO: others
}
