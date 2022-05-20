Shader "DoWrite/Color_Round"
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

            // _MainTex 为需要写入的贴图
            sampler2D _MainTex;
            // _DestTex 为目标贴图
            sampler2D _DestTex;
            // 方形
            float4 _rect;
            float _lerp;

            float4 _color;
            Texture2D _ColTable;
            SamplerState sampler_ColTable;

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // 画圆
                const unorm float DIST = abs(_rect.x - _rect.z) / 2.0;
                
                const float asepct = abs(_rect.z - _rect.x) / abs(_rect.y - _rect.w);
                float2 disVec = i.uv - (_rect.xy + _rect.zw) / 2.0;
                disVec = asepct > 1 ? float2(disVec.x / asepct, disVec.y) :
                    float2(disVec.x, disVec.y * asepct);
                float dis = length(disVec);
                
                dis = dis <= DIST ? 1 - dis / DIST : 0;
                _color.a *= dis;
                
                float3 colNew = ap_mixbox_kmerp(col.xyz, _color.xyz, _color.a, _ColTable, sampler_ColTable);
                float a = col.a + _color.a;
                
                return float4(colNew, a);
            }
            ENDHLSL
        }
    }
}
