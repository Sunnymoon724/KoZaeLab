using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Utilities
{
	/// <summary>
	/// Applies <see cref="StanzaLerpRender"/> fade/free modes to <see cref="SpriteRenderer"/> targets.
	/// Clones materials per renderer when driving shader floats in free mode.
	/// </summary>
	public class StanzaLerpRenderSprite : StanzaLerpRender
	{
		[SerializeField,ListDrawerSettings(DraggableItems = false),OnValueChanged(nameof(_OnChangedSpriteRenderer))]
		private List<SpriteRenderer> m_spriteRendererList = new();

		private void _OnChangedSpriteRenderer()
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

		/// <summary>Lazy-cloned materials so each sprite renderer can be driven independently.</summary>
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
