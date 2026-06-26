Shader "KZLib/TextureMask"
{
	// UI mask reveal: _MaskTex brightness vs _Range drives output alpha; RGB comes from _MainTex.
	// Optional path for TransitionPanel (TransitionStanzaLerp Material mode animates _Range).
	// Default TransitionPanel prefab uses alpha fade instead; this shader is inactive until Material mode is enabled.

	Properties
	{
		[Toggle] _Cutout("Is Cutout",Float) = 1.0   // Hard step vs soft smoothstep edge
		[Toggle] _Reverse("Is Reverse",Float) = 0.0 // Invert the computed mask alpha
		_MaskTex("Mask Texture",2D) = "white" {}

		[HideInInspector] _MainTex("Main Texture (RGBA)",2D) = "white" {}

		// Set automatically by Unity UI (Image, Mask, RectMask2D)
		[HideInInspector] _StencilComp("Stencil Comparison",Float) = 8.0
		[HideInInspector] _Stencil("Stencil ID",Float) = 0.0
		[HideInInspector] _StencilOp("Stencil Operation",Float) = 0.0
		[HideInInspector] _StencilWriteMask("Stencil Write Mask",Float) = 255.0
		[HideInInspector] _StencilReadMask("Stencil Read Mask",Float) = 255.0

		[HideInInspector] _ColorMask("Color Mask",Float) = 15.0

		_Range("Range",Range(0,1)) = 0.0 // Reveal threshold (0 = hidden, 1 = fully visible)
	}

	SubShader
	{
		Tags
		{
			"Queue"				= "Transparent"
			"IgnoreProjector"	= "True"
			"RenderType"		= "Transparent"
			"PreviewType"		= "Plane"
			"CanUseSpriteAtlas"	= "True"
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

		// UI defaults (aligned with TextureFlash.shader)
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			static const float SoftEdgeHalfWidth = 0.1;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};

			v2f vert(appdata_t input)
			{
				v2f result;

				result.vertex = UnityObjectToClipPos(input.vertex);
				result.texcoord = input.texcoord;
				result.color = input.color; // Image.color tint (RGB; alpha handled in frag)

				return result;
			}

			sampler2D _MainTex;
			sampler2D _MaskTex;
			float _Range;
			float _Cutout;
			float _Reverse;

			fixed4 frag(v2f input) : SV_Target
			{
				fixed4 texColor = tex2D(_MainTex,input.texcoord)*input.color;
				float alpha = 0.0;

				// Tolerant endpoints for tween values that never hit exactly 0 or 1
				if(_Range <= 0.0)
				{
					alpha = 0.0;
				}
				else if(_Range >= 1.0)
				{
					alpha = 1.0;
				}
				else
				{
					float4 maskColor = tex2D(_MaskTex,input.texcoord);
					// Grayscale RGB masks and alpha-only mask textures
					float brightness = max(maskColor.a,max(maskColor.r,max(maskColor.g,maskColor.b)));

					if(_Cutout > 0.5)
					{
						// Hard mask: visible where brightness is at or below the threshold
						alpha = brightness <= _Range ? 1.0 : 0.0;
					}
					else
					{
						// Soft edge around _Range in brightness space
						alpha = 1.0-smoothstep(_Range-SoftEdgeHalfWidth,_Range+SoftEdgeHalfWidth,brightness);
					}
				}

				if(_Reverse > 0.5)
				{
					alpha = 1.0-alpha;
				}

				// Material mode: mask alpha fully controls visibility (Image.color.a is not multiplied).
				// Fade mode uses the default UI shader instead and tweens Image.color alpha in C#.
				texColor.a = alpha;

				return texColor;
			}
			ENDCG
		}
	}
}
