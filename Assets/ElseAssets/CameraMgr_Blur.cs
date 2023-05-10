// using Sirenix.OdinInspector;
// using System.Collections;
// using UnityEngine;

// namespace KZLib
// {
// 	public partial class CameraMgr : SingletonMB<CameraMgr>
//     {
// 		private CC_RadialBlur m_RadialBlur = null;

// 		[SerializeField,HideInInspector]
// 		private float m_BlurMaxAmount = 0.3f;
// 		[SerializeField,HideInInspector]
// 		private float m_BlurSpeed = 1.0f;

// 		[BoxGroup("Blur")]
// 		[HorizontalGroup("Blur/옵션"),ShowInInspector,LabelText("Blur 스피드")]
// 		public float BlurSpeed { get => m_BlurSpeed; set => m_BlurSpeed = value; }
// 		[HorizontalGroup("Blur/옵션"),ShowInInspector,LabelText("Blur 최대 양")]
// 		public float BlurMaxAmount { get => m_BlurMaxAmount; set => m_BlurMaxAmount = value; }
		
// 		private IEnumerator m_CoBlur = null;

// 		private Vector2 m_BlurOrigin = Vector2.zero;

// 		private void ReleaseBlur()
// 		{
// 			if(m_CoBlur != null)
// 			{
// 				StopCoroutine(m_CoBlur);
// 			}

// 			m_CoBlur = null;
// 		}

// 		public void PlayBlur(float _amountX = 0.5f,float _amountY = 0.5f)
// 		{
// 			ReleaseBlur();

// 			StartCoroutine(m_CoBlur = CoBlurProcess(new Vector2(_amountX,_amountY)));
// 		}

// 		private IEnumerator CoBlurProcess(Vector2 _amount)
// 		{
// 			var option = GameDataMgr.In.Access<GameData.Option>();

// 			m_RadialBlur.enabled = option.IsIncludeGraphicQualityOption(GameData.Option.GraphicQualityOption.RADIAL_BLUR_ENABLE);
// 			m_RadialBlur.amount = 0.0f;

// 			m_RadialBlur.center = _amount;

// 			while(m_RadialBlur.amount <= m_BlurMaxAmount)
// 			{
// 				m_RadialBlur.amount += Time.deltaTime*m_BlurSpeed;

// 				yield return null;
// 			}

// 			yield return YieldCache.WaitForSeconds(0.3f);

// 			while(m_RadialBlur.amount > 0.0f)
// 			{
// 				m_RadialBlur.amount -= Time.deltaTime*m_BlurSpeed;

// 				yield return null;
// 			}

// 			m_RadialBlur.amount = 0.0f;
// 			m_RadialBlur.enabled = false;
// 			m_RadialBlur.center = m_BlurOrigin;
// 		}

// 		public void ResetBlur()
// 		{
// 			ReleaseBlur();

// 			m_RadialBlur.amount = 0.0f;
// 			m_RadialBlur.enabled = false;
// 			m_RadialBlur.center = m_BlurOrigin;
// 		}
// 	}
// }