﻿Shader "LBM_SRT/D2Q9Update_Show"
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
            sampler2D _LastTex0;
            sampler2D _LastTex1234;
            sampler2D _LastTex5678;
            
            float4 frag (v2f i) : SV_Target
            {
                const AP_D2Q9_Fi f0 = ap_tex2D(_LastTex0, _LastTex1234, _LastTex5678, i.uv);
                const float r = ap_d2q9_getRho(f0);
                const float2 v = ap_d2q9_getVelocity(f0);
                
                return float4(r, tex2D(_LastTex0, i.uv).y, v);
            }
            ENDHLSL
        }
    }
}
