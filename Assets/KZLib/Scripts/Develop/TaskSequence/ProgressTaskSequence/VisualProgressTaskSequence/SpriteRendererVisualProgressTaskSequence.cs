using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZDevelop
{
	public class SpriteRendererVisualProgressTaskSequence : VisualProgressTaskSequence
	{
		[SerializeField,ListDrawerSettings(DraggableItems = false),OnValueChanged(nameof(_OnChangedSpriteRenderer))]
		private List<SpriteRenderer> m_spriteRendererList = new();

		private void _OnChangedSpriteRenderer()
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
					m_materialArray = new Material[m_spriteRendererList.Count];

					for(var i=0;i<m_spriteRendererList.Count;i++)
					{
						if(!m_spriteRendererList[i].material)
						{
							continue;
						}

						m_materialArray[i] = new Material(m_spriteRendererList[i].material);

						m_spriteRendererList[i].material = m_materialArray[i];
					}
				}

				return m_materialArray;
			}
		}

		protected override void _SetProgress(float progress)
		{
			for(var i=0;i<m_spriteRendererList.Count;i++)
			{
				if(IsFadeMode)
				{
					m_spriteRendererList[i].color = _GetFadeColor(m_spriteRendererList[i].color,progress);
				}
				else
				{
					if(HideMaterial)
					{
						m_spriteRendererList[i].color = _GetColor(m_spriteRendererList[i].color,progress);
					}
					else
					{
						if(!MaterialArray[i])
						{
							continue;
						}

						MaterialArray[i].SetFloat(m_materialName,progress);
					}
				}
			}
		}
	}
}