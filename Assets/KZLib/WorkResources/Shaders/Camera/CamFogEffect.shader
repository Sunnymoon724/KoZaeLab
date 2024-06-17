Shader "KZLib/Cam/FogEffect"
{
	Properties
	{
		[HideInInspector] _MainTex ("Texture",2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			float4 _MainTex_ST;
			
			v2f vert(appdata_t _data)
			{
				v2f result;
				result.vertex = UnityObjectToClipPos(_data.vertex);
				result.uv = TRANSFORM_TEX(_data.uv,_MainTex);

				return result;
			}
			
			fixed4 frag(v2f _data) : SV_Target
			{
				return Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture,_data.uv)));
			}

			ENDCG
		}
	}
}