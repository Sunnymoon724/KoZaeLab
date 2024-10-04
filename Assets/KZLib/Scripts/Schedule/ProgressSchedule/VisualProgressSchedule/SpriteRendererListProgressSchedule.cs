using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZSchedule
{
	public class SpriteRendererListProgressSchedule : VisualProgressSchedule
	{
		[VerticalGroup("List",Order = -1),SerializeField,LabelText("Renderer List"),ListDrawerSettings(DraggableItems = false),OnValueChanged(nameof(OnChangeGraphic))]
		private List<SpriteRenderer> m_SpriteRendererList = new();

		private void OnChangeGraphic()
		{
			m_MaterialArray = null;
		}

		private Material[] m_MaterialArray = null;

		private Material[] MaterialArray
		{
			get
			{
				if(m_MaterialArray == null)
				{
					m_MaterialArray = new Material[m_SpriteRendererList.Count];

					for(var i=0;i<m_SpriteRendererList.Count;i++)
					{
						if(!m_SpriteRendererList[i].material)
						{
							continue;
						}

						m_MaterialArray[i] = new Material(m_SpriteRendererList[i].material);

						m_SpriteRendererList[i].material = m_MaterialArray[i];
					}
				}

				return m_MaterialArray;
			}
		}

		protected override void SetProgress(float _progress)
		{
			for(var i=0;i<m_SpriteRendererList.Count;i++)
			{
				if(IsFadeMode)
				{
					m_SpriteRendererList[i].color = GetFadeColor(m_SpriteRendererList[i].color,_progress);
				}
				else
				{
					if(HideMaterial)
					{
						m_SpriteRendererList[i].color = GetColor(m_SpriteRendererList[i].color,_progress);
					}
					else
					{
						if(!MaterialArray[i])
						{
							continue;
						}

						MaterialArray[i].SetFloat(m_MaterialName,_progress);
					}
				}
			}
		}
	}
}