#ifndef AP_D2Q9MODEL_BOUND_BACK_INCLUDED
#define AP_D2Q9MODEL_BOUND_BACK_INCLUDED

// 实现半回弹
#include "d2q9Model.hlsl"

// ------------------------------------- 一些定义 ------
#define K0 0.2
#define K1 0.1
#define K2 0.5
#define K3 0.1
#define K4 0.1

// 储存数据结构体
struct AP_D2Q9_Fik
{
    AP_D2Q9_Fi fi;
    unorm float4 k;
};
float __AP_D2Q9BB_PRIVATE_UpdateFi(const float data_fi, const float4 data_fik, const float data_fk, const float4 data_fkk);
// ----------------------------------------------------
/**
 * @brief 更新1234方向的值
 * @param data1_r 右边像素的fik值
 * @param data2_u 上边像素的fik值
 * @param data3_l 左边像素的fik值
 * @param data4_d 下边像素的fik值
 * @return 更新后f1234向量的值
 */
float4 ap_d2q9bb_updateDataF1234(const AP_D2Q9_Fik data0_c, const AP_D2Q9_Fik data1_r, const AP_D2Q9_Fik data2_u, const AP_D2Q9_Fik data3_l, const AP_D2Q9_Fik data4_d)
{
    const float f1 = __AP_D2Q9BB_PRIVATE_UpdateFi(data3_l.fi.f1234.x, data3_l.k, data0_c.fi.f1234.x, data0_c.k);
    const float f2 = __AP_D2Q9BB_PRIVATE_UpdateFi(data4_d.fi.f1234.y, data4_d.k, data0_c.fi.f1234.y, data0_c.k);
    const float f3 = __AP_D2Q9BB_PRIVATE_UpdateFi(data1_r.fi.f1234.z, data1_r.k, data0_c.fi.f1234.z, data0_c.k);
    const float f4 = __AP_D2Q9BB_PRIVATE_UpdateFi(data2_u.fi.f1234.w, data2_u.k, data0_c.fi.f1234.w, data0_c.k);

    return float4(f1, f2, f3, f4);
}

/**
 * @brief 更新5678方向的值
 * @param data5_ru 右边像素的fik值
 * @param data6_lu 上边像素的fik值
 * @param data7_ld 左边像素的fik值
 * @param data8_rd 下边像素的fik值
 * @return 更新后f1234向量的值
 */
float4 ap_d2q9bb_updateDataF5678(const AP_D2Q9_Fik data0_c, const AP_D2Q9_Fik data5_ru, const AP_D2Q9_Fik data6_lu, const AP_D2Q9_Fik data7_ld, const AP_D2Q9_Fik data8_rd)
{
    const float f5 = __AP_D2Q9BB_PRIVATE_UpdateFi(data7_ld.fi.f5678.x, data7_ld.k, data0_c.fi.f5678.x, data0_c.k);
    const float f6 = __AP_D2Q9BB_PRIVATE_UpdateFi(data8_rd.fi.f5678.y, data8_rd.k, data0_c.fi.f5678.y, data0_c.k);
    const float f7 = __AP_D2Q9BB_PRIVATE_UpdateFi(data5_ru.fi.f5678.z, data5_ru.k, data0_c.fi.f5678.z, data0_c.k);
    const float f8 = __AP_D2Q9BB_PRIVATE_UpdateFi(data6_lu.fi.f5678.w, data6_lu.k, data0_c.fi.f5678.w, data0_c.k);

    return float4(f5, f6, f7, f8);
}

// --------------------------------------- 这些为内部使用函数, 不建议外用
unorm float __AP_D2Q9BB_PRIVATE_K(const float4 k1, const float4 k2)
{
    return ((K0 + K1 * k1.x + K1 * k1.y + K3 * k1.z + K4 * k1.w) +
            (K0 + K1 * k2.x + K2 * k2.y + K3 * k2.z + K4 * k2.w)) / 2.0;
}

float __AP_D2Q9BB_PRIVATE_UpdateFi(const float data_fi, const float4 data_fik, const float data_fk, const float4 data_fkk)
{
    const unorm float k = __AP_D2Q9BB_PRIVATE_K(data_fik, data_fkk);
    return k*data_fk+(1-k)*data_fi;
}

#endif