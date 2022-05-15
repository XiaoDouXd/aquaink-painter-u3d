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
            
            #include "../ColmixModel/colmix.hlsl"

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
            
            float4 frag (v2f i) : SV_Target
            {
                float2 v = tex2D(_LastTex0, i.uv).zw;
                v = float2(v.x *  _Delta.x, v.y * _Delta.y );
                
                const float4 pf = tex2D(_Last, i.uv);
                const float4 pfn = tex2D(_Last, i.uv + v);
                const float gamma = lerp(1, 0.1, smoothstep(0, 0.5, length(v)));
                const float3 col = ap_mixbox_kmerp(pf.xyz, pfn.xyz, gamma* pfn.w, _ColTable, sampler_ColTable);
                float a = lerp(pfn.w, pf.w, gamma*pfn.w);

                const float factor = ap_getFixtureFactor(_LastTex0, _LastTex1234, _LastTex5678, i.uv);
                a = clamp(a - factor * a, 0, 1);
                
                return float4(col, a);
            }
            ENDHLSL
        }
    }
}
