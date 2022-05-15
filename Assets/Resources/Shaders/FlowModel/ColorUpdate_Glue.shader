Shader "COL_UPD/ColUpdate_Glue"
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
            
            float frag (v2f i) : SV_Target
            {
                const AP_D2Q9_Fi fi = ap_tex2D(_LastTex0, _LastTex1234, _LastTex5678, i.uv);
                float2 v = ap_d2q9_getVelocity(fi);
                v = float2(v.x * (1/10.0), v.y * (1/10.0) );
                
                const float pf = tex2D(_Last, i.uv).r;
                const float pfn = tex2D(_Last, i.uv + v).r;
                const float gamma = lerp(1, 0.1, smoothstep(0, 0.5, length(v)));
                float o = lerp(pf, pfn, gamma*pfn);
                o -= 0.001;         // 蒸发
                
                return o;
            }
            ENDHLSL
        }
    }
}
