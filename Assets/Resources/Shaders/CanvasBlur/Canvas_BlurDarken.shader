Shader "LayerBlur/Darken"
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
            sampler2D _TexCur;

            Texture2D _ColTable;
            SamplerState sampler_ColTable;

            float4 frag (v2f i) : SV_Target
            {
                const float4 col = tex2D(_MainTex, i.uv);
                const float4 colCur = tex2D(_TexCur, i.uv);

                return lerp(colCur, col, col.a);
            }
            ENDHLSL
        }
    }
}
