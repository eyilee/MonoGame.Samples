#include "Sdf.fxh"

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

float4 MainPS (PSInput i) : SV_Target
{
    float distance = sdfLine (i.LocalPos, i.ShapeData0.xy, i.ShapeData0.zw);
    float thickness = i.Rotation_Scale_Thickness.w / 2.0;
    float w = fwidth (distance);
    float alpha = 1.0 - smoothstep (thickness - w, thickness + w, distance);
    return float4 (i.Color.rgb * alpha, i.Color.a * alpha);
}

technique SdfLine
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS ();
        PixelShader = compile PS_SHADERMODEL MainPS ();
    }
}