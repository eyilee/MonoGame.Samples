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
    return length (p) - radius;
}

float sdfLine (float2 p, float2 a, float2 b, float thickness)
{
    float2 pa = p - a;
    float2 ba = b - a;
    float h = saturate (dot (pa, ba) / dot (ba, ba));
    return length (pa - ba * h) - thickness;
}

float sdfParabola (float2 p, float2 f, float2 d, float thickness)
{
    float2 fd = f - d;
    float2 a = d + float2 (-fd.y, fd.x);
    float2 b = d + float2 (fd.y, -fd.x);
    
    float2 pa = p - a;
    float2 ba = b - a;
    float h = dot (pa, ba) / dot (ba, ba);
    return abs (length (pa - ba * h) - length (p - f)) - thickness;
}

float4 MainPS (PSInput i) : SV_Target
{
    float4 distances;
    distances.x = sdfCircle (i.LocalPos, i.ShapeData0.x);
    distances.y = sdfLine (i.LocalPos, i.ShapeData0.xy, i.ShapeData0.zw, i.ShapeData1.x);
    distances.z = sdfParabola (i.LocalPos, i.ShapeData0.xy, i.ShapeData0.zw, i.ShapeData1.x);
    distances.w = 0;
    
    float distance = dot (distances, i.ShapeMask0);

    float w = fwidth (distance);
    float alpha = 1 - smoothstep (0, w, distance);

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