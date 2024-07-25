using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.KZSchedule
{
	public class GraphicProgressSchedule : VisualProgressSchedule
	{
		[SerializeField,LabelText("그래픽 리스트"),ListDrawerSettings(DraggableItems = false),OnValueChanged(nameof(OnChangeGraphic))]
		private List<Graphic> m_GraphicList = new();

		[SerializeField,LabelText("프로그레스 사용")]
		private bool m_UseFillAmount = false;

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
					m_MaterialArray = new Material[m_GraphicList.Count];

					for(var i=0;i<m_GraphicList.Count;i++)
					{
						if(!m_GraphicList[i].material)
						{
							continue;
						}

						m_MaterialArray[i] = new Material(m_GraphicList[i].material);

						m_GraphicList[i].material = m_MaterialArray[i];
					}
				}

				return m_MaterialArray;
			}
		}

		protected override void SetProgress(float _progress)
		{
			for(var i=0;i<m_GraphicList.Count;i++)
			{
				if(IsFadeMode)
				{
					m_GraphicList[i].color = GetFadeColor(m_GraphicList[i].color,_progress);
				}
				else
				{
					if(HideMaterial)
					{
						m_GraphicList[i].color = GetColor(m_GraphicList[i].color,_progress);
					}
					else
					{
						if(!MaterialArray[i])
						{
							continue;
						}

						MaterialArray[i].SetFloat(m_MaterialName,_progress);
					}

					if(m_UseFillAmount)
					{
						var image = m_GraphicList[i] as Image;

						if(image == null)
						{
							continue;
						}

						image.fillAmount = _progress;
					}
				}
			}
		}
	}
}