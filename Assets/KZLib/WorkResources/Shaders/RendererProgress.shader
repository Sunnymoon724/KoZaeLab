Shader "KZLib/RendererProgress"
{
	Properties
	{
		_Color("Color", Color) = (1.0,1.0,1.0,1.0)
		_Progress("Progress",Range(0.0,1.0)) = 0.0

		[Toggle] _Horizontal("Is Horizontal",Float) = 1.0
		[Toggle] _Reverse("Is Reverse",Float) = 0.0

		[HideInInspector] _MainTex("Main Tex (RGBA)", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Overlay+1"
		}

		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				
				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _Color;
				float _Progress;
				float _Horizontal;
				float _Reverse;
				
				struct v2f
				{
					float4 pos : POSITION;
					float2 uv : TEXCOORD0;
				};
				
				v2f vert(appdata_base v)
				{
					v2f result;

					result.pos = UnityObjectToClipPos(v.vertex);
					result.uv = TRANSFORM_TEX(v.texcoord,_MainTex);

					return result;
				}

				float4 frag(v2f input) : SV_Target
				{
					float4 color = tex2D(_MainTex,input.uv);

					if(_Horizontal > 0.5)
					{
						color.a *= _Reverse > 0.5 ? step(1.0-_Progress,input.uv.x) : step(input.uv.x,_Progress);
					}
					else
					{
						color.a *= _Reverse > 0.5 ? step(1.0-_Progress,input.uv.y) : step(input.uv.y,_Progress);
					}

					return color * _Color;
				}
			ENDCG
		}
	}
}