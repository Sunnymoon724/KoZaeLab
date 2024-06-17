Shader "KZLib/Cam/NoiseEffect"
{
	Properties
	{
		[HideInInspector] _MainTex("Base",2D) = "white" {}
	}

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _Intensity;
			sampler2D _Noise;

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

			v2f vert(appdata_t _data)
			{
				v2f result;
				result.vertex = UnityObjectToClipPos(_data.vertex);
				result.uv = _data.uv;

				return result;
			}

			float4 frag(v2f _data) : SV_Target
			{
				float2 center = float2(0.5,0.5);
				float2 redUV = _data.uv;
				float2 greenUV = lerp(_data.uv,center,0.05*_Intensity);
				float2 blueUV = lerp(_data.uv,center,0.1*_Intensity);
				
				float4 result = float4(tex2D(_MainTex,redUV).r,tex2D(_MainTex,greenUV).g,tex2D(_MainTex,blueUV).b,1.0);
				
				result += float4(tex2D(_Noise,redUV).a*_Intensity,tex2D(_Noise,greenUV).a*_Intensity,tex2D(_Noise,blueUV).a*_Intensity,0.0);

				return result;
			}
			ENDCG
		}
	}
}
