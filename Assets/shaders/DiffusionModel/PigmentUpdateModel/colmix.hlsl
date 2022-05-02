#ifndef AP_COLMIX_INCLUDED
#define AP_COLMIX_INCLUDED

#include "colmixCoefs.hlsl"

// ------------------ 一些内部使用函数和变量的定义 ------
struct AP_COLMIX_Latent {
    float4 c;
    float3 subrgb;
};
AP_COLMIX_Latent __AP_COLMIX_PRIVATE_rgbToLatent(const float3 rgb, Texture2D c_table, const SamplerState samplerState);
float3 __AP_COLMIX_PRIVATE_latentToRgb(AP_COLMIX_Latent latent);
// ----------------------------------------------------

// 颜色混合
// @param col_1 颜色1
// @param col_2 颜色2
// @param t 混合系数
// @param col_mapping_table 混色采样表
// @return 混合结果
float3 ap_mixbox_kmerp(const float3 col_1, const float3 col_2, const float t, const Texture2D col_mapping_table, const SamplerState samplerState)
{
    // 转换色彩空间
    AP_COLMIX_Latent c_1 = __AP_COLMIX_PRIVATE_rgbToLatent(col_1, col_mapping_table, samplerState);
    AP_COLMIX_Latent c_2 = __AP_COLMIX_PRIVATE_rgbToLatent(col_2, col_mapping_table, samplerState);

    AP_COLMIX_Latent c_out;
    c_out.c = lerp(c_1.c, c_2.c, float4(t, t, t, t));
    c_out.subrgb = lerp(c_1.subrgb, c_2.subrgb, float3(t, t, t));

    return __AP_COLMIX_PRIVATE_latentToRgb(c_out);
}

// --------------------------------------- 这些为内部使用函数, 不建议外用
float3 __AP_COLMIX_PRIVATE_mix(float4 c)
{
    const float c00 = c.x * c.x;
    const float c11 = c.y * c.y;
    const float c22 = c.z * c.z;
    const float c33 = c.w * c.w;
    const float c01 = c.x * c.y;
    const float c02 = c.x * c.z;

    float3 rgb = float3(0, 0, 0);

    // 根据权值计算 rgb
    rgb += c.x*c00 * coefs[0];
    rgb += c.y*c11 * coefs[1];
    rgb += c.z*c22 * coefs[2];
    rgb += c.w*c33 * coefs[3];
    rgb += c00*c.y * coefs[4];
    rgb += c01*c.y * coefs[5];
    rgb += c00*c.z * coefs[6];
    rgb += c02*c.z * coefs[7];
    rgb += c00*c.w * coefs[8];
    rgb += c.x*c33 * coefs[9];
    rgb += c11*c.z * coefs[10];
    rgb += c.y*c22 * coefs[11];
    rgb += c11*c.w * coefs[12];
    rgb += c.y*c33 * coefs[13];
    rgb += c22*c.w * coefs[14];
    rgb += c.z*c33 * coefs[15];
    rgb += c01*c.z * coefs[16];
    rgb += c01*c.w * coefs[17];
    rgb += c02*c.w * coefs[18];
    rgb += c.y*c.z*c.w * coefs[19];

    return rgb;
}

AP_COLMIX_Latent __AP_COLMIX_PRIVATE_rgbToLatent(const float3 rgb, Texture2D c_table, const SamplerState samplerState)
{
    // 将颜色扩张到 0-255
    const float3 c_rgb = clamp(rgb, float3(0, 0, 0), float3(1, 1, 1)) * 255.0;

    // 拿到整数部分和小数部分
    uint3 i_rgb;                                 // 整数部分
    const float3 t_rgb = modf(c_rgb, i_rgb);    // 小数部分

    // 拿到对应在贴图中的位置
    const uint2 pos  = uint2(((i_rgb.b       % 16) * 256 + i_rgb.r    ), ((i_rgb.b       / 16) * 256 + i_rgb.g    ));
    const uint2 pos1 = uint2(((i_rgb.b       % 16) * 256 + i_rgb.r + 1), ((i_rgb.b       / 16) * 256 + i_rgb.g    ));
    const uint2 pos2 = uint2(((i_rgb.b       % 16) * 256 + i_rgb.r    ), ((i_rgb.b       / 16) * 256 + i_rgb.g + 1));
    const uint2 pos3 = uint2(((i_rgb.b       % 16) * 256 + i_rgb.r + 1), ((i_rgb.b       / 16) * 256 + i_rgb.g + 1));
    const uint2 pos4 = uint2((((i_rgb.b + 1) % 16) * 256 + i_rgb.r    ), (((i_rgb.b + 1) / 16) * 256 + i_rgb.g    ));
    const uint2 pos5 = uint2((((i_rgb.b + 1) % 16) * 256 + i_rgb.r + 1), (((i_rgb.b + 1) / 16) * 256 + i_rgb.g    ));
    const uint2 pos6 = uint2((((i_rgb.b + 1) % 16) * 256 + i_rgb.r    ), (((i_rgb.b + 1) / 16) * 256 + i_rgb.g + 1));
    const uint2 pos7 = uint2((((i_rgb.b + 1) % 16) * 256 + i_rgb.r + 1), (((i_rgb.b + 1) / 16) * 256 + i_rgb.g + 1));

    // 防止溢出, 在下面的判断中, 溢出则处理为零
    i_rgb.r = i_rgb.r + 1;
    i_rgb.b = i_rgb.b + 1;
    i_rgb.g = i_rgb.g + 1;
    
    // 神秘的c
    float4 c = float4(0, 0, 0, 0);
    c.rgb +=                                        (1-t_rgb.r)*(1-t_rgb.g)*(1-t_rgb.b)*c_table.Sample(samplerState, (pos )/4096.0).rgb;
    c.rgb += i_rgb.r >= 255 ? 0 :                     (t_rgb.r)*(1-t_rgb.g)*(1-t_rgb.b)*c_table.Sample(samplerState, (pos1)/4096.0).rgb;
    c.rgb += i_rgb.g >= 255 ? 0 :                   (1-t_rgb.r)*  (t_rgb.g)*(1-t_rgb.b)*c_table.Sample(samplerState, (pos2)/4096.0).rgb;
    c.rgb += i_rgb.g >= 255 || i_rgb.r >= 255 ? 0 :   (t_rgb.r)*  (t_rgb.g)*(1-t_rgb.b)*c_table.Sample(samplerState, (pos3)/4096.0).rgb;
    c.rgb += i_rgb.b >= 255 ? 0 :                   (1-t_rgb.r)*(1-t_rgb.g)*  (t_rgb.b)*c_table.Sample(samplerState, (pos4)/4096.0).rgb;
    c.rgb += i_rgb.b >= 255 || i_rgb.r >= 255 ? 0 :   (t_rgb.r)*(1-t_rgb.g)*  (t_rgb.b)*c_table.Sample(samplerState, (pos5)/4096.0).rgb;
    c.rgb += i_rgb.b >= 255 || i_rgb.g >= 255 ? 0 : (1-t_rgb.r)*  (t_rgb.g)*  (t_rgb.b)*c_table.Sample(samplerState, (pos6)/4096.0).rgb;
    c.rgb += i_rgb.b >= 255 || i_rgb.g >= 255 || i_rgb.r >= 255 ? 0 : (t_rgb.r)*(t_rgb.g)*(t_rgb.b)*c_table.Sample(samplerState, (pos7)/4096.0).rgb;
    c.a = 1.0 - (c.r + c.g + c.b);

    // 拿到subrgb
    float3 mixrgb = __AP_COLMIX_PRIVATE_mix(c);
    AP_COLMIX_Latent o;
    o.c = c;
    o.subrgb = rgb - mixrgb;

    return o;
}

float3 __AP_COLMIX_PRIVATE_latentToRgb(AP_COLMIX_Latent latent)
{
    float3 rgb = __AP_COLMIX_PRIVATE_mix(latent.c);
    return clamp(rgb + latent.subrgb, float3(0, 0, 0), float3(1, 1, 1));
}

#endif