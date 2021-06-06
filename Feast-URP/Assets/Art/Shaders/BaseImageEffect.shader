﻿Shader "Hidden/BaseImageEffect"
{
    Properties
    {
		[HideInInspector]_MainTex("Texture", 2D) = "white" {}
		_Saturation("Saturation", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			fixed _InvertValue;
			fixed4 _FadeColor;
			float _ScreenFade;
			float _Saturation;

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = tex2D(_MainTex, i.uv);
				float desaturated = col.r * 0.21f + col.g * 0.72 * col.b * 0.7;
				//Invert value
				col =
					(1 - _InvertValue) * col
					+ (_InvertValue) * (1 - lerp(float4(desaturated, desaturated, desaturated, 1), col, _Saturation));
				//Fade
				col = lerp(col, _FadeColor, _ScreenFade);
                return col;
            }
            ENDCG
        }
    }
}
