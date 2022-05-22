#ifndef AP_DO_WRITE_SAMPLER_TRANS_INCLUDED
#define AP_DO_WRITE_SAMPLER_TRANS_INCLUDED

// 需要传入数据
float4 _DoWriteTrans_rect;
unorm float4 _DoWriteTrans_rotaWH;
unorm float _DoWriteTrans_alphaAdd;
unorm float _DoWriteTrans_soft;
sampler2D _DoWriteTrans_brush;

bool isUV(float2 uv)
{
    return step(0, uv.x) * step(uv.x, 1) * step(0, uv.y) * step(uv.y, 1);
}

// 计算 UV 并采样
float getColorAlpha(float2 uv)
{
    const float aspect = _DoWriteTrans_rotaWH.z / _DoWriteTrans_rotaWH.w;
    const float2 rwh = float2(
        abs(_DoWriteTrans_rect.z - _DoWriteTrans_rect.x),
        abs(_DoWriteTrans_rect.y - _DoWriteTrans_rect.w));
    const float2 center = (_DoWriteTrans_rect.xy + _DoWriteTrans_rect.zw) / 2.0;
                
    float2 uvRota = aspect > 1 ?
        center - float2(uv.x * aspect, uv.y) :
        center - float2(uv.x, uv.y / aspect);
    uvRota = float2(
        uvRota.x * _DoWriteTrans_rotaWH.x - uvRota.y * _DoWriteTrans_rotaWH.y,
        uvRota.x * _DoWriteTrans_rotaWH.y + uvRota.y * _DoWriteTrans_rotaWH.x
        );
                
    float2 c = aspect > 1 ?
        center - float2(center.x * aspect, center.y) :
        center - float2(center.x, center.y / aspect);
    c = float2(
        c.x * _DoWriteTrans_rotaWH.x - c.y * _DoWriteTrans_rotaWH.y,
        c.x * _DoWriteTrans_rotaWH.y + c.y * _DoWriteTrans_rotaWH.x);

    uvRota = float2((c.x - uvRota.x)*4/rwh.x + 0.5, (c.y - uvRota.y)*4/rwh.y + 0.5);
    c = (uvRota - float2(0.5, 0.5)) * 2;
    float clip = (1 - dot(c, c) * (_DoWriteTrans_soft + 1) + _DoWriteTrans_soft);
    
    return saturate((tex2D(_DoWriteTrans_brush, uvRota).x * saturate(clip) + _DoWriteTrans_alphaAdd)) * step(0, clip);
}

#endif