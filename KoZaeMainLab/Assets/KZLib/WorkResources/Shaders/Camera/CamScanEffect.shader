Shader "KZLib/Cam/ScanEffect"
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
			sampler2D _CameraDepthTexture;

			float _ScanDistance;
			float _ScanWidth;
			float4 _ScanColor;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv_depth : TEXCOORD1;
			};

			v2f vert(appdata_t _data)
			{
				v2f result;
				result.vertex = UnityObjectToClipPos(_data.vertex);
				result.uv = _data.uv.xy;
				result.uv_depth = _data.uv.xy;

				return result;
			}

			half4 frag(v2f _data) : SV_Target
			{
				half4 color = tex2D(_MainTex,_data.uv);
				float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,_data.uv_depth));


				if(depth < _ScanDistance && depth > _ScanDistance - _ScanWidth && depth < 1.0f)
				{
					_ScanColor *= 1.0f-(_ScanDistance-depth)/(_ScanWidth);

					return color+_ScanColor;
				}

				return color;
			}
			ENDCG
		}
	}
}
