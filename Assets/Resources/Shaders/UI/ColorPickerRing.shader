Shader "AP/UI/ColorPickerRing"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        
        _Hue ("Hue Value", Range(0, 1)) = 0
        _DiameterOuter ("Diameter Of Ring Outer", Range(0, 1)) = 0.95
        _DiameterInner ("Diameter Of Ring Inner", Range(0, 1)) = 0.75
        _RingOffset ("Offset", Range(0, 1)) = 0.0425
        _RingThink ("Think", Range(0, 0.01)) = 0.002
        _RingSize ("Size", Range(0, 1)) = 0.038
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "ColorPickerRing"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                half4  mask : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            float4 _ClipRect;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                OUT.texcoord = v.texcoord;
                OUT.mask = half4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));
                return OUT;
            }

            float3 hsb2rgb( float3 c )
            {
                float3 rgb = clamp( abs(fmod(c.x*6.0+float3(0.0,4.0,2.0),6)-3.0)-1.0, 0, 1);
                rgb = rgb*rgb*(3.0-2.0*rgb);
                return c.z * lerp( float3(1,1,1), rgb, c.y);
            }

            float _Hue;
            float _DiameterOuter;
            float _DiameterInner;
            float _RingOffset;
            float _RingThink;
            float _RingSize;

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord.xy;
	            float2 toCenter = float2(0.5, 0.5) - uv;
	            float angle = atan2(toCenter.y, toCenter.x);
	            float radius = length(toCenter) * 2;
	            float3 col = hsb2rgb(float3((angle / UNITY_TWO_PI) + 0.5, radius, 1.0));

	            float ring = step(_DiameterInner, radius);
	            ring = ring * ( 1 - step(_DiameterOuter, radius));
	            col = lerp(float3(0, 0, 0),col, ring);

	            float ang = -_Hue * UNITY_TWO_PI - UNITY_PI * 0.5;
	            float2 pos = _RingOffset * (float2(sin(ang), cos(ang)));

	            float huering = _RingThink / abs(length(toCenter - pos) - _RingSize);
                
                float4 color = step(huering, 0.5) ? float4(col, ring) : float4(1-col, ring);

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                color.rgb *= color.a;

                return color;
            }
        ENDCG
        }
    }
}
