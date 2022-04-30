#ifndef AP_D2Q9MODEL_INCLUDED
#define AP_D2Q9MODEL_INCLUDED

// D2Q9模型的方向图
// C6  C2  C5    
//   \  |  /     
// C3  C0  C1       // 这是D2Q9的图, D2Q4只取C1~4方向
//   /  |  \
// C7  C4  C8  
// ------------------------------------- 一些定义 ------
#define c 1                                            // 常量c 为 dx/dt 其中 dx 为距离
#define c_sqrtD3 3                                                      // 3/(c^2)
#define c_2sqrtSqrtD9 4.5                                               // 9/(2*(c^4))
#define c_2squrD3 1.5                                                   // 3/(2*(c^2))
#define rho0 1                                                          // 神秘参数

#define weight1234 0.11111111111111111111111111111111                   // 各向权值
#define weight5678 0.02777777777777777777777777777778                   // 各项权值
#define weight0 0.44444444444444444444444444444444                      // 各项权值
#define sqrt2 1.4142135623730950488016887242097                         // 根号2
#define sqrt2by2 0.70710678118654752440084436210485                     // 二分之根号二

// 该点保存的所有分量信息结构体
struct AP_D2Q9_Fi
{
    float4 f1234;
    float4 f5678;
    float f0;
};
float __AP_D2Q9_PRIVATE_FEQ(const float weight, const float rho, const float alpha, const float2 v, const float2 e_dir);
float __AP_D2Q9_PRIVATE_F(const float omega, const float pre, const float feq);
float __AP_D2Q9_PRIVATE_RHO(const AP_D2Q9_Fi data);
float __AP_D2Q9_PRIVATE_UpdateFi(const AP_D2Q9_Fi data, const float2 dir, const float data_fi, const float weight, const float alpha, const float omega);
float2 __AP_D2Q9_PRIVATE_Velocity(const AP_D2Q9_Fi data);
// ----------------------------------------------------
// 利用贴图构建分量信息结构体
// @param tex0 0方向分量贴图 只取贴图的x值
// @param tex1234 1234方向的分量贴图 xyzw分别代表1234方向的值
// @param tex5678 5678方向的分量贴图
// @return 分量信息结构体
AP_D2Q9_Fi tex2D(Texture2D tex0, Texture2D tex1234, Texture2D tex5678, const SamplerState samplerState, const float2 uv)
{
    AP_D2Q9_Fi o;
    o.f0 = tex0.Sample(samplerState, uv).x;
    o.f1234 = tex1234.Sample(samplerState, uv);
    o.f5678 = tex5678.Sample(samplerState, uv);

    return o;
}

// 利用贴图构建分量信息结构体 这里 f0 恒为0
// @param tex1234 1234方向的分量贴图 xyzw 分别代表1234方向的值
// @param tex5678 5678方向的分量贴图
// @return 分量信息结构体
AP_D2Q9_Fi tex2D(Texture2D tex1234, Texture2D tex5678, const SamplerState samplerState, const float2 uv)
{
    AP_D2Q9_Fi o;
    o.f0 = 0.0;
    o.f1234 = tex1234.Sample(samplerState, uv);
    o.f5678 = tex5678.Sample(samplerState, uv);

    return o;
}

// 利用信息构建分量信息结构体
// @param data0 0分量的值
// @param data1234 1234分量的值
// @param data5678 5678分量的值
// @return 分量信息结构体
AP_D2Q9_Fi ap_d2q9_buildFi(const float data0, const float4 data1234, const float4 data5678)
{
    AP_D2Q9_Fi o;
    o.f0 = data0;
    o.f1234 = data1234;
    o.f5678 = data5678;
    return o;
}

// 使用0方向像素值更新当前像素的速度
// @param data0_c 当前像素0位置的速度
// @return 更新了的值
float ap_d2q9_updateDataF0(const AP_D2Q9_Fi data0_c, const float alpha, const float omega)
{
    return __AP_D2Q9_PRIVATE_UpdateFi(data0_c, float2(0, 0), data0_c.f0, weight0, alpha, omega);
}

// 使用1234方向像素值更新当前像素的速度
// @param data1_r 右向(1方向)像素位置的速度
// @param dara2_u 2方向像素的速度
// @param data3_l 3方向像素的速度
// @param data4_d 4方向像素的速度
// @return 更新了的该像素数据
float4 ap_d2q9_updateDataF1234(
    AP_D2Q9_Fi data1_r, AP_D2Q9_Fi data2_u, AP_D2Q9_Fi data3_l, AP_D2Q9_Fi data4_d,
    const float alpha, const float omega)
{
    // 用我右边像素的fpre和feq值来更新我的fpre值
    float f1 = __AP_D2Q9_PRIVATE_UpdateFi(data1_r, float2(-1, 0), data1_r.f1234.z, weight1234, alpha, omega);

    // 用我上边像素的fpre和feq值来更新我的fpre值
    float f2 = __AP_D2Q9_PRIVATE_UpdateFi(data2_u, float2(0, -1), data2_u.f1234.w, weight1234, alpha, omega);;

    // 用我左边像素的fpre和feq值来更新我的fpre值
    float f3 = __AP_D2Q9_PRIVATE_UpdateFi(data3_l, float2(1, 0), data3_l.f1234.x, weight1234, alpha, omega);;

    // 用我下边像素的fpre和feq值来更新我的fpre值
    float f4 = __AP_D2Q9_PRIVATE_UpdateFi(data4_d, float2(0, 1), data4_d.f1234.y, weight1234, alpha, omega);;

    return float4(f3, f4, f1, f2);
}

// 使用5678像素值更新当前像素的速度
// @param data5_ru 右上向(5方向)像素位置的速度
// @param dara6_lu 6方向像素的速度
// @param data7_ld 7方向像素的速度
// @param data8_rd 8方向像素的速度
// @return 更新了的该像素数据
float4 ap_d2q9_updateDataF5678(
    AP_D2Q9_Fi data5_ru, AP_D2Q9_Fi data6_lu, AP_D2Q9_Fi data7_ld, AP_D2Q9_Fi data8_rd,
    const float alpha, const float omega)
{
    // 用我右上边像素的fpre和feq值来更新我的fpre值
    float f1 = __AP_D2Q9_PRIVATE_UpdateFi(data5_ru, float2(-sqrt2by2, -sqrt2by2), data5_ru.f5678.z, weight5678, alpha, omega);

    // 用我左上边像素的fpre和feq值来更新我的fpre值
    float f2 = __AP_D2Q9_PRIVATE_UpdateFi(data6_lu, float2(sqrt2by2, -sqrt2by2), data6_lu.f5678.w, weight5678, alpha, omega);;

    // 用我左下边像素的fpre和feq值来更新我的fpre值
    float f3 = __AP_D2Q9_PRIVATE_UpdateFi(data7_ld, float2(sqrt2by2, sqrt2by2), data7_ld.f5678.x, weight5678, alpha, omega);;

    // 用我右下边像素的fpre和feq值来更新我的fpre值
    float f4 = __AP_D2Q9_PRIVATE_UpdateFi(data8_rd, float2(-sqrt2by2, sqrt2by2), data8_rd.f5678.y, weight5678, alpha, omega);

    return float4(f3, f4, f1, f2);
}

// 计算当前像素的宏观速度
float2 ap_d2q9_getVelocity(const AP_D2Q9_Fi data)
{
    return __AP_D2Q9_PRIVATE_Velocity(data);
}

float2 ap_d2q9_getVelocity(const float4 f1234, const float4 f5678)
{
    AP_D2Q9_Fi fi;
    fi.f0 = 0.0;
    fi.f1234 = f1234;
    fi.f5678 = f5678;
    return __AP_D2Q9_PRIVATE_Velocity(fi);
}

// --------------------------------------- 这些为内部使用函数, 不建议外用
// 更新Fi
float __AP_D2Q9_PRIVATE_UpdateFi(const AP_D2Q9_Fi data, const float2 dir, const float data_fi, const float weight, const float alpha, const float omega)
{
    float rho = __AP_D2Q9_PRIVATE_RHO(data);
    float v = __AP_D2Q9_PRIVATE_Velocity(data);
    float feq = __AP_D2Q9_PRIVATE_FEQ(weight1234, rho, alpha, v, dir);
    return __AP_D2Q9_PRIVATE_F(omega, data_fi, feq);
}

// 适用于SRT(单松弛模型)的平衡态方程
float __AP_D2Q9_PRIVATE_FEQ(const float weight, const float rho, const float alpha, const float2 v, const float2 e_dir)
{
    float Psi = smoothstep(0, alpha, rho);
    float ev = dot(e_dir, v);
    float vv = dot(v, v);

    return weight * (rho + rho0*Psi*(c_sqrtD3*ev + c_2sqrtSqrtD9*ev*ev - c_2squrD3*vv));
}

// LBGK更新方程
float __AP_D2Q9_PRIVATE_F(const float omega, const float pre, const float feq)
{
    return (1-omega)*pre + omega*feq;
}

// 密度计算
float __AP_D2Q9_PRIVATE_RHO(const AP_D2Q9_Fi data)
{
    return data.f0 +
        data.f1234.x + data.f1234.y + data.f1234.z + data.f1234.w +
        data.f5678.x + data.f5678.y + data.f5678.z + data.f5678.w;
}

// 速度计算
float2 __AP_D2Q9_PRIVATE_Velocity(const AP_D2Q9_Fi data)
{
    return
        (data.f1234.x * float2(1, 0) + data.f1234.y * float2(0, 1) +
        data.f1234.z * float2(-1, 0) + data.f1234.w * float2(0, -1) +
        data.f5678.x * float2(sqrt2by2, sqrt2by2) + data.f5678.y + float2(-sqrt2by2, sqrt2by2) +
        data.f5678.z * float2(-sqrt2by2, -sqrt2by2) + data.f5678.w + float2(sqrt2by2, -sqrt2by2)) / rho0;
}

#endif