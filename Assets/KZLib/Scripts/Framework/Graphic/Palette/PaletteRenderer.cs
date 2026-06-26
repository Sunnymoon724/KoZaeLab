using UnityEngine;
using KZLib.Data;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;

namespace KZLib.Utilities
{
	/// <summary>
	/// Collects child <see cref="Renderer"/> instances and applies a palette via <see cref="KZPaletteKit"/>.
	/// </summary>
	public class PaletteRenderer : MonoBehaviour
	{
		[SerializeField,ReadOnly]
		private List<Renderer> m_rendererList = new();

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
		/// Throws when renderers, colors, material count, or shader are invalid.
		/// </summary>
		public void SetPalette(Vector4[] colorArray)
		{
			KZPaletteKit.SetPalette(m_rendererList,colorArray);
		}

		/// <summary>Collects all child <see cref="Renderer"/> components (including inactive) into <see cref="m_rendererList"/>.</summary>
		[Button("Fill Renderers",ButtonSizes.Large)]
		private void _OnFillRendererList()
		{
			m_rendererList.Clear();

			m_rendererList.AddRange(GetComponentsInChildren<Renderer>(true));
		}

		/// <summary>Refreshes the renderer list and returns whether it changed compared to before.</summary>
		public bool RefreshRenderer()
		{
			var rendererList = new List<Renderer>(m_rendererList);

			_OnFillRendererList();

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