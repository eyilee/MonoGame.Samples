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
    float3 LocalPos : POSITION0;
    float2 WorldPos : TEXCOORD0;
    float4 Rotation_Scale_Thickness : TEXCOORD1;
    float4 ShapeData0 : TEXCOORD2;
    float4 ShapeData1 : TEXCOORD3;
    float4 Color : COLOR0;
};

struct PSInput
{
    float4 Position : SV_POSITION;
    float2 LocalPos : TEXCOORD0;
    float4 Rotation_Scale_Thickness : TEXCOORD1;
    float4 ShapeData0 : TEXCOORD2;
    float4 ShapeData1 : TEXCOORD3;
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

    float rotation = input.Rotation_Scale_Thickness.x;
    float2 scale = input.Rotation_Scale_Thickness.yz;
    float2 localPos = input.LocalPos.xy * scale;
    float2 worldPos = rotate2D (localPos, rotation) + input.WorldPos;

    o.Position = mul (float4 (worldPos, 0.0, 1.0), WorldViewProjection);
    o.LocalPos = localPos;
    o.Rotation_Scale_Thickness = input.Rotation_Scale_Thickness;
    o.ShapeData0 = input.ShapeData0;
    o.ShapeData1 = input.ShapeData1;
    o.Color = float4 (input.Color.rgb * input.Color.a, input.Color.a);

    return o;
}
