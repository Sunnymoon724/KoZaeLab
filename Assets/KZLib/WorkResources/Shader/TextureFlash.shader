Shader "KZLib/TextureFlash"
{
	// UI sprite flash: brightens visible pixels toward _FlashColor without changing the alpha mask.
	// Typical use: hit flash, emphasis pulse (_FlashAmount animated 0 → 1 → 0).

	Properties
	{
		_FlashColor("Flash Color",Color) = (1.0,1.0,1.0,1.0) // Tint target; only RGB is used
		_FlashAmount("Flash Amount",Range(0.0,1.0)) = 0.0   // Blend strength (0 = original, 1 = full flash color)

		// Set automatically by Unity UI (Image, Mask, RectMask2D)
		[HideInInspector] _StencilComp("Stencil Comparison",Float) = 8.0		[HideInInspector] _Stencil("Stencil ID",Float) = 0.0
		[HideInInspector] _StencilOp("Stencil Operation",Float) = 0.0
		[HideInInspector] _StencilWriteMask("Stencil Write Mask",Float) = 255.0
		[HideInInspector] _StencilReadMask("Stencil Read Mask",Float) = 255.0

		[HideInInspector] _ColorMask("Color Mask",Float) = 15.0
		[Space]
		_MainTex("Main Texture (RGBA)",2D) = "white" {}
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

		// UI defaults (aligned with TextureMask.shader)
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

				struct appdata_t
				{
					float4 vertex	: POSITION;
					float4 color	: COLOR;
					float2 texcoord	: TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex	: SV_POSITION;
					float4 color	: COLOR;
					float2 texcoord	: TEXCOORD0;
				};

				sampler2D _MainTex;
				float4 _FlashColor;
				float _FlashAmount;

				v2f vert(appdata_t input)
				{
					v2f result;

					result.vertex = UnityObjectToClipPos(input.vertex);
					result.texcoord = input.texcoord;
					result.color = input.color; // Image.color tint (includes alpha)

					return result;
				}

				float4 frag(v2f input) : SV_Target
				{
					float4 texColor = tex2D(_MainTex,input.texcoord)*input.color;

					// Flash RGB only. Alpha defines silhouette; changing it would shift edges during flash.
					// _FlashColor.a is intentionally ignored — strength is controlled by _FlashAmount.
					texColor.rgb = lerp(texColor.rgb,_FlashColor.rgb,_FlashAmount);

					return texColor;
				}			
			ENDCG
		}
	}
}