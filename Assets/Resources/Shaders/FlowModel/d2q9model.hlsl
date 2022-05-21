#ifndef AP_D2Q9MODEL_INCLUDED
#define AP_D2Q9MODEL_INCLUDED

// D2Q9模型的方向图
// C6  C2  C5    
//   \  |  /     
// C3  C0  C1       // 这是D2Q9的图, D2Q4只取C1~4方向
//   /  |  \
// C7  C4  C8  
// ------------------------------------- 一些定义 ------
#define DX 1                                                            // 距离间隔
#define DT 1                                                            // 时间间隔
#define RHO0 1                                                          // 液体平均密度(水的密度是1)
float E ;//0.0002                                                        // 蒸发率
#define EB 0.3                                                          // 边界蒸发率影响因子

static const float weight1234 = 1.0 / 9.0;                              // 各向权值
static const float weight5678 = 1.0 / 36.0;                             // 各项权值
static const float weight0 = 4.0 / 9.0;                                 // 各项权值

static const float c_ = DX / DT;                                        // 常量c
static const float c_sqrtD3 = 3.0 / c_*c_;                              // 3/(c^2)
static const float c_2sqrtSqrtD9 = 9.0 / (2.0*c_*c_*c_*c_);             // 9/(2*(c^4))
static const float c_2squrD3 = 3.0 / (2.0*c_*c_);                       // 3/(2*(c^2))
static const float sqrt2by2 = sqrt(2) / 2.0;                            // 二分之根号二
static const float evaporation = E * c_;                                // 蒸发率
static const float avgRho = 1.0 / RHO0;                                 // 平均密度

#define K0 0.002
#define K1 0.01
#define K2 0.6
#define K3 0.1
#define K4 0.1

#define Q1 0.0001
#define Q2 0.0001
#define Q3 0.01
#define THETA 0.005

#define XI 0.005
#define MU 0.005
float ETA; //0.005


// 该点保存的所有分量信息结构体
struct AP_D2Q9_Fi
{
    unorm half4 k;          // 阻塞因子
    unorm half4 f1234;      // 各方向粒子数
    unorm half4 f5678;      // 各方向粒子数
    unorm half f0;          // 静态粒子数
    unorm half e;           // 蒸发因子
    bool pin;               // 是否完全阻塞
};
float __AP_D2Q9_PRIVATE_FEQ(const float weight, const float rho, const float alpha, const float2 v, const float2 e_dir);
float __AP_D2Q9_PRIVATE_F(const float omega, const unorm float k, const float data_fi, const float data_fk, const float feq);
float __AP_D2Q9_PRIVATE_RHO(const AP_D2Q9_Fi data);
unorm float __AP_D2Q9_PRIVATE_UpdateFi(const AP_D2Q9_Fi data, const AP_D2Q9_Fi data_c, const float2 dir, const float data_fi, const float data_fk, const float weight, const float alpha, const float omega);
float2 __AP_D2Q9_PRIVATE_Velocity(const AP_D2Q9_Fi data);
// ----------------------------------------------------
/** 利用信息构建分量信息结构体
   @param data0 0分量的值
   @param data1234 1234分量的值
   @param data5678 5678分量的值
   @return 分量信息结构体
*/
AP_D2Q9_Fi ap_d2q9_buildFi(const float data0, const float4 data1234, const float4 data5678, const bool kp = false, const unorm float4 k = 0)
{
    AP_D2Q9_Fi o;
    o.e = 1;
    o.k = 0;
    o.pin = false;
    o.f0 = data0;
    o.f1234 = data1234;
    o.f5678 = data5678;
    return o;
}

/** 使用0方向像素值更新当前像素的速度
   @param data0_c 当前像素0位置的速度
   @return 更新了的值
*/
half ap_d2q9_updateDataF0(const AP_D2Q9_Fi data0_c, const float alpha, const float omega)
{
    return __AP_D2Q9_PRIVATE_UpdateFi(data0_c, data0_c, float2(0, 0), data0_c.f0, data0_c.f0, weight0, alpha, omega);
}

/** 使用1234方向像素值更新当前像素的速度
   @param data1_r 右向(1方向)像素位置的速度
   @param data2_u 2方向像素的速度
   @param data3_l 3方向像素的速度
   @param data4_d 4方向像素的速度
   @return 更新了的该像素数据
*/
half4 ap_d2q9_updateDataF1234(
    const AP_D2Q9_Fi data0_c,
    const AP_D2Q9_Fi data1_r, const AP_D2Q9_Fi data2_u, const AP_D2Q9_Fi data3_l, const AP_D2Q9_Fi data4_d,
    const float alpha, const float omega)
{
    // 用我右边像素的fpre和feq值来更新我的fpre值
    float f3 = __AP_D2Q9_PRIVATE_UpdateFi(data1_r, data0_c, float2(-1,  0), data1_r.f1234.z, data0_c.f1234.x, weight1234, alpha, omega);

    // 用我上边像素的fpre和feq值来更新我的fpre值
    float f4 = __AP_D2Q9_PRIVATE_UpdateFi(data2_u, data0_c, float2( 0, -1), data2_u.f1234.w, data0_c.f1234.y, weight1234, alpha, omega);

    // 用我左边像素的fpre和feq值来更新我的fpre值
    float f1 = __AP_D2Q9_PRIVATE_UpdateFi(data3_l, data0_c, float2( 1,  0), data3_l.f1234.x, data0_c.f1234.z, weight1234, alpha, omega);

    // 用我下边像素的fpre和feq值来更新我的fpre值
    float f2 = __AP_D2Q9_PRIVATE_UpdateFi(data4_d, data0_c, float2( 0,  1), data4_d.f1234.y, data0_c.f1234.w, weight1234, alpha, omega);

    return half4(f1, f2, f3, f4);
}

/** 使用5678像素值更新当前像素的速度
   @param data5_ru 右上向(5方向)像素位置的速度
   @param data6_lu 6方向像素的速度
   @param data7_ld 7方向像素的速度
   @param data8_rd 8方向像素的速度
   @return 更新了的该像素数据
*/
half4 ap_d2q9_updateDataF5678(
    const AP_D2Q9_Fi data0_c,
    const AP_D2Q9_Fi data5_ru, const AP_D2Q9_Fi data6_lu, const AP_D2Q9_Fi data7_ld, const AP_D2Q9_Fi data8_rd,
    const float alpha, const float omega)
{
    // 用我右上边像素的fpre和feq值来更新我的fpre值
    float f7 = __AP_D2Q9_PRIVATE_UpdateFi(data5_ru, data0_c, float2(-sqrt2by2, -sqrt2by2), data5_ru.f5678.z, data0_c.f5678.x, weight5678, alpha, omega);

    // 用我左上边像素的fpre和feq值来更新我的fpre值
    float f8 = __AP_D2Q9_PRIVATE_UpdateFi(data6_lu, data0_c, float2( sqrt2by2, -sqrt2by2), data6_lu.f5678.w, data0_c.f5678.y, weight5678, alpha, omega);

    // 用我左下边像素的fpre和feq值来更新我的fpre值
    float f5 = __AP_D2Q9_PRIVATE_UpdateFi(data7_ld, data0_c, float2( sqrt2by2,  sqrt2by2), data7_ld.f5678.x, data0_c.f5678.z, weight5678, alpha, omega);

    // 用我右下边像素的fpre和feq值来更新我的fpre值
    float f6 = __AP_D2Q9_PRIVATE_UpdateFi(data8_rd, data0_c, float2(-sqrt2by2,  sqrt2by2), data8_rd.f5678.y, data0_c.f5678.w, weight5678, alpha, omega);

    return half4(f5, f6, f7, f8);
}

/** 计算当前像素的宏观速度 */
float2 ap_d2q9_getVelocity(const AP_D2Q9_Fi data)
{
    return __AP_D2Q9_PRIVATE_Velocity(data);
}

