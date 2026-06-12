#include "Sdf.fxh"

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
    float distance = sdfParabola (i.LocalPos, i.ShapeData0.x, i.ShapeData0.yz);
    float thickness = i.Rotation_Scale_Thickness.w / 2.0;
    float w = fwidth (distance);
    float alpha = 1.0 - smoothstep (thickness - w, thickness + w, distance);
    return float4 (i.Color.rgb * alpha, i.Color.a * alpha);
}

technique SdfParabola
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS ();
        PixelShader = compile PS_SHADERMODEL MainPS ();
    }
}