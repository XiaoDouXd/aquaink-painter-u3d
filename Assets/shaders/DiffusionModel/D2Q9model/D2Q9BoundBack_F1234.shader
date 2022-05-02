Shader "LBM_SRT/D2Q9BoundBack_F1234"
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
                const AP_D2Q9_Fik f0 = tex2D(_LastTex1234, _LastTex5678, _Paper, _Glue, i.uv);
                const AP_D2Q9_Fik f1 = tex2D(_LastTex1234, _LastTex5678, _Paper, _Glue, i.uv + float2(_Delta.x, 0));
                const AP_D2Q9_Fik f2 = tex2D(_LastTex1234, _LastTex5678, _Paper, _Glue, i.uv + float2(0, _Delta.y));
                const AP_D2Q9_Fik f3 = tex2D(_LastTex1234, _LastTex5678, _Paper, _Glue, i.uv + float2(-_Delta.x, 0));
                const AP_D2Q9_Fik f4 = tex2D(_LastTex1234, _LastTex5678, _Paper, _Glue, i.uv + float2(0, -_Delta.y));
                
                return ap_d2q9bb_updateDataF1234(f0, f1, f2, f3, f4);
            }
            ENDHLSL
        }
    }
}
