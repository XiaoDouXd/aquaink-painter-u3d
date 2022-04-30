Shader "Hidden/mixboxMat"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "colmix.hlsl"

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

            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            float3 _Color1;
            float3 _Color2;

            float4 frag (v2f i) : SV_Target
            {
                float3 c1 = _Color1;
                float3 c2 = _Color2;
                float3 c3 = float3(0.2, 0.15, 0.8);
                
                float k = i.uv.x;
                
                float3 col = ap_mixbox_kmerp(c1, c2, k, _MainTex, sampler_MainTex);
                float3 col2 = lerp(c1, c2, k);
                
                return i.uv.y > 0.5 ? float4(col, 1) : float4(col2, 1);
            }
            ENDCG
        }
    }
}
