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
    float2 Scale : TEXCOORD2;
    float4 ShapeData0 : TEXCOORD3;
    float4 ShapeData1 : TEXCOORD4;
    float4 ShapeMask0 : TEXCOORD5;
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

PSInput MainVS (VSInput input)
{
    PSInput o;

    float2 localPos = input.VertexPos * input.Scale;
    float2 worldPos = localPos + input.Position;

    o.Position = mul (float4 (worldPos, 0, 1), WorldViewProjection);
    o.LocalPos = localPos;
    o.ShapeData0 = input.ShapeData0;
    o.ShapeData1 = input.ShapeData1;
    o.ShapeMask0 = input.ShapeMask0;
    o.Color = float4 (input.Color.rgb * input.Color.a, input.Color.a);

    return o;
}

float sdfCircle (float2 p, float radius)
{
    return abs (length (p) - radius);
}

float sdfLine (float2 p, float2 a, float2 b)
{
    float2 pa = p - a;
    float2 ba = b - a;
    float h = saturate (dot (pa, ba) / dot (ba, ba));
    return length (pa - ba * h);
}

float sdfParabolaImplicit (float2 p, float2 f, float2 d)
{
    return length (p - f) - dot (p - d, normalize (f - d));
}

// not SDF exact, but good enough for rendering
float sdfParabola (float2 p, float2 f, float2 d)
{
    float distance = sdfParabolaImplicit (p, f, d);

    float eps = 0.5f;

    float2 gradient;
    gradient.x = sdfParabolaImplicit (p + float2 (eps, 0), f, d) - distance;
    gradient.y = sdfParabolaImplicit (p + float2 (0, eps), f, d) - distance;

    return abs (distance / max (length (gradient), eps));
}

float4 MainPS (PSInput i) : SV_Target
{
    float4 distances;
    distances.x = sdfCircle (i.LocalPos, i.ShapeData0.x);
    distances.y = sdfLine (i.LocalPos, i.ShapeData0.xy, i.ShapeData0.zw);
    distances.z = sdfParabola (i.LocalPos, i.ShapeData0.xy, i.ShapeData0.zw);
    distances.w = 0;

    float distance = dot (distances, i.ShapeMask0);

    float4 thicknesses;
    thicknesses.x = i.ShapeData1.x;
    thicknesses.y = i.ShapeData1.x;
    thicknesses.z = i.ShapeData1.x;
    thicknesses.w = 0;

    float thickness = dot (thicknesses, i.ShapeMask0) / 2;

    float w = fwidth (distance);
    float alpha = 1 - smoothstep (thickness - w, thickness + w, distance);

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