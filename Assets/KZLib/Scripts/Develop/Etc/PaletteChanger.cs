using UnityEngine;
using KZLib.KZData;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System;

namespace KZLib.KZDevelop
{
    public class PaletteChanger : MonoBehaviour
	{
#if UNITY_EDITOR
		[SerializeField,HideInInspector]
		private int m_paletteNum = 0;

		[ShowInInspector,ValueDropdown(nameof(PaletteNameList))]
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

				ProtoMgr.In.LoadInEditor();

				SetPalette(ProtoMgr.In.GetProto<ColorProto>(m_paletteNum));
			}
		}
#endif

		[SerializeField,ReadOnly]
		private List<Renderer> m_rendererList = new();

		private MaterialPropertyBlock m_propertyBlock = null;
		private MaterialPropertyBlock PropertyBlock => m_propertyBlock ??= new MaterialPropertyBlock();

		public void SetPalette(ColorProto colorPrt)
		{
			if(m_rendererList.Count < 1 || colorPrt == null)
			{
				return;
			}

			var vectorArray = _HexCodeToVectorArray(colorPrt.ColorArray);

			PropertyBlock.SetVectorArray("_PixelColorArray",vectorArray);

			for(var i=0;i<m_rendererList.Count;i++)
			{
				var renderer = m_rendererList[i];
				
				if(!renderer)
				{
					throw new NullReferenceException($"Renderer is not exist in {gameObject.name}");
				}

				renderer.SetPropertyBlock(PropertyBlock);
			}
		}

		private Vector4[] _HexCodeToVectorArray(string[] hexCodeArray)
		{
			var colorArray = new Vector4[hexCodeArray.Length];

			colorArray[0] = hexCodeArray[0].IsEmpty() ? Color.clear : hexCodeArray[0].ToColor();

			for(var i=1;i<hexCodeArray.Length;i++)
			{
				colorArray[i] = hexCodeArray[i].IsEmpty() ? colorArray[0] : hexCodeArray[i].ToColor();
			}

			return colorArray;
		}

		[Button("Fill Renderer List")]
		private void OnFillRendererList()
		{
			m_rendererList.Clear();

			m_rendererList.AddRange(GetComponentsInChildren<Renderer>(true));
		}

		public bool CheckRenderer()
		{
			var rendererList = new List<Renderer>(m_rendererList);

			OnFillRendererList();

			return rendererList.IsEquals(m_rendererList);
		}

#if UNITY_EDITOR
		private static readonly ValueDropdownList<int> m_paletteNameList = new();

		private static IEnumerable PaletteNameList
		{
			get
			{
				if(m_paletteNameList.IsNullOrEmpty())
				{
					ProtoMgr.In.Reload();

					foreach(var prt in ProtoMgr.In.GetProtoGroup<ColorProto>())
					{
						m_paletteNameList.Add(LingoMgr.In.FindString(prt.NameKey),prt.Num);
					}
				}

				return m_paletteNameList;
			}
		}
#endif
	}
}