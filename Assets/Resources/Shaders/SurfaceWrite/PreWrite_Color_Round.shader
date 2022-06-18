Shader "AP/PreWrite/Color_Round"
{
    Properties
    {
        _MainTex ("要写入的贴图", 2D) = "white" {}
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
            #include "PreWrite_Input.cginc"

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

            // _MainTex 为需要写入的贴图
            sampler2D _MainTex;
            float4 _color;

            float _AlphaThreshold;
            float getThresholdAlpha(float alpha)
            {
                return saturate((alpha - _AlphaThreshold) / (1.0 - _AlphaThreshold));
            }

            float4 frag (v2f i) : SV_Target
            {
                // 画圆
                float dis = getThresholdAlpha(preAlpha(i.uv));
                float a = _color.a * dis;
                
                return float4(_color.xyz, a);
            }
            ENDHLSL
        }
    }
}
