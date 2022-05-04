Shader "LBM_SRT/D2Q9Update_F1234"
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
            #include "d2q9modelCoefs.hlsl"

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
            sampler2D _Paper;
            sampler2D _Glue;
            sampler2D _LastTex0;
            sampler2D _LastTex1234;
            sampler2D _LastTex5678;

            float4 frag (v2f i) : SV_Target
            {
                // PIX_RULD
                const AP_D2Q9_Fi f0 = ap_tex2D_kp(_LastTex0, _LastTex1234, _LastTex5678, i.uv, _Delta, _Paper, _Glue);
                const AP_D2Q9_Fi f1 = ap_tex2D(_LastTex0, _LastTex1234, _LastTex5678, i.uv + float2(_Delta.x, 0), _Paper, _Glue);
                const AP_D2Q9_Fi f2 = ap_tex2D(_LastTex0, _LastTex1234, _LastTex5678, i.uv + float2(0, _Delta.y), _Paper, _Glue);
                const AP_D2Q9_Fi f3 = ap_tex2D(_LastTex0, _LastTex1234, _LastTex5678, i.uv + float2(-_Delta.x, 0), _Paper, _Glue);
                const AP_D2Q9_Fi f4 = ap_tex2D(_LastTex0, _LastTex1234, _LastTex5678, i.uv + float2(0, -_Delta.y), _Paper, _Glue);
                
                return ap_d2q9_updateDataF1234(f0, f1, f2, f3, f4, alpha, omega);
            }
            ENDHLSL
        }
    }
}
