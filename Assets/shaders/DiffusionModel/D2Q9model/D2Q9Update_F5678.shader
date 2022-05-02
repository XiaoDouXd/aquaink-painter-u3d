Shader "LBM_SRT/D2Q9Update_F5678"
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
            sampler2D _LastTex0;
            sampler2D _LastTex1234;
            sampler2D _LastTex5678;

            float4 frag (v2f i) : SV_Target
            {
                // PIX_5678
                const AP_D2Q9_Fi f5 = tex2D(_LastTex0, _LastTex1234, _LastTex5678, i.uv + float2(_Delta.x, _Delta.y));
                const AP_D2Q9_Fi f6 = tex2D(_LastTex0, _LastTex1234, _LastTex5678, i.uv + float2(-_Delta.x, _Delta.y));
                const AP_D2Q9_Fi f7 = tex2D(_LastTex0, _LastTex1234, _LastTex5678, i.uv + float2(-_Delta.x, -_Delta.y));
                const AP_D2Q9_Fi f8 = tex2D(_LastTex0, _LastTex1234, _LastTex5678, i.uv + float2(_Delta.x, -_Delta.y));
                
                return ap_d2q9_updateDataF5678(f5, f6, f7, f8, alpha, omega);
            }
            ENDHLSL
        }
    }
}
