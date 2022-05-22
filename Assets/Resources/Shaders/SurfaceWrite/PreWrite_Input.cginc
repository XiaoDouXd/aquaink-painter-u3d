#ifndef AP_PRE_WRITE_INCLUDED
#define AP_PRE_WRITE_INCLUDED

// 需要传入数据
sampler2D _PreTex;
sampler2D _PreTexAlpha;

float4 preCol(float2 uv)
{
    return tex2D(_PreTex, uv);
}

float preAlpha(float2 uv)
{
    return tex2D(_PreTexAlpha, uv).x;
}

#endif