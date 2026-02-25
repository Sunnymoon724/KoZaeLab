using UnityEngine;
using KZLib.Data;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System;

namespace KZLib.Development
{
	public class PaletteChanger : MonoBehaviour
	{
		[SerializeField,ReadOnly]
		private List<Renderer> m_rendererList = new();

		private MaterialPropertyBlock m_propertyBlock = null;
		private MaterialPropertyBlock PropertyBlock => m_propertyBlock ??= new MaterialPropertyBlock();

		public void SetPalette(Vector4[] colorArray)
		{
			if(m_rendererList.Count < 1 || colorArray.IsNullOrEmpty())
			{
				return;
			}

			PropertyBlock.SetVectorArray("_PixelColorArray",colorArray);

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

		[Button("Fill Renderers",ButtonSizes.Large)]
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
		[SerializeField,HideInInspector]
		private int m_paletteNum = 0;

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

		private IEnumerable PaletteNameGroup
		{
			get
			{
				if(m_paletteNameList.IsNullOrEmpty())
				{
					ProtoManager.In.LoadInEditor();

					foreach(var prt in ProtoManager.In.FindProtoGroup<IColorProto>())
					{
						m_paletteNameList.Add($"{prt.Num}",prt.Num);
					}
				}

				return m_paletteNameList;
			}
		}
#endif
	}
}