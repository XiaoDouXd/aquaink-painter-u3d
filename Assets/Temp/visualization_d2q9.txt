Shader "Hidden/d2q9_visualization"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LastTex0 ("TextureLast0", 2D) = "white" {}
        _LastTex1234 ("TextureLast1234", 2D) = "white" {}
        _LastTex5678 ("TextureLast5678", 2D) = "white" {}
    }
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

            sampler2D _MainTex;
            // 上一帧的分量数据
            sampler2D _LastTex0;
            sampler2D _LastTex1234;
            sampler2D _LastTex5678;

            float4 frag (v2f i) : SV_Target
            {
                const AP_D2Q9_Fi fi = ap_tex2D(_LastTex0, _LastTex1234, _LastTex5678, i.uv);
                float2 tmp = ap_d2q9_getVelocity(fi);

                return float4(tmp, 0, 1);
            }
            ENDHLSL
        }
    }
}
