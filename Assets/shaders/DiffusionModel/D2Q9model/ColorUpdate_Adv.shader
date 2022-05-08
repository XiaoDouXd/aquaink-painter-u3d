Shader "COL_UPD/ColUpdate_Adv"
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
            
            #include "../PigmentUpdateModel/colmix.hlsl"

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

            Texture2D _ColTable;
            SamplerState sampler_ColTable;

            void get_ChangeCol(const unorm float2 uv, inout float4 newCol)
            {
                const float2 v = ap_d2q9_getVelocity(ap_tex2D(_LastTex1234, _LastTex5678, uv));
                const float p = dot(normalize(uv), v);
                
                const float4 la = tex2D(_Last, uv);

                newCol = p <= 0 ? float4(newCol.xyz, newCol.w + p * la.w) :
                float4(ap_mixbox_kmerp(newCol.xyz, la.xyz, p * la.w, _ColTable, sampler_ColTable), newCol.w + p * la.w);
            }
            
            float4 frag (v2f i) : SV_Target
            {
                const AP_D2Q9_Fi fi = ap_tex2D(_LastTex0, _LastTex1234, _LastTex5678, i.uv);
                float2 v = ap_d2q9_getVelocity(fi);
                v = float2(v.x * (1/10.0), v.y * (1/10.0) );
                
                const float4 pf = tex2D(_Last, i.uv);
                const float4 pfn = tex2D(_Last, i.uv + v);
                const float gamma = lerp(1, 0.1, smoothstep(0, 0.5, length(v)));
                const float3 col = ap_mixbox_kmerp(pf.xyz, pfn.xyz, gamma*pfn.w, _ColTable, sampler_ColTable);
                float a = lerp(pfn.w, pf.w, gamma*pfn.w);

                const float factor = ap_getFixtureFactor(_LastTex0, _LastTex1234, _LastTex5678, i.uv);
                a = clamp(a - factor * a, 0, 1);
                
                return float4(col, a);
            }
            ENDHLSL
        }
    }
}
