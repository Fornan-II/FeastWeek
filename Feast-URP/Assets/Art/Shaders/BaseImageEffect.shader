Shader "Hidden/BaseImageEffect"
{
    Properties
    {
		[HideInInspector]_MainTex("Texture", 2D) = "white" {}
		[NoScaleOffset]_UVNoise("UV Noise", 2D) = "bump" {}
		//_NoiseStrength("Noise Strength", float) = 0.002
		_NoiseScale("Noise Scale", float) = 0.1
		_NoiseSpeed("Noise Speed", float) = 64
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

			sampler2D _UVNoise;
			float _NoiseStrength;
			float _NoiseScale;
			float _NoiseSpeed;
			float _Saturation;

			fixed4 frag(v2f i) : SV_Target
			{
				//float2 offset = (2 * (tex2D(_UVNoise, i.uv * _NoiseScale + _NoiseSpeed * _Time.xx).rg) - 1) * _NoiseStrength;
				float2 offset = tex2D(_UVNoise, i.uv * _NoiseScale + _NoiseSpeed * _Time.xx).rg;
				float theta = offset.x * UNITY_TWO_PI;
				offset = offset.y * float2(cos(theta), sin(theta)) * _NoiseStrength;
				fixed4 col = tex2D(_MainTex, i.uv + offset);

				float desaturated = col.r * 0.21f + col.g * 0.72f + col.b * 0.7f;
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
