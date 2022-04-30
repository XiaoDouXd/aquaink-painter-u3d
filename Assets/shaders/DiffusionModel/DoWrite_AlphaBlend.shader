Shader "AP/DoWrite_AlphaBlend"
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
            #include "doWrite_clamp.hlsl"

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
                const float2 mainUV = float2((i.uv.x - _rect.x) / (_rect.z - _rect.x), (i.uv.y - _rect.y) / (_rect.w - _rect.y));
                
                float4 col = tex2D(_DestTex, i.uv);
                const float4 colNew = tex2D(_MainTex, mainUV);

                col = in01(mainUV) ? col : lerp(colNew, col, colNew.a);
                
                return col;
            }
            ENDHLSL
        }
    }
}
