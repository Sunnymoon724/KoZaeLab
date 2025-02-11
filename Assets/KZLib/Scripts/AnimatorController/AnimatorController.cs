// using UnityEngine;
// using KZLib.KZAttribute;
// using KZLib.KZData;
// using Sirenix.OdinInspector;
// using UnityEditor;

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

// #if UNITY_EDITOR
// 		[SerializeField]
// 		private string m_stateName = null;
// 		private bool m_isPlaying = false;
// 		private double m_lastTime = 0.0d;
// #endif

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

// 		public bool IsExistAnimationClip(string stateName)
// 		{
// 			var clipsArray = m_animator.runtimeAnimatorController.animationClips;

// 			foreach(var clip in clipsArray)
// 			{
// 				if(clip.name.Contains(stateName))
// 				{
// 					return true;
// 				}
// 			}

// 			return false;
// 		}

// 		private void OnPlayEvent(string text)
// 		{
// 			LogTag.Build.W(text);
// 		}

// #if UNITY_EDITOR
// 		[Button("Play Animation")]
// 		protected void OnPlayAnimationInEditor()
// 		{
// 			if(!m_animator)
// 			{
// 				LogTag.System.W("Animator is null");

// 				return;
// 			}

// 			EditorApplication.update -= UpdateInEditor;
// 			EditorApplication.update += UpdateInEditor;

// 			m_animator.Play(m_stateName);

// 			m_lastTime = EditorApplication.timeSinceStartup;
// 			m_isPlaying = true;
// 		}

// 		[Button("Stop Animation")]
// 		protected void OnStopAnimationInEditor()
// 		{
// 			m_isPlaying = false;
// 			EditorApplication.update -= UpdateInEditor;
// 		}

// 		void UpdateInEditor()
// 		{
// 			if(!m_isPlaying || !m_animator)
// 			{
// 				return;
// 			}

// 			var currentTime = EditorApplication.timeSinceStartup;

// 			m_lastTime = currentTime;

// 			m_animator.Update((float)(currentTime-m_lastTime));
// 			SceneView.RepaintAll();
// 		}
// #endif
// 	}
// }