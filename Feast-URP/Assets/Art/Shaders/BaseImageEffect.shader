Shader "Hidden/BaseImageEffect"
{
    Properties
    {
		[HideInInspector]_MainTex("Texture", 2D) = "white" {}
		[HDR]_FogColor("Fog Color", color) = (1,1,1,1)
		_FogNear("Fog Start Distance", float) = 1
		_FogFar("Fog End Distance", float) = 10
		_FogExp("Fog Exponent", float) = 2
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
			fixed4 _FogColor;
			float _FogNear;
			float _FogFar;
			float _FogExp;

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = tex2D(_MainTex, i.uv);

				float t = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv));
				t = saturate((t - _FogNear) / (_FogFar - _FogNear));
				col = lerp(col, _FogColor, pow(t, _FogExp));
                return col;
            }
            ENDCG
        }
    }
}
