using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class CommonUtility
{
	private static readonly Dictionary<string,Shader> s_ShaderDict = new();

	private static Material s_Grayscale = null;
	private static Material s_Blur = null;
	private static Material s_GrayscaleBlur = null;

	public static Material Grayscale
	{
		get
		{
			if(!s_Grayscale)
			{
				s_Grayscale = GetMaterial("KZLib/TextureGrayscaleBlur");
				s_Grayscale.SetFloat("_BlurSize",0.0f);
				s_Grayscale.SetFloat("_Saturation",1.0f);
			}

			return s_Grayscale;
		}
	}

	public static Material Blur
	{
		get
		{
			if(!s_Blur)
			{
				s_Blur = GetMaterial("KZLib/TextureGrayscaleBlur");
				s_Blur.SetFloat("_BlurSize",0.8f);
				s_Blur.SetFloat("_Saturation",0.0f);
			}

			return s_Blur;
		}
	}

	public static Material GrayscaleBlur
	{
		get
		{
			if(!s_GrayscaleBlur)
			{
				s_GrayscaleBlur = GetMaterial("KZLib/TextureGrayscaleBlur");
				s_GrayscaleBlur.SetFloat("_BlurSize",0.8f);
				s_GrayscaleBlur.SetFloat("_Saturation",1.0f);
			}

			return s_GrayscaleBlur;
		}
	}

	public static Material GetMaterial(string _shaderName)
	{
		return new Material(GetShader(_shaderName));
	}

	public static Material GetTextureGrayscaleBlurMaterial()
	{
		return GetMaterial("KZLib/TextureGrayscaleBlur");
	}

	private static Shader GetShader(string _name)
	{
		if(s_ShaderDict.TryGetValue(_name,out var shader))
		{
			return shader;
		}

		shader = Shader.Find(_name);

		if(!shader)
		{
			throw new NullReferenceException(string.Format("쉐이더 {0}이 없습니다.",_name));
		}

		s_ShaderDict.Add(_name,shader);

		return shader;
	}
}