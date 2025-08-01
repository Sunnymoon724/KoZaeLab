using KZLib.KZDevelop;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TransitionPanel
{
	public class TransitionProgressTask : ProgressTask
	{
		private enum TransitionType { None, Material };

		[SerializeField]
		private Image m_image = null;

		[SerializeField]
		private Material m_currentMaterial = null;

		private Material CurrentMaterial
		{
			get
			{
				if(m_currentMaterial == null)
				{
					m_currentMaterial = new Material(m_image.material);
				}

				return m_currentMaterial;
			}
		}

		[SerializeField,HideInInspector]
		private TransitionType m_currentType = TransitionType.None;

		[ShowInInspector]
		private TransitionType CurrentType
		{
			get => m_currentType;
			set
			{
				m_currentType = value;

				m_image.material = IsFade ? null : CurrentMaterial;

				SetProgress(Progress);
			}
		}

		private bool IsFade => CurrentType == TransitionType.None;

		protected override void OnDisable()
		{
			base.OnDisable();

			if(CurrentMaterial)
			{
				CurrentMaterial.DestroyObject();
			}
		}

		protected override void SetProgress(float progress)
		{
			if(IsFade)
			{
				m_image.color = m_image.color.MaskAlpha(progress);
			}
			else
			{
				CurrentMaterial.SetFloat("_Range",progress);
			}
		}
	}
}