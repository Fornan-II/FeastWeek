Shader "Unlit/Blob_Bones"
{
	Properties
	{
		_Color("Color", color) = (0,0,0,1)
		_Extrusion("Extrusion Factor", float) = 0
		_Scale("Extrusion Scale", float) = 1
        _NoiseTex ("Reveal Noise Texture", 2D) = "white" {}
		_NoiseScroll("Noise Scroll Speed", float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct v2f
            {
                float reveal : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

			fixed4 _Color;
			float _Extrusion;
			float _Scale;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			float _NoiseScroll;

            v2f vert (appdata_base v)
            {
                v2f o;
				float2 uv = TRANSFORM_TEX(v.texcoord, _NoiseTex);
				o.reveal = tex2Dlod(_NoiseTex, float4(uv + _Time.x * _NoiseScroll, 0, 0)).r;
				o.reveal *= tex2Dlod(_NoiseTex, float4(uv - _Time.x * _NoiseScroll, 0, 0)).r;
				o.reveal += _Extrusion;
				float4 objectPos = mul(v.vertex, unity_WorldToObject) + float4(v.normal * max(o.reveal, 0) * _Scale, 0);
				o.vertex = UnityObjectToClipPos(mul(objectPos, unity_ObjectToWorld));
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				clip(step(0, i.reveal) - 0.5);
				fixed4 col = _Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
