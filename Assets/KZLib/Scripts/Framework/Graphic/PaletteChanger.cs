using UnityEngine;
using KZLib.Data;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System;

namespace KZLib.Utilities
{
	/// <summary>
	/// Applies a palette (color array) to child <see cref="Renderer"/> instances via <see cref="MaterialPropertyBlock"/>.
	/// Writes the shader property <c>_PixelColorArray</c> without instantiating per-renderer materials.
	/// </summary>
	public class PaletteChanger : MonoBehaviour
	{
		/// <summary>Shader property name for the palette color array.</summary>
		private const string c_pixelColorArray = "_PixelColorArray";

		[SerializeField,ReadOnly]
		private List<Renderer> m_rendererList = new();

		private MaterialPropertyBlock m_propertyBlock = null;

		/// <summary>Shared property block reused across all target renderers.</summary>
		private MaterialPropertyBlock PropertyBlock => m_propertyBlock ??= new MaterialPropertyBlock();

		/// <summary>Resolves colors from <paramref name="colorPrt"/> and applies them.</summary>
		public void SetPalette(IColorProto colorPrt)
		{
			SetPalette(ProtoManager.In.GetColorVectorArray(colorPrt));
		}

		/// <summary>Resolves colors by proto number and applies them.</summary>
		public void SetPalette(int colorNum)
		{
			SetPalette(ProtoManager.In.GetColorVectorArray(colorNum));
		}

		/// <summary>
		/// Sets <c>_PixelColorArray</c> on every entry in <see cref="m_rendererList"/>.
		/// Throws when renderers or colors are missing.
		/// </summary>
		public void SetPalette(Vector4[] colorArray)
		{
			if(m_rendererList.Count < 1)
			{
				throw new InvalidOperationException($"Renderer list is empty in {gameObject.name}. Fill Renderers must be called.");
			}

			if(colorArray.IsNullOrEmpty())
			{
				throw new InvalidOperationException($"Color array is null or empty in {gameObject.name}. Color data must be assigned.");
			}

			PropertyBlock.SetVectorArray(c_pixelColorArray,colorArray);

			for(var i=0;i<m_rendererList.Count;i++)
			{
				var renderer = m_rendererList[i];

				if(!renderer)
				{
					throw new NullReferenceException($"Renderer does not exist in {gameObject.name}. GameObject must be assigned.");
				}

				renderer.SetPropertyBlock(PropertyBlock);
			}
		}

		/// <summary>Collects all child <see cref="Renderer"/> components (including inactive) into <see cref="m_rendererList"/>.</summary>
		[Button("Fill Renderers",ButtonSizes.Large)]
		private void OnFillRendererList()
		{
			m_rendererList.Clear();

			m_rendererList.AddRange(GetComponentsInChildren<Renderer>(true));
		}

		/// <summary>Refreshes the renderer list and returns whether it changed compared to before.</summary>
		public bool RefreshRenderer()
		{
			var rendererList = new List<Renderer>(m_rendererList);

			OnFillRendererList();

			return rendererList.AreEqual(m_rendererList);
		}

#if UNITY_EDITOR
		[SerializeField,HideInInspector]
		private int m_paletteNum = 0;

		/// <summary>Editor-only palette picker; loads proto data and previews via <see cref="SetPalette(int)"/>.</summary>
		[ShowInInspector,ValueDropdown(nameof(PaletteNameGroup))]
		protected int PaletteNum
		{
			get => m_paletteNum;
			private set
			{
				if(m_paletteNum == value)
				{
					return;
				}

				m_paletteNum = value;

				if(m_paletteNum < 1)
				{
					return;
				}

				ProtoManager.In.LoadInEditor();

				var vectorArray = ProtoManager.In.GetColorVectorArray(m_paletteNum);

				SetPalette(vectorArray);
			}
		}

		private readonly ValueDropdownList<int> m_paletteNameList = new();

		/// <summary>Color proto entries for the Odin palette dropdown.</summary>
		private IEnumerable PaletteNameGroup
		{
			get
			{
				if(m_paletteNameList.IsNullOrEmpty())
				{
					ProtoManager.In.LoadInEditor();

					foreach(var colorPrt in ProtoManager.In.FindColorGroup())
					{
						m_paletteNameList.Add($"{colorPrt.Num}",colorPrt.Num);
					}
				}

				return m_paletteNameList;
			}
		}
#endif
	}
}
