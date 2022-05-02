#ifndef AP_D2Q9MODEL_U3D_INCLUDED
#define AP_D2Q9MODEL_U3D_INCLUDED

#include "d2q9model.hlsl"
#include "d2q9model_boundback.hlsl"

/** 利用贴图构建分量信息结构体
 * @param tex0 0方向分量贴图 只取贴图的x值
 * @param tex1234 1234方向的分量贴图 xyzw 分别代表1234方向的值
 * @param tex5678 5678方向的分量贴图
 * @return 分量信息结构体
 */
AP_D2Q9_Fi tex2D(const sampler2D tex0, const sampler2D tex1234, const sampler2D tex5678, const float2 uv)
{
    AP_D2Q9_Fi o;
    o.f0 = tex2D(tex0, uv).x;
    o.f1234 = tex2D(tex1234, uv);
    o.f5678 = tex2D(tex5678, uv);

    return o;
}

/** 利用贴图构建带阻塞的分量信息结构体 !!这里的 f0 恒为0!!
 * @param tex1234 1234 方向的分量贴图 xyzw 分别代表 1234 方向的值
 * @param tex5678 5678 方向的贴图
 * @param paper 纸张纹理贴图 格式为 float2
 * @param glue 胶水贴图
 * @param uv uv
 * @return 带阻塞的分量信息结构体
 */
AP_D2Q9_Fik tex2D(const sampler2D tex1234, const sampler2D tex5678, const sampler2D paper, const sampler2D glue, const float2 uv)
{
    AP_D2Q9_Fik o;
    o.fi.f0 = 0;
    o.fi.f1234 = tex2D(tex1234, uv);
    o.fi.f5678 = tex2D(tex5678, uv);
    o.k = float4(tex2D(paper, uv).xy, tex2D(glue, uv).zw);
    return o;
}
#endif