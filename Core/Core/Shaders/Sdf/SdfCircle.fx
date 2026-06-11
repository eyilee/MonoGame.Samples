#include "Sdf.fxh"

// p: local position
// r: radius of circle
float sdfCircle (float2 p, float r)
{
    return abs (length (p) - r);
}

float4 MainPS (PSInput i) : SV_Target
{
    float distance = sdfCircle (i.LocalPos, i.ShapeData0.x);
    float thickness = i.Rotation_Scale_Thickness.w / 2.0;
    float w = fwidth (distance);
    float alpha = 1.0 - smoothstep (thickness - w, thickness + w, distance);
    return float4 (i.Color.rgb * alpha, i.Color.a * alpha);
}

technique SdfCircle
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS ();
        PixelShader = compile PS_SHADERMODEL MainPS ();
    }
}