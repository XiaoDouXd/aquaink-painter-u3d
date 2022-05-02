#ifndef AP_D2Q4MODEL_INCLUDED
#define AP_D2Q4MODEL_INCLUDED

// D2Q4模型的方向图
// C6  C2  C5    
//   \  |  /     
// C3  C0  C1       // 这是D2Q9的图, D2Q4只取C1~4方向
//   /  |  \
// C7  C4  C8  
// ------------------ 一些内部使用函数和变量的定义 ------
#define c 0.5773502691896258                // 常量c 为 dx/dt 其中 dx 为距离
#define c_sqrtD3 9                          // 3/(c^2)
#define c_2sqrtSqrtD9 40.5                  // 9/(2*(c^4))
#define c_2squrD3 4.5                       // 3/(2*(c^2))
#define RHO0 1                              // 神秘参数
#define weight1234 0.25                     // 各向权值
float __AP_D2Q4_PRIVATE_FEQ(const float weight, const float rho, const float alpha, const float2 v, const float2 e_dir);
float __AP_D2Q4_PRIVATE_F(const float omega, const float pre, const float feq);
float __AP_D2Q4_PRIVATE_RHO(const float4 data);
float2 __AP_D2Q4_PRIVATE_Velocity(const float4 data);
// ----------------------------------------------------

// 更新当前像素的速度
// @param data1_r 右向(1方向)像素位置的速度
// @param dara2_u 2方向像素的速度
// @param data3_l 3方向像素的速度
// @param data4_d 4方向像素的速度
// @return 更新了的该像素数据
float4 ap_d2q4_updateData(const float4 data1_r, const float4 data2_u, const float4 data3_l, const float4 data4_d, const float alpha, const float omega)
{
    // 用我右边像素的fpre和feq值来更新我的fpre值
    float rho = __AP_D2Q4_PRIVATE_RHO(data1_r);
    float2 v = __AP_D2Q4_PRIVATE_Velocity(data1_r);
    float feq = __AP_D2Q4_PRIVATE_FEQ(weight1234, rho, alpha, v, float2(-1, 0));
    float f1 = __AP_D2Q4_PRIVATE_F(omega, data1_r.z, feq);

    // 用我上边像素的fpre和feq值来更新我的fpre值
    rho = __AP_D2Q4_PRIVATE_RHO(data2_u);
    v = __AP_D2Q4_PRIVATE_Velocity(data2_u);
    feq = __AP_D2Q4_PRIVATE_FEQ(weight1234, rho, alpha, v, float2(0, -1));
    float f2 = __AP_D2Q4_PRIVATE_F(omega, data2_u.w, feq);

    // 用我左边像素的fpre和feq值来更新我的fpre值
    rho = __AP_D2Q4_PRIVATE_RHO(data3_l);
    v = __AP_D2Q4_PRIVATE_Velocity(data3_l);
    feq = __AP_D2Q4_PRIVATE_FEQ(weight1234, rho, alpha, v, float2(1, 0));
    float f3 = __AP_D2Q4_PRIVATE_F(omega, data3_l.x, feq);

    // 用我下边像素的fpre和feq值来更新我的fpre值
    rho = __AP_D2Q4_PRIVATE_RHO(data4_d);
    v = __AP_D2Q4_PRIVATE_Velocity(data4_d);
    feq = __AP_D2Q4_PRIVATE_FEQ(weight1234, rho, alpha, v, float2(0, 1));
    float f4 = __AP_D2Q4_PRIVATE_F(omega, data4_d.y, feq);

    return float4(f3, f4, f1, f2);
}

// 计算当前像素的宏观速度
float2 ap_d2q4_getVelocity(const float4 data)
{
    return __AP_D2Q4_PRIVATE_Velocity(data);
}

// --------------------------------------- 这些为内部使用函数, 不建议外用
// 适用于SRT(单松弛模型)的平衡态方程
float __AP_D2Q4_PRIVATE_FEQ(const float weight, const float rho, const float alpha, const float2 v, const float2 e_dir)
{
    float Psi = smoothstep(0, alpha, rho);
    float ev = dot(e_dir, v);
    float vv = dot(v, v);

    return weight * (rho + RHO0*Psi*(c_sqrtD3*ev + c_2sqrtSqrtD9*ev*ev - c_2squrD3*vv));
}

// LBGK更新方程
float __AP_D2Q4_PRIVATE_F(const float omega, const float pre, const float feq)
{
    return (1-omega)*pre + omega*feq;
}

// 密度计算
float __AP_D2Q4_PRIVATE_RHO(const float4 data)
{
    return data.x + data.y + data.z + data.w;
}

// 速度计算
float2 __AP_D2Q4_PRIVATE_Velocity(const float4 data)
{
     return data.x * float2(1, 0) + data.y * float2(0, 1) + data.z * float2(-1, 0) + data.w * float2(0, -1);
}

#endif