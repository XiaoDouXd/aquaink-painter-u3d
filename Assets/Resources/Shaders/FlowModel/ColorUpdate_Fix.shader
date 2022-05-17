Shader "COL_UPD/ColUpdate_Fix"
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
            sampler2D _Adv;
            sampler2D _Fix;
            sampler2D _Flow;

            Texture2D _ColTable;
            SamplerState sampler_ColTable;
            
            float4 frag (v2f i) : SV_Target
            {
                const float factor = ap_getFixtureFactor(tex2D(_Flow, i.uv));

                float4 col_a = tex2D(_Adv, i.uv);
                float4 col_f = tex2D(_Fix, i.uv);
                col_f.w = clamp(col_f.w + col_a.w * factor, 0, 1);
                col_f.xyz = ap_mixbox_kmerp(col_f.xyz, col_a.xyz, (1 - col_a.w * factor), _ColTable, sampler_ColTable);
                
                return col_f;
            }
            ENDHLSL
        }
    }
}
