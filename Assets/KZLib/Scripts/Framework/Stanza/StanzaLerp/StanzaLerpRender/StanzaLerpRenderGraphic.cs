using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.Utilities
{
	/// <summary>
	/// Applies <see cref="StanzaLerpRender"/> fade/free modes to UI <see cref="Graphic"/> targets.
	/// Optionally drives <see cref="Image.fillAmount"/> and per-instance material floats.
	/// </summary>
	public class StanzaLerpRenderGraphic : StanzaLerpRender
	{
		[SerializeField,ListDrawerSettings(DraggableItems = false),OnValueChanged(nameof(_OnChangedGraphic))]
		private List<Graphic> m_graphicList = new();

		[SerializeField]
		private bool m_useFillAmount = false;

		private void _OnChangedGraphic()
		{
			_DestroyMaterialArray();

			m_materialArray = null;
		}

		private void OnDestroy()
		{
			_DestroyMaterialArray();
		}

		private void _DestroyMaterialArray()
		{
			if(m_materialArray == null)
			{
				return;
			}

			for(var i=0;i<m_materialArray.Length;i++)
			{
				if(m_materialArray[i])
				{
					m_materialArray[i].DestroyObject();
				}
			}
		}

		private Material[] m_materialArray = null;

		/// <summary>Lazy-cloned materials so each graphic can be driven independently.</summary>
		private Material[] MaterialArray
		{
			get
			{
				if(m_materialArray == null)
				{
					m_materialArray = new Material[m_graphicList.Count];

					for(var i=0;i<m_graphicList.Count;i++)
					{
						if(!m_graphicList[i].material)
						{
							continue;
						}

						m_materialArray[i] = new Material(m_graphicList[i].material);

						m_graphicList[i].material = m_materialArray[i];
					}
				}

				return m_materialArray;
			}
		}

		protected override void _SetProgress(float progress)
		{
			for(var i=0;i<m_graphicList.Count;i++)
			{
				if(IsFadeMode)
				{
					m_graphicList[i].color = _GetFadeColor(m_graphicList[i].color,progress);
				}
				else
				{
					if(HideMaterial)
					{
						m_graphicList[i].color = _GetColor(m_graphicList[i].color,progress);
					}
					else
					{
						if(!MaterialArray[i])
						{
							continue;
						}

						MaterialArray[i].SetFloat(m_materialName,progress);
					}

					if(m_useFillAmount)
					{
						var image = m_graphicList[i] as Image;

						if(image == null)
						{
							continue;
						}

						image.fillAmount = progress;
					}
				}
			}
		}
	}
}
