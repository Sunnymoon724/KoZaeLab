using System;
using System.Collections.Generic;
using UnityEngine;
using KZLib.KZUtility;

namespace KZLib
{
	public class ShaderManager : Singleton<ShaderManager>
	{
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
					m_grayscale.SetFloat("_BlurSize",0.0f);
					m_grayscale.SetFloat("_Saturation",1.0f);
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
					m_blur.SetFloat("_BlurSize",0.8f);
					m_blur.SetFloat("_Saturation",0.0f);
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
					m_grayscaleBlur.SetFloat("_BlurSize",0.8f);
					m_grayscaleBlur.SetFloat("_Saturation",1.0f);
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