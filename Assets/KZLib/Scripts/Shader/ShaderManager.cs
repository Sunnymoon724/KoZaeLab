using System;
using System.Collections.Generic;
using UnityEngine;
using KZLib.Utilities;

namespace KZLib
{
	public class ShaderManager : Singleton<ShaderManager>
	{
		private const float c_defaultBlurSize = 0.8f;
		private const float c_enableSize = 1.0f;
		private const float c_disableSize = 0.0f;
		
		private const string c_blurText = "_BlurSize";
		private const string c_saturationText = "_Saturation";

		private readonly Dictionary<string,Shader> m_ShaderDict = new();

		private Material m_grayscale = null;
		private Material m_blur = null;
		private Material m_grayscaleBlur = null;

		public Material Grayscale
		{
			get
			{
				if(!m_grayscale)
				{
					m_grayscale = GetMaterial("KZLib/TextureGrayscaleBlur");
					m_grayscale.SetFloat(c_blurText,c_disableSize);
					m_grayscale.SetFloat(c_saturationText,c_enableSize);
				}

				return m_grayscale;
			}
		}

		public Material Blur
		{
			get
			{
				if(!m_blur)
				{
					m_blur = GetMaterial("KZLib/TextureGrayscaleBlur");
					m_blur.SetFloat(c_blurText,c_defaultBlurSize);
					m_blur.SetFloat(c_saturationText,c_disableSize);
				}

				return m_blur;
			}
		}

		public Material GrayscaleBlur
		{
			get
			{
				if(!m_grayscaleBlur)
				{
					m_grayscaleBlur = GetMaterial("KZLib/TextureGrayscaleBlur");
					m_grayscaleBlur.SetFloat(c_blurText,c_defaultBlurSize);
					m_grayscaleBlur.SetFloat(c_saturationText,c_enableSize);
				}

				return m_grayscaleBlur;
			}
		}

		private ShaderManager() { }

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_ShaderDict.Clear();

				if(m_grayscale)
				{
					m_grayscale.DestroyObject();
				}

				if(m_blur)
				{
					m_blur.DestroyObject();
				}

				if(m_grayscaleBlur)
				{
					m_grayscaleBlur.DestroyObject();
				}
			}

			base._Release(disposing);
		}

		public Shader FindShader(string shaderName)
		{
			if(!m_ShaderDict.ContainsKey(shaderName))
			{
				var shader = Shader.Find(shaderName);

				if(shader == null)
				{
					var name = shaderName[(shaderName.LastIndexOf('/') + 1)..];

					shader = Resources.Load($"Shaders/{name}") as Shader;

					if(shader == null)
					{
						throw new NullReferenceException($"Shader {shaderName} not found.");
					}
				}

				m_ShaderDict.Add(shaderName,shader);
			}

			return m_ShaderDict[shaderName];
		}

		public Material GetMaterial(string shaderName)
		{
			return new Material(FindShader(shaderName));
		}
	}
}