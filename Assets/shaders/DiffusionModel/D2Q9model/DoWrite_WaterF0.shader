Shader "AP/DoWrite_WaterF0"
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
            // _DestTex 为目标贴图
            sampler2D _DestTex;
            // 方形
            float4 _rect;

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_DestTex, i.uv);
                // const float4 colNew = abs(tex2D(_MainTex, mainUV));
                // 画圆
                const unorm float DIST = abs(_rect.x - _rect.z) / 2.0;
                float dis = distance(i.uv, (_rect.xy + _rect.zw)/2.0);
                dis = dis <= DIST ? 1 - dis / DIST : 0;
                float4 colNew = dis * float4(4.0/9, 4.0/9, 4.0/9, 4.0/9) + (1 - dis) * col;
                
                return colNew;
            }
            ENDHLSL
        }
    }
}
