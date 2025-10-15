using UnityEngine;
using KZLib.KZAttribute;
using KZLib.KZData;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Threading;



#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	[RequireComponent(typeof(Animator))]
	public class MotionCon : SerializedMonoBehaviour
	{
		[SerializeField]
		protected Animator m_animator = null;

		[SerializeField,KZRichText]
		protected string m_currentStateName = null;

#if UNITY_EDITOR
		[SerializeField]
		private string m_stateName = null;
		private bool m_isPlaying = false;
		private double m_lastTime = 0.0d;
#endif

		private readonly Dictionary<int,MotionEvent> m_motionEventDict = new();

		public void PlayAnimation(string stateName,int layer,float normalizedTime = 0.0f)
		{
			if(!m_animator)
			{
				return;
			}

			m_currentStateName = stateName;

			m_animator.Play(stateName,layer,normalizedTime);
		}

		public bool IsExistAnimationClip(string stateName)
		{
			return m_animator.IsExistAnimationClip(stateName);
		}

		public float FindAnimationClipLength(string stateName)
		{
			return m_animator.FindAnimationClipLength(stateName);
		}

		public void PlayMotion(int motionNum,int layer)
		{
			if(!m_animator)
			{
				return;
			}

			var motionPrt = ProtoMgr.In.GetProto<MotionProto>(motionNum);

			m_motionEventDict.Clear();

			foreach(var motionEvent in motionPrt.EventArray)
			{
				m_motionEventDict.Add(motionEvent.Order,motionEvent);
			}

			PlayAnimation(motionPrt.StateName,layer,0.0f);
		}
		
		public async UniTask WaitForAnimationFinishAsync(string stateName,int layer,CancellationToken cancellationToken)
		{
			await m_animator.WaitForAnimationFinishAsync(stateName,layer,cancellationToken);
		}

		protected void OnPlayEffect(int order)
		{
			if(m_motionEventDict.TryGetValue(order,out var motionEvent))
			{
				PlayMotionEvent(motionEvent);
			}
		}

		protected virtual void PlayMotionEvent(MotionEvent motionEvent) { }

		protected void Reset()
		{
			if(!m_animator)
			{
				m_animator = GetComponent<Animator>();
			}
		}

#if UNITY_EDITOR
		[Button("Play Animation")]
		protected void OnPlayAnimationInEditor()
		{
			if(!m_animator)
			{
				LogSvc.System.W("Animator is null");

				return;
			}

			EditorApplication.update -= UpdateInEditor;
			EditorApplication.update += UpdateInEditor;

			m_animator.Play(m_stateName);

			m_lastTime = EditorApplication.timeSinceStartup;
			m_isPlaying = true;
		}

		[Button("Stop Animation")]
		protected void OnStopAnimationInEditor()
		{
			m_isPlaying = false;
			EditorApplication.update -= UpdateInEditor;
		}

		void UpdateInEditor()
		{
			if(!m_isPlaying || !m_animator)
			{
				return;
			}

			var currentTime = EditorApplication.timeSinceStartup;

			m_lastTime = currentTime;

			m_animator.Update((float)(currentTime-m_lastTime));
			SceneView.RepaintAll();
		}
#endif
	}
}