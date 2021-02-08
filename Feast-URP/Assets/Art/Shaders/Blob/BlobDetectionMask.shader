Shader "Unlit/BlobDetectionMask"
{
    Properties
    {

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
			Cull Off // Turn off backface culling

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			struct v2f
			{
				float4 vertex : POSITION;
				float4 worldVertex : TEXCOORD0;
			};

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (float4 vertex : POSITION)
            {
				v2f o;
				o.vertex = UnityObjectToClipPos(vertex);
				o.worldVertex = mul(vertex, unity_ObjectToWorld);
				return o;
            }

            fixed4 frag (v2f v, fixed facing : VFACE) : SV_Target
            {
				// https://forum.unity.com/threads/is-it-possible-to-determine-which-vertices-are-part-of-a-backface.538187/
				return 1 - facing;
				// If global X is negative, invert value
            }
            ENDCG
        }
    }
}
