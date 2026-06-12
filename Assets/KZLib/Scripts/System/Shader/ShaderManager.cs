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

		private static readonly int s_blurSizeId = Shader.PropertyToID("_BlurSize");
		private static readonly int s_saturationId = Shader.PropertyToID("_Saturation");

		private readonly Dictionary<string,Shader> m_shaderDict = new();

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
					m_grayscale.SetFloat(s_blurSizeId,c_disableSize);
					m_grayscale.SetFloat(s_saturationId,c_enableSize);
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
					m_blur.SetFloat(s_blurSizeId,c_defaultBlurSize);
					m_blur.SetFloat(s_saturationId,c_disableSize);
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
					m_grayscaleBlur.SetFloat(s_blurSizeId,c_defaultBlurSize);
					m_grayscaleBlur.SetFloat(s_saturationId,c_enableSize);
				}

				return m_grayscaleBlur;
			}
		}

		private ShaderManager() { }

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_shaderDict.Clear();

				if(m_grayscale)
				{
					m_grayscale.DestroyObject();
					m_grayscale = null;
				}

				if(m_blur)
				{
					m_blur.DestroyObject();
					m_blur = null;
				}

				if(m_grayscaleBlur)
				{
					m_grayscaleBlur.DestroyObject();
					m_grayscaleBlur = null;
				}
			}

			base._Release(disposing);
		}

		public Shader FindShader(string shaderName)
		{
			if(!m_shaderDict.ContainsKey(shaderName))
			{
				var shader = Shader.Find(shaderName);

				if(shader == null)
				{
					var name = shaderName[(shaderName.LastIndexOf('/') + 1)..];

					shader = Resources.Load($"Shaders/{name}") as Shader;

					if(shader == null)
					{
						throw new NullReferenceException($"Shader {shaderName} not found. ShaderName must be assigned.");
					}
				}

				m_shaderDict.Add(shaderName,shader);
			}

			return m_shaderDict[shaderName];
		}

		/// <summary>
		/// Creates and returns a new Material instance from the specified shader.
		/// The caller is responsible for destroying the returned Material via DestroyObject() when it is no longer needed.
		/// </summary>
		public Material GetMaterial(string shaderName)
		{
			return new Material(FindShader(shaderName));
		}
	}
}