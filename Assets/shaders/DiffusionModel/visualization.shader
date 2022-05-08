Shader "Hidden/visualization"
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
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "PigmentUpdateModel/colmix.hlsl"
            #include "D2Q9model/d2q9modelU3d.cginc"

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
            
            sampler2D _Adv;
            sampler2D _Fix;
            sampler2D _LastTex0;
            sampler2D _LastTex1234;
            sampler2D _LastTex5678;
            sampler2D _MainTex;

            Texture2D _ColTable;
            SamplerState sampler_ColTable;

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_Fix, i.uv);
                float4 colAdv = tex2D(_Adv, i.uv);

                col = float4(ap_mixbox_kmerp(col.xyz, colAdv.xyz, 0, _ColTable, sampler_ColTable), col.w + colAdv.w);
                col = lerp(float4(1, 1, 1, 1), col, col.w);
                
                return col;
            }
            ENDHLSL
        }
    }
}
