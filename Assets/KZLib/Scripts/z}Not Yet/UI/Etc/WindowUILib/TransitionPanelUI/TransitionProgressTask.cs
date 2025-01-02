using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TransitionPanel
{
	public class TransitionProgressTask : ProgressTask
	{
		public record TransitionProgressParam(TransitionData Data,bool IsReverse = false) : ProgressParam(IsReverse);

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

		protected override void SetParamData(ProgressParam progressParam)
		{
			base.SetParamData(progressParam);

			if(progressParam is not TransitionProgressParam param)
			{
				return;
			}

			Duration = param.Data.Duration;

			m_image.color = param.Data.TransitionColor;

			CurrentType = param.Data.IsFade ? TransitionType.None : TransitionType.Material;

			if(!IsFade)
			{
				m_image.color = m_image.color.MaskAlpha(1.0f);
			}
			else
			{
				CurrentMaterial.SetTexture("_MainTex",param.Data.TransitionTexture);
				CurrentMaterial.SetFloat("_Range",1.0f);
				CurrentMaterial.SetFloat("_Cutout",param.Data.IsCutout ? 1.0f : 0.0f);
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