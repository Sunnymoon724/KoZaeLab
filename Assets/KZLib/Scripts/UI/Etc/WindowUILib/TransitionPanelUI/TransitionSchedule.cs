using KZLib.KZSchedule;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TransitionPanel
{
	public class TransitionSchedule : ProgressSchedule
	{
		private enum TransitionType { None, Material };

		[SerializeField]
		private Image m_Image = null;

		[SerializeField]
		private Material m_Material = null;

		private Material TransitionMaterial
		{
			get
			{
				if(m_Material == null)
				{
					m_Material = new Material(m_Image.material);
				}

				return m_Material;
			}
		}

		[SerializeField,HideInInspector]
		private TransitionType m_Type = TransitionType.None;

		[ShowInInspector]
		private TransitionType Type
		{
			get => m_Type;
			set
			{
				m_Type = value;

				m_Image.material = IsFade ? null : m_Material;

				SetProgress(Progress);
			}
		}

		private bool IsFade => Type == TransitionType.None;

		protected override void Release()
		{
			base.Release();

			if(TransitionMaterial)
			{
				UnityUtility.DestroyObject(TransitionMaterial);
			}
		}

		public void SetTransitionData(TransitionData _data)
		{
			m_Image.color = _data.TransitionColor;

			Type = _data.IsFade ? TransitionType.None : TransitionType.Material;

			if(!IsFade)
			{
				m_Image.color = m_Image.color.MaskAlpha(1.0f);
			}
			else
			{
				TransitionMaterial.SetTexture("_MainTex",_data.TransitionTexture);
				TransitionMaterial.SetFloat("_Range",1.0f);
				TransitionMaterial.SetFloat("_Cutout",_data.IsCutout ? 1.0f : 0.0f);
			}
		}

		protected override void SetProgress(float _progress)
		{
			if(IsFade)
			{
				m_Image.color = m_Image.color.MaskAlpha(_progress);
			}
			else
			{
				TransitionMaterial.SetFloat("_Range",_progress);
			}
		}
	}
}