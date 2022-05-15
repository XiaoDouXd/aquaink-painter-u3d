Shader "LBM_SRT/D2Q4Update"
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
            #include "d2q4model.hlsl"

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

            sampler2D _MainTex;
            sampler2D _LastTex;

            float4 frag (v2f i) : SV_Target
            {
                const float4 col_r = tex2D(_LastTex, i.uv + float2(1.0/512.0, 0));
                const float4 col_u = tex2D(_LastTex, i.uv + float2(0, 1.0/512.0));
                const float4 col_l = tex2D(_LastTex, i.uv + float2(-1.0/512.0, 0));
                const float4 col_d = tex2D(_LastTex, i.uv + float2(0, -1.0/512.0));

                const float4 col = ap_d2q4_updateData(col_r, col_u, col_l, col_d, 1, 0.3);
                return col;
            }
            ENDHLSL
        }
    }
}
