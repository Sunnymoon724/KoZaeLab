using KZLib.KZSchedule;
using UnityEngine;
using UnityEngine.UI;

namespace TransitionPanel
{
	public class TransitionSchedule : ProgressSchedule
	{
		private enum TransitionType { Fade, Wipe };

		[SerializeField]
		private Image m_Image = null;

		private Material m_WipeMaterial = null;

		private Material WipeMaterial
		{
			get
			{
				if(!m_WipeMaterial)
				{
					m_WipeMaterial = new Material(m_Image.material);

					m_Image.material = m_WipeMaterial;
				}

				return m_WipeMaterial;
			}
		}

		[SerializeField]
		private TransitionType m_TransitionType = TransitionType.Fade;

		private bool IsFade => m_TransitionType == TransitionType.Fade;

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if(m_WipeMaterial)
			{
				CommonUtility.DestroyObject(m_WipeMaterial);
			}
		}

		public void SetData(TransitionData _data)
		{
			m_Image.color = _data.TransitionColor;

			m_TransitionType = _data.IsFade ? TransitionType.Fade : TransitionType.Wipe;

			if(IsFade)
			{
				WipeMaterial.SetFloat("_Range",1.0f);
				WipeMaterial.SetFloat("_Cutout",_data.IsCutout ? 1.0f : 0.0f);
			}
			else
			{
				m_Image.color = m_Image.color.MaskAlpha(1.0f);
			}
		}

		protected override void SetProgress(float _progress)
		{
			if(IsFade)
			{
				m_Image.color = m_Image.color.MaskAlpha(Mathf.Lerp(0.0f,1.0f,_progress));
			}
			else
			{
				WipeMaterial.SetFloat("_Range",_progress);
			}
		}
	}
}