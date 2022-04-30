Shader "LBM_SRT/D2Q9Calc_Velocity"
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
            #include "d2q9model.hlsl"

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
            Texture2D _LastTex0;
            Texture2D _LastTex1234;
            Texture2D _LastTex5678;
            SamplerState sampler_LastTex5678;

            float2 frag (v2f i) : SV_Target
            {
                const AP_D2Q9_Fi fi = tex2D(_LastTex0, _LastTex1234, _LastTex5678, sampler_LastTex5678, i.uv);
                float a = __AP_D2Q9_PRIVATE_Velocity(fi);
                return float4(a, a, a, 1);//ap_d2q9_getVelocity(fi);
            }
            ENDHLSL
        }
    }
}
