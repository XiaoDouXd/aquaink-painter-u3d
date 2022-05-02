Shader "LBM_SRT/D2Q9BoundBack_F5678"
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
            sampler2D _LastTex1234;
            sampler2D _LastTex5678;
            sampler2D _Paper;
            sampler2D _Glue;

            float4 frag (v2f i) : SV_Target
            {
                // PIX_5678
                const AP_D2Q9_Fik f0 = tex2D(_LastTex1234, _LastTex5678, _Paper, _Glue, i.uv);
                const AP_D2Q9_Fik f5 = tex2D(_LastTex1234, _LastTex5678, _Paper, _Glue, i.uv + float2(_Delta.x, _Delta.y));
                const AP_D2Q9_Fik f6 = tex2D(_LastTex1234, _LastTex5678, _Paper, _Glue, i.uv + float2(-_Delta.x, _Delta.y));
                const AP_D2Q9_Fik f7 = tex2D(_LastTex1234, _LastTex5678, _Paper, _Glue, i.uv + float2(-_Delta.x, -_Delta.y));
                const AP_D2Q9_Fik f8 = tex2D(_LastTex1234, _LastTex5678, _Paper, _Glue, i.uv + float2(_Delta.x, -_Delta.y));
                
                return ap_d2q9bb_updateDataF5678(f0, f5, f6, f7, f8);
            }
            ENDHLSL
        }
    }
}
