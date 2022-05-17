Shader "LBM_SRT/D2Q9Update_F0"
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
            sampler2D _Fix;
            sampler2D _LastTex0;
            sampler2D _LastTex1234;
            sampler2D _LastTex5678;
            
            float2 frag (v2f i) : SV_Target
            {
                // PIX_C
                const AP_D2Q9_Fi f0 = ap_tex2D_kp(_LastTex0, _LastTex1234, _LastTex5678, i.uv, _Delta, _Paper, _Glue, _Fix);
                const float r = ap_d2q9_getRho(f0);
                const float o = ap_d2q9_updateDataF0(f0, alpha, omega);
                
                return float2(o, r);
            }
            ENDHLSL
        }
    }
}
