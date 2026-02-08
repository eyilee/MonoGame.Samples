#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

struct VSInput
{
    float2 VertexPos : POSITION0;
    float2 Position : TEXCOORD1;
    float Rotation : TEXCOORD2;
    float2 Scale : TEXCOORD3;
    float4 ShapeData0 : TEXCOORD4;
    float4 ShapeData1 : TEXCOORD5;
    float4 ShapeMask0 : TEXCOORD6;
    float4 Color : COLOR1;
};

struct PSInput
{
    float4 Position : SV_POSITION;
    float2 LocalPos : TEXCOORD0;
    float4 ShapeData0 : TEXCOORD1;
    float4 ShapeData1 : TEXCOORD2;
    float4 ShapeMask0 : TEXCOORD3;
    float4 Color : COLOR0;
};

// p: position
// r: rotation (radians)
float2 rotate2D (float2 p, float r)
{
    float c = cos (r);
    float s = sin (r);
    return float2 (c * p.x - s * p.y, s * p.x + c * p.y);
}

PSInput MainVS (VSInput input)
{
    PSInput o;

    float2 localPos = input.VertexPos * input.Scale;
    float2 worldPos = rotate2D (localPos, input.Rotation) + input.Position;

    o.Position = mul (float4 (worldPos, 0.0, 1.0), WorldViewProjection);
    o.LocalPos = localPos;
    o.ShapeData0 = input.ShapeData0;
    o.ShapeData1 = input.ShapeData1;
    o.ShapeMask0 = input.ShapeMask0;
    o.Color = float4 (input.Color.rgb * input.Color.a, input.Color.a);

    return o;
}

// p: local position
// r: radius of circle
float sdfCircle (float2 p, float r)
{
    return abs (length (p) - r);
}

// p: local position
// a: line start
// b: line end
float sdfLine (float2 p, float2 a, float2 b)
{
    float2 pa = p - a;
    float2 ba = b - a;
    float h = saturate (dot (pa, ba) / dot (ba, ba));
    return length (pa - ba * h);
}

// pos: local position
// k: parabola coefficient
// o: offset translation
float sdfParabola (float2 pos, float k, float2 o)
{
    pos += o;
    pos.x = abs (pos.x);

    // the derivative of the distance function between point and parabola:
    // (u, v) = (pos.x, pos.y)
    // 2k^2x^3 + (1 - 2kv)x - u = 0
    // divide by 2k^2 to get following standard form:
    // x^3 - 3px - 2q = 0
    // where p = (2kv - 1) / 6k^2 and q = u / (4k^2)
    float ik = 1.0 / k;
    float p = ik * (2.0 * pos.y - ik) / 6.0;
    float q = pos.x * 0.25 * ik * ik;

    float x;

    // have peak at u = -sqrt(p) and valley at x = sqrt(p)
    // put x = sqrt(p) where sqrt(p) > 0 into x^3 - 3px - 2q = 0
    // get p * sqrt(p) - 3p * sqrt(p) - 2q = 0
    // simplify to -2p * sqrt(p) - 2q = 0
    // when there is three real roots, the shift part 2q must smaller than valley value
    // implies -2p * sqrt(p) - 2q < 0
    // shift -2p to the right side and get -2q < 2p * sqrt(p)
    // finally square both sides will get q^2 < p^3
    float h = q * q - p * p * p;
    if (h > 0.0)
    {
        float r = pow (abs (q + sqrt (h)), 1.0 / 3.0);
        x = r + p / r;
    }
    else
    {
        float r = sqrt (p);
        x = 2.0 * r * cos (acos (q / (p * r)) / 3.0);
    }

    return length (pos - float2 (x, k * x * x));
}

float4 MainPS (PSInput i) : SV_Target
{
    float4 distances;
    distances.x = sdfCircle (i.LocalPos, i.ShapeData0.x);
    distances.y = sdfLine (i.LocalPos, i.ShapeData0.xy, i.ShapeData0.zw);
    distances.z = sdfParabola (i.LocalPos, i.ShapeData0.x, i.ShapeData0.yz);
    distances.w = 0.0;

    float distance = dot (distances, i.ShapeMask0);

    float4 thicknesses;
    thicknesses.x = i.ShapeData1.x;
    thicknesses.y = i.ShapeData1.x;
    thicknesses.z = i.ShapeData1.x;
    thicknesses.w = 0.0;

    float thickness = dot (thicknesses, i.ShapeMask0) / 2.0;

    float w = fwidth (distance);
    float alpha = 1.0 - smoothstep (thickness - w, thickness + w, distance);

    return float4 (i.Color.rgb * alpha, i.Color.a * alpha);
}

technique SDFEffect
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS ();
        PixelShader = compile PS_SHADERMODEL MainPS ();
    }
}