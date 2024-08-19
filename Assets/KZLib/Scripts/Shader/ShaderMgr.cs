using System;
using System.Collections.Generic;
using UnityEngine;

namespace KZLib
{
	public class ShaderMgr : Singleton<ShaderMgr>
	{
		private bool m_Disposed = false;

		private readonly Dictionary<string,Shader> m_ShaderDict = new();

		private Material m_Grayscale = null;
		private Material m_Blur = null;
		private Material m_GrayscaleBlur = null;

		public Material Grayscale
		{
			get
			{
				if(!m_Grayscale)
				{
					m_Grayscale = GetMaterial("KZLib/TextureGrayscaleBlur");
					m_Grayscale.SetFloat("_BlurSize",0.0f);
					m_Grayscale.SetFloat("_Saturation",1.0f);
				}

				return m_Grayscale;
			}
		}

		public Material Blur
		{
			get
			{
				if(!m_Blur)
				{
					m_Blur = GetMaterial("KZLib/TextureGrayscaleBlur");
					m_Blur.SetFloat("_BlurSize",0.8f);
					m_Blur.SetFloat("_Saturation",0.0f);
				}

				return m_Blur;
			}
		}

		public Material GrayscaleBlur
		{
			get
			{
				if(!m_GrayscaleBlur)
				{
					m_GrayscaleBlur = GetMaterial("KZLib/TextureGrayscaleBlur");
					m_GrayscaleBlur.SetFloat("_BlurSize",0.8f);
					m_GrayscaleBlur.SetFloat("_Saturation",1.0f);
				}

				return m_GrayscaleBlur;
			}
		}

		protected override void Initialize()
		{
			
		}

		protected override void Release(bool _disposing)
		{
			if(m_Disposed)
			{
				return;
			}

			if(_disposing)
			{
				m_ShaderDict.Clear();

				if(m_Grayscale)
				{
					UnityUtility.DestroyObject(m_Grayscale);
				}

				if(m_Blur)
				{
					UnityUtility.DestroyObject(m_Blur);
				}

				if(m_GrayscaleBlur)
				{
					UnityUtility.DestroyObject(m_GrayscaleBlur);
				}
			}

			m_Disposed = true;

			base.Release(_disposing);
		}

		public Shader GetShader(string _shaderName)
		{
			if(!m_ShaderDict.ContainsKey(_shaderName))
			{
				var shader = Shader.Find(_shaderName);

				if(shader == null)
				{
					var name = _shaderName[(_shaderName.LastIndexOf('/') + 1)..];

					shader = Resources.Load(string.Format("Shaders/{0}",name)) as Shader;

					if(shader == null)
					{
						throw new NullReferenceException(string.Format("쉐이더 {0}이 없습니다.",_shaderName));
					}
				}

				m_ShaderDict.Add(_shaderName,shader);
			}

			return m_ShaderDict[_shaderName];
		}

		public Material GetMaterial(string _shaderName)
		{
			return new Material(GetShader(_shaderName));
		}
	}
}