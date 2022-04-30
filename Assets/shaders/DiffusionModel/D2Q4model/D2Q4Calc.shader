Shader "LBM_SRT/D2Q4Calc"
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

            Texture2D _LastTex;
            SamplerState sampler_LastTex;

            float2 frag (v2f i) : SV_Target
            {
                return  ap_d2q4_getVelocity(_LastTex.Sample(sampler_LastTex, i.uv));
            }
            ENDHLSL
        }
    }
}
