Shader "KZLib/Cam/MotionBlurEffect"
{
	Properties
	{
		[HideInInspector] _MainTex("Base",2D) = "white" {}
	}

	CGINCLUDE
	
#include "UnityCG.cginc"
#if defined(TEN) 
#define MULTIPLAYER 0.1h
#elif defined(EIGHT)
#define MULTIPLAYER 0.125h
#else
#define MULTIPLAYER 0.16666667h
#endif

	sampler2D _MainTex;
	sampler2D _BlurTex;

	fixed _Distance;
	fixed4x4 _ViewProjectionMatrix;
	
	fixed4 _MainTex_ST;
	fixed4 _BlurTex_ST;
	
	struct appdata_t
	{
		fixed4 pos : POSITION;
		fixed2 uv : TEXCOORD0;
	};

	struct v2f
	{
		fixed4 pos : POSITION;
		fixed2 uv : TEXCOORD0;
	};

	struct v2fb
	{
		fixed4 pos : POSITION;
		fixed4 uv : TEXCOORD0;
		fixed4 uv1 : TEXCOORD1;
		fixed4 uv2 : TEXCOORD2;
#if defined(EIGHT)
		fixed4 uv3 : TEXCOORD3;
#endif
#if defined(TEN)
		fixed4 uv4 : TEXCOORD4;
#endif
	};

	v2f vert(appdata_t _data)
	{
		v2f result;
		result.pos = UnityObjectToClipPos(_data.pos);
		result.uv = _data.uv;

		return result;
	}

	v2fb vertb(appdata_t _data)
	{
		v2fb result;
		result.pos = UnityObjectToClipPos(_data.pos);

		fixed4 position = fixed4(_data.uv*2.0h-1.0h,_Distance,1.0h);
		fixed4 previous = mul(_ViewProjectionMatrix,position);
		previous /= previous.w;

		fixed2 velocity = (previous.xy-position.xy)*MULTIPLAYER*0.5h;

		result.uv.xy = _data.uv;
		result.uv.zw = velocity;
		result.uv1.xy = velocity*2.0h;
		result.uv1.zw = velocity*3.0h;
		result.uv2.xy = velocity*4.0h;
		result.uv2.zw = velocity*5.0h;
#if defined(EIGHT)
		result.uv3.xy = velocity*6.0h;
		result.uv3.zw = velocity*7.0h;
#endif
#if defined(TEN)
		result.uv4.xy = velocity*8.0h;
		result.uv4.zw = velocity*9.0h;
#endif
		return result;
	}

	fixed4 fragb(v2fb _data) : SV_Target
	{
		fixed4 col = tex2D(_MainTex,_data.uv);
		fixed col1A = tex2D(_MainTex,_data.uv+_data.uv.zw).a;
		fixed col2A = tex2D(_MainTex,_data.uv+_data.uv1.xy).a;
		fixed col3A = tex2D(_MainTex,_data.uv+_data.uv1.zw).a;
		fixed col4A = tex2D(_MainTex,_data.uv+_data.uv2.xy).a;
		fixed col5A = tex2D(_MainTex,_data.uv+_data.uv2.zw).a;

		col += tex2D(_MainTex,_data.uv+_data.uv.zw*col1A);
		col += tex2D(_MainTex,_data.uv+_data.uv1.xy*col2A);
		col += tex2D(_MainTex,_data.uv+_data.uv1.zw*col3A);
		col += tex2D(_MainTex,_data.uv+_data.uv2.xy*col4A);
		col += tex2D(_MainTex,_data.uv+_data.uv2.zw*col5A);

#if defined(EIGHT)
		fixed col6A = tex2D(_MainTex,_data.uv+_data.uv3.xy).a;
		fixed col7A = tex2D(_MainTex,_data.uv+_data.uv3.zw).a;

		col += tex2D(_MainTex,_data.uv+_data.uv3.zw*col6A);
		col += tex2D(_MainTex,_data.uv+_data.uv3.xy*col7A);
#endif
#if defined(TEN)
		fixed col8A = tex2D(_MainTex,_data.uv+_data.uv4.xy).a;
		fixed col9A = tex2D(_MainTex,_data.uv+_data.uv4.zw).a;

		col += tex2D(_MainTex,_data.uv+_data.uv4.zw*col8A);
		col += tex2D(_MainTex,_data.uv+_data.uv4.xy*col9A);
#endif
		return col*MULTIPLAYER;
	}
	fixed4 frag(v2f _data) : SV_Target
	{
		fixed4 blur = tex2D(_BlurTex,_data.uv);

		return lerp(tex2D(_MainTex,_data.uv),blur,blur.a);
	}

	ENDCG

	SubShader
	{
		ZTest Always Cull Off ZWrite Off
		Fog{ Mode off }

		Pass
		{
			CGPROGRAM
			#pragma shader_feature EIGHT
			#pragma shader_feature TEN
			#pragma vertex vertb
			#pragma fragment fragb
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG
		}
		
		ZTest Always Cull Off ZWrite Off
		Fog{ Mode off }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG
		}
	}
}