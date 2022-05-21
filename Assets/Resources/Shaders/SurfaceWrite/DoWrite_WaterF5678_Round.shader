Shader "DoWrite/WaterF5678_Round"
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
            #include "DoWrite_SamplerTrans.cginc"

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
            // 湿度
            unorm float _wet;

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // 画圆
                float dis = getColorAlpha(i.uv);
                dis *= _wet;

                float4 colNew = dis * float4(1.0/36, 1.0/36, 1.0/36, 1.0/36) + (1 - dis) * col;
                
                return colNew;
            }
            ENDHLSL
        }
    }
}
