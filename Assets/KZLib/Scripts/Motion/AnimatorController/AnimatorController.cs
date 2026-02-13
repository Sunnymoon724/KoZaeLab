// using UnityEngine;
// using KZLib.Attributes;


// namespace KZLib
// {
// 	// TODO 이벤트 부분 개발 중

// 	/// <summary>
// 	/// 매 프레임 체크하며 애니메이션 실행함 (애니메이션 확인도 추가)
// 	/// </summary>
// 	[RequireComponent(typeof(Animator)),ExecuteInEditMode]
// 	public class AnimatorController : BaseComponent
// 	{
// 		[SerializeField]
// 		protected Animator m_animator = null;

// 		[SerializeField,KZRichText]
// 		private string m_currentStateName = null;



// 		protected override void Reset()
// 		{
// 			base.Reset();

// 			if(!m_animator)
// 			{
// 				m_animator = GetComponent<Animator>();
// 			}
// 		}

// 		public void PlayAnimation(string stateName,float normalizedTime = 0.0f)
// 		{
// 			if(!m_animator)
// 			{
// 				return;
// 			}

// 			m_currentStateName = stateName;

// 			m_animator.Play(stateName,0,normalizedTime);
// 		}

// 		public void PlayAction(string stateName,int actionNum,float normalizedTime = 0.0f)
// 		{
// 			if(!m_animator)
// 			{
// 				return;
// 			}

// 			// create Action

// 			PlayAnimation(stateName,normalizedTime);
// 		}

		

// 		private void OnPlayEvent(string text)
// 		{
// 			LogChannel.Build.W(text);
// 		}


// 	}
// }