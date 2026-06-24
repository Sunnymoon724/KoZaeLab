using System;
using System.Collections.Generic;
using UnityEngine;
using KZLib.Utilities;

namespace KZLib
{
	/// <summary>
	/// Caches <see cref="Shader"/> lookups and creates <see cref="Material"/> instances on demand.
	/// </summary>
	public class ShaderManager : Singleton<ShaderManager>
	{
		/// <summary>Screen effect presets backed by <see cref="TextureGrayscaleBlur"/>.</summary>
		public enum TextureEffectType
		{
			Grayscale,
			Blur,
			GrayscaleBlur,
		}

		private const string c_textureGrayscaleBlurShader = "KZLib/TextureGrayscaleBlur";
		private const float c_defaultBlurSize = 0.8f;
		private const float c_enableSize = 1.0f;
		private const float c_disableSize = 0.0f;

		private static readonly int s_blurSizeId = Shader.PropertyToID("_BlurSize");
		private static readonly int s_saturationId = Shader.PropertyToID("_Saturation");

		//? By design: cache key is the exact shaderName string passed by the caller
		private readonly Dictionary<string,Shader> m_shaderDict = new();

		private ShaderManager() { }

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_shaderDict.Clear();
			}

			base._Release(disposing);
		}

		public Shader FindShader(string shaderName)
		{
			if(shaderName.IsEmpty())
			{
				throw new ArgumentNullException(nameof(shaderName),"ShaderName must be assigned.");
			}

			if(m_shaderDict.TryGetValue(shaderName,out var cachedShader))
			{
				return cachedShader;
			}

			var shader = Shader.Find(shaderName);

			if(shader == null)
			{
				var name = shaderName[(shaderName.LastIndexOf('/') + 1)..];

				//? Fallback when Shader.Find fails (e.g. stripped shaders). Resources path: Assets/**/Resources/Shader/{name}
				shader = Resources.Load<Shader>($"Shader/{name}");

				if(shader == null)
				{
					throw new InvalidOperationException($"Shader '{shaderName}' was not found.");
				}
			}

			m_shaderDict.Add(shaderName,shader);

			return shader;
		}

		/// <summary>
		/// Creates and returns a new <see cref="Material"/> instance from the specified shader.
		/// The caller is responsible for destroying the returned material via <see cref="ObjectExtension.DestroyObject"/> when it is no longer needed.
		/// </summary>
		public Material GetMaterial(string shaderName)
		{
			return new Material(FindShader(shaderName));
		}

		/// <summary>
		/// Creates and returns a new configured <see cref="Material"/> for <see cref="TextureEffectType"/>.
		/// The caller owns the instance and must destroy it when it is no longer needed.
		/// </summary>
		public Material GetTextureEffectMaterial(TextureEffectType effectType)
		{
			//? By design: one shader, three presets via _BlurSize and _Saturation
			var material = GetMaterial(c_textureGrayscaleBlurShader);

			switch(effectType)
			{
				case TextureEffectType.Grayscale:
					material.SetFloat(s_blurSizeId,c_disableSize);
					material.SetFloat(s_saturationId,c_enableSize);

					break;

				case TextureEffectType.Blur:
					material.SetFloat(s_blurSizeId,c_defaultBlurSize);
					material.SetFloat(s_saturationId,c_disableSize);

					break;

				case TextureEffectType.GrayscaleBlur:
					material.SetFloat(s_blurSizeId,c_defaultBlurSize);
					material.SetFloat(s_saturationId,c_enableSize);

					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(effectType),effectType,null);
			}

			return material;
		}
	}
}