using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.Development
{
	public class GraphicVisualProgressTaskSequence : VisualProgressTaskSequence
	{
		[SerializeField,ListDrawerSettings(DraggableItems = false),OnValueChanged(nameof(_OnChangedGraphic))]
		private List<Graphic> m_graphicList = new();

		[SerializeField]
		private bool m_useFillAmount = false;

		private void _OnChangedGraphic()
		{
			m_materialArray = null;
		}

		private Material[] m_materialArray = null;

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