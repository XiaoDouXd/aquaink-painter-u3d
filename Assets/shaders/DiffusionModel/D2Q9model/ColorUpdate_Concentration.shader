Shader "COL_UPD/ColUpdaten_Concentration"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "d2q9modelU3d.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }
            
            // 上一帧的分量数据
            float2 _Delta;
            sampler2D _Last;
            sampler2D _LastTex0;
            sampler2D _LastTex1234;
            sampler2D _LastTex5678;

            // 更新当前像素的平流浓度 (也就是更新y通道)
            float update_adv(AP_D2Q9_Fi fi, float d_rho, sampler2D last, float2 uv)
            {
                float o = 0.0;
                // 拿01234方向的浓度更新我的浓度
                o += fi.f0 * tex2D(last, uv).y * d_rho;
                o += fi.f1234.x * tex2D(last, uv + float2(-_Delta.x, 0)) * d_rho;
                o += fi.f1234.y * tex2D(last, uv + float2(0, -_Delta.y)) * d_rho;
                o += fi.f1234.z * tex2D(last, uv + float2(_Delta.x, 0)) * d_rho;
                o += fi.f1234.w * tex2D(last, uv + float2(0, _Delta.y)) * d_rho;

                // 拿5678方向来更新我的浓度
                o += fi.f5678.x * tex2D(last, uv + float2(-_Delta.x, -_Delta.y)) * d_rho;
                o += fi.f5678.y * tex2D(last, uv + float2(_Delta.x, -_Delta.y)) * d_rho;
                o += fi.f5678.z * tex2D(last, uv + float2(_Delta.x, _Delta.y)) * d_rho;
                o += fi.f5678.w * tex2D(last, uv + float2(-_Delta.x, _Delta.y)) * d_rho;
                
                return o;
            }

            // xyz 值分别为表面、渗透、沉淀的浓度
            float3 frag (v2f i) : SV_Target
            {
                const AP_D2Q9_Fi fi = tex2D(_LastTex0, _LastTex1234, _LastTex5678, i.uv);
                const float dRho = 1.0 / ap_d2q9_getRho(fi);
                const float y = update_adv(fi, dRho, _Last, i.uv);
                
                return float3(0, y, 0);
            }
            ENDHLSL
        }
    }
}
