#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

struct VSInput
{
    float2 VertexPos : POSITION0; // quad [-0.5..0.5]
    float2 InstancePos : TEXCOORD1;
    float2 InstanceScale : TEXCOORD2;
    float4 ShapeData0 : TEXCOORD3;
    float4 ShapeMask0 : TEXCOORD4;
    float4 Color : COLOR0;
};

struct PSInput
{
    float4 Position : SV_POSITION;
    float2 LocalPos : TEXCOORD0;
    float4 ShapeData0 : TEXCOORD1;
    float4 ShapeMask0 : TEXCOORD2;
    float4 Color : COLOR0;
};

PSInput MainVS (VSInput input)
{
    PSInput o;
    float2 worldPos = input.VertexPos * input.InstanceScale + input.InstancePos;
    o.Position = mul (float4 (worldPos, 0, 1), WorldViewProjection);
    o.LocalPos = input.VertexPos;
    o.ShapeData0 = input.ShapeData0;
    o.ShapeMask0 = input.ShapeMask0;
    o.Color = input.Color;
    return o;
}

// SDF functions
float sdCircle (float2 p, float radius)
{
    return length (p) - radius;
}
float sdBox (float2 p, float2 size)
{
    float2 d = abs (p) - size;
    return length (max (d, 0)) + min (max (d.x, d.y), 0);
}
float sdLine (float2 p, float2 a, float2 b)
{
    float2 pa = p - a, ba = b - a;
    float h = saturate (dot (pa, ba) / dot (ba, ba));
    return length (pa - ba * h);
}

float4 MainPS (PSInput i) : SV_Target
{
    float4 distances;
    distances.x = sdLine (i.LocalPos, float2 (0, 0), float2 (i.ShapeData0.x, i.ShapeData0.y));
    distances.y = sdCircle (i.LocalPos, i.ShapeData0.z);
    distances.z = sdBox (i.LocalPos, i.ShapeData0.xy);
    distances.w = 0;

    float dist = dot (distances, i.ShapeMask0);
    float aa = fwidth (dist);
    float alpha = 1 - smoothstep (0, aa, dist);

    return float4 (i.Color.rgb, i.Color.a * alpha);
}

technique SDFEffect
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS ();
        PixelShader = compile PS_SHADERMODEL MainPS ();
    }
}