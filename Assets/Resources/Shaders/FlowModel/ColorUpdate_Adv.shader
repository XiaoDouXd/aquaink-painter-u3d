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
            sampler2D _Flow;

            Texture2D _ColTable;
            SamplerState sampler_ColTable;
            
            float4 frag (v2f i) : SV_Target
            {
                float4 flow = tex2D(_Flow, i.uv);
                float2 v = -float2(flow.z *  _Delta.x, flow.w * _Delta.y );
                
                const float4 pf = tex2D(_Last, i.uv);
                const float4 pfn = tex2D(_Last, i.uv + v);
                const float gamma = lerp(1, 0.1, smoothstep(0, 0.5, length(v)));
                const float3 col = ap_mixbox_kmerp(pf.xyz, pfn.xyz, gamma* pfn.w, _ColTable, sampler_ColTable);
                float a = lerp(pf.w, pfn.w, gamma*pfn.w);

                const float factor = ap_getFixtureFactor(flow);
                a = clamp(a - factor * a, 0, 1);
                
                return float4(col, a);
            }
            ENDHLSL
        }
    }
}