/** 仅根据 F1234 和 F5678 计算速度 */
float2 ap_d2q9_getVelocity(const float4 f1234, const float4 f5678)
{
    AP_D2Q9_Fi fi;
    fi.e = 1;
    fi.k = 0;
    fi.pin = false;
    fi.f0 = 0.0;
    fi.f1234 = f1234;
    fi.f5678 = f5678;
    return __AP_D2Q9_PRIVATE_Velocity(fi);
}

/** 计算密度 */
float ap_d2q9_getRho(const AP_D2Q9_Fi fi)
{
    return __AP_D2Q9_PRIVATE_RHO(fi);
}
// --------------------------------------- 这些为内部使用函数, 不建议外用
// 计算阻塞参数
unorm float __AP_D2Q9BB_PRIVATE_K(const unorm float4 k1, const unorm float4 k2)
{
    return ((K0 + K1 * k1.x + K2 * k1.y + K3 * k1.z + K4 * k1.w) +
            (K0 + K1 * k2.x + K2 * k2.y + K3 * k2.z + K4 * k2.w)) / 2.0;
}

// 更新Fi
unorm float __AP_D2Q9_PRIVATE_UpdateFi(const AP_D2Q9_Fi data, const AP_D2Q9_Fi data_c, const float2 dir, const float data_fi, const float data_fk, const float weight, const float alpha, const float omega)
{
    const float rho = __AP_D2Q9_PRIVATE_RHO(data);
    const float v = __AP_D2Q9_PRIVATE_Velocity(data);
    const float feq = __AP_D2Q9_PRIVATE_FEQ(weight, rho, alpha, v, dir);
    const unorm float k = data_c.pin ? 1 : __AP_D2Q9BB_PRIVATE_K(data_c.k, data.k);
    // 减去蒸发量(蒸发量等于蒸发率乘以时间)
    return clamp(__AP_D2Q9_PRIVATE_F(omega, k, data_fi, data_fk, feq) - weight * data_c.e * evaporation, 0, 1);
}

// 适用于SRT(单松弛模型)的平衡态方程
float __AP_D2Q9_PRIVATE_FEQ(const float weight, const float rho, const float alpha, const float2 v, const float2 e_dir)
{
    float Psi = smoothstep(0, alpha, rho);
    float ev = dot(e_dir, v);
    float vv = dot(v, v);

    return weight * (rho + avgRho*Psi*(c_sqrtD3*ev + c_2sqrtSqrtD9*ev*ev - c_2squrD3*vv));
}

// LBGK更新方程 可变渗透率
float __AP_D2Q9_PRIVATE_F(const float omega, const unorm float k, const float data_fi, const float data_fk, const float feq)
{
    const float fi_new = (1 - omega)*data_fi + omega*feq;
    return k * data_fk + (1-k) * fi_new;
}

// 密度计算
float __AP_D2Q9_PRIVATE_RHO(const AP_D2Q9_Fi data)
{
    float o = data.f0 +
        data.f1234.x + data.f1234.y + data.f1234.z + data.f1234.w +
        data.f5678.x + data.f5678.y + data.f5678.z + data.f5678.w;
    
    return o;
}

// 速度计算
float2 __AP_D2Q9_PRIVATE_Velocity(const AP_D2Q9_Fi data)
{
    return
        ((data.f1234.x * float2( 1, 0)) + (data.f1234.y * float2(0,  1)) +
         (data.f1234.z * float2(-1, 0)) + (data.f1234.w * float2(0, -1)) +
         (data.f5678.x * float2( sqrt2by2,  sqrt2by2)) + (data.f5678.y * float2(-sqrt2by2,  sqrt2by2)) +
         (data.f5678.z * float2(-sqrt2by2, -sqrt2by2)) + (data.f5678.w * float2( sqrt2by2, -sqrt2by2))) / avgRho;
}

#endif