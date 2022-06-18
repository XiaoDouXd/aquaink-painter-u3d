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
            #include "PreWrite_Input.cginc"
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
            
            Texture2D _ColTable;
            SamplerState sampler_ColTable;

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                
                // 画圆
                float4 colPre = preCol(i.uv);
                
                float3 colNew = ap_mixbox_kmerp(col.xyz, colPre.xyz, colPre.a, _ColTable, sampler_ColTable);
                float a = saturate(col.a + colPre.a);
                
                return float4(colNew, a);
            }
            ENDHLSL
        }
    }
}
