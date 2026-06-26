Shader "KZLib/RendererProgress"
{
	// Hard-edge progress wipe: clips texture alpha by UV vs _Progress.
	// Overlay use (ZTest Always). Also works on UI Image when vertex color / stencil are set.

	Properties
	{
		_Color("Color", Color) = (1.0,1.0,1.0,1.0)
		_Progress("Progress",Range(0.0,1.0)) = 0.0

		[Toggle] _Horizontal("Is Horizontal",Float) = 1.0
		[Toggle] _Reverse("Is Reverse",Float) = 0.0

		[HideInInspector] _MainTex("Main Tex (RGBA)", 2D) = "white" {}

		// Set automatically by Unity UI (Image, Mask, RectMask2D)
		[HideInInspector] _StencilComp("Stencil Comparison",Float) = 8.0
		[HideInInspector] _Stencil("Stencil ID",Float) = 0.0
		[HideInInspector] _StencilOp("Stencil Operation",Float) = 0.0
		[HideInInspector] _StencilWriteMask("Stencil Write Mask",Float) = 255.0
		[HideInInspector] _StencilReadMask("Stencil Read Mask",Float) = 255.0

		[HideInInspector] _ColorMask("Color Mask",Float) = 15.0
	}

	SubShader
	{
		Tags
		{
			"Queue"				= "Overlay+1"
			"IgnoreProjector"	= "True"
			"RenderType"		= "Transparent"
			"PreviewType"		= "Plane"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		ColorMask [_ColorMask]

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Always // Draw on top of scene geometry (overlay progress)
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

				struct appdata_t
				{
					float4 vertex	: POSITION;
					float4 color	: COLOR;
					float2 texcoord	: TEXCOORD0;
				};

				struct v2f
				{
					float4 pos			: SV_POSITION;
					float4 color		: COLOR;
					float2 uv			: TEXCOORD0;
					float2 progressUV	: TEXCOORD1;
				};

				v2f vert(appdata_t input)
				{
					v2f result;

					result.pos = UnityObjectToClipPos(input.vertex);
					result.uv = TRANSFORM_TEX(input.texcoord,_MainTex);
					result.progressUV = input.texcoord; // Wipe uses 0-1 mesh UV, not material tiling
					result.color = input.color;

					return result;
				}

				float4 frag(v2f input) : SV_Target
				{
					float4 color = tex2D(_MainTex,input.uv)*input.color;

					// Alpha-only wipe; RGB silhouette comes from the texture
					if(_Horizontal > 0.5)
					{
						color.a *= _Reverse > 0.5 ? step(1.0-_Progress,input.progressUV.x) : step(input.progressUV.x,_Progress);
					}
					else
					{
						color.a *= _Reverse > 0.5 ? step(1.0-_Progress,input.progressUV.y) : step(input.progressUV.y,_Progress);
					}

					return color*_Color;
				}
			ENDCG
		}
	}
}