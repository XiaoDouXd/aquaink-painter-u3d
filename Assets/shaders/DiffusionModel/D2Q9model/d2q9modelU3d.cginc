#ifndef AP_D2Q9MODEL_U3D_INCLUDED
#define AP_D2Q9MODEL_U3D_INCLUDED

#include "d2q9model.hlsl"

/** 利用贴图构建分量信息结构体 (kp恒等于false k值恒等于0)
 * @param tex0 0方向分量贴图 只取贴图的x值
 * @param tex1234 1234方向的分量贴图 xyzw 分别代表1234方向的值
 * @param tex5678 5678方向的分量贴图
 * @return 分量信息结构体
 */
AP_D2Q9_Fi ap_tex2D(
    const sampler2D tex0, const sampler2D tex1234, const sampler2D tex5678,
    const float2 uv)
{
    AP_D2Q9_Fi o;

    o.e = 1;
    o.k = 0;
    o.pin = false;
    o.f0 = tex2D(tex0, uv).x;
    o.f1234 = tex2D(tex1234, uv);
    o.f5678 = tex2D(tex5678, uv);

    return o;
}

/** 利用贴图构建分量信息结构体 (kp恒等于false)
 * @param tex0 0方向分量贴图 只取贴图的x值
 * @param tex1234 1234方向的分量贴图 xyzw 分别代表1234方向的值
 * @param tex5678 5678方向的分量贴图
 * @return 分量信息结构体
 */
AP_D2Q9_Fi ap_tex2D(
    const sampler2D tex0, const sampler2D tex1234, const sampler2D tex5678,
    const float2 uv,
    const sampler2D paper, const sampler2D glue)
{
    AP_D2Q9_Fi o;

    o.e = 1;
    o.pin = false;
    o.k.xy = tex2D(paper, uv).xy;
    o.k.zw = tex2D(glue, uv).yz;
    o.f0 = tex2D(tex0, uv).x;
    o.f1234 = tex2D(tex1234, uv);
    o.f5678 = tex2D(tex5678, uv);

    return o;
}

/** 利用贴图构建分量信息结构体 (会计算kp值)
 * @param tex0 0方向分量贴图 只取贴图的x值
 * @param tex1234 1234方向的分量贴图 xyzw 分别代表1234方向的值
 * @param tex5678 5678方向的分量贴图
 * @return 分量信息结构体
 */
AP_D2Q9_Fi ap_tex2D_kp(
    const sampler2D tex0, const sampler2D tex1234, const sampler2D tex5678,
    const float2 uv, const float2 deltaUV,
    const sampler2D paper, const sampler2D glue)
{
    AP_D2Q9_Fi o;
    const float3 p = tex2D(paper, uv);
    o.k.xy = p.xy;
    o.k.zw = tex2D(glue, uv).yz;
    o.f0 = tex2D(tex0, uv).x;
    o.f1234 = tex2D(tex1234, uv);
    o.f5678 = tex2D(tex5678, uv);

    // 计算 sigma
    const float sigma = Q1 + o.k.w * Q2 + Q3 * lerp(o.k.x, p.z, smoothstep(0, THETA, o.k.z));
    const float sqrt2_sigma = sqrt(2) * sigma;

    // 采样并计算密度
    bool isPin = true;
    isPin = isPin && (__AP_D2Q9_PRIVATE_RHO(ap_tex2D(tex0, tex1234, tex5678, uv + float2( deltaUV.x, 0))) <= sigma);
    isPin = isPin && (__AP_D2Q9_PRIVATE_RHO(ap_tex2D(tex0, tex1234, tex5678, uv + float2(-deltaUV.x, 0))) <= sigma);
    isPin = isPin && (__AP_D2Q9_PRIVATE_RHO(ap_tex2D(tex0, tex1234, tex5678, uv + float2(0,  deltaUV.y))) <= sigma);
    isPin = isPin && (__AP_D2Q9_PRIVATE_RHO(ap_tex2D(tex0, tex1234, tex5678, uv + float2(0, -deltaUV.y))) <= sigma);

    isPin = isPin && (__AP_D2Q9_PRIVATE_RHO(ap_tex2D(tex0, tex1234, tex5678, uv + float2( deltaUV.x, deltaUV.y))) <= sqrt2_sigma);
    isPin = isPin && (__AP_D2Q9_PRIVATE_RHO(ap_tex2D(tex0, tex1234, tex5678, uv + float2(-deltaUV.x, deltaUV.y))) <= sqrt2_sigma);
    isPin = isPin && (__AP_D2Q9_PRIVATE_RHO(ap_tex2D(tex0, tex1234, tex5678, uv + float2( deltaUV.x,-deltaUV.y))) <= sqrt2_sigma);
    isPin = isPin && (__AP_D2Q9_PRIVATE_RHO(ap_tex2D(tex0, tex1234, tex5678, uv + float2(-deltaUV.x,-deltaUV.y))) <= sqrt2_sigma);

    o.pin = isPin;
    o.e = o.pin ? EB : 1;
    
    return o;
}
#endif