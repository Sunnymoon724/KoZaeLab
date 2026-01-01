using UnityEngine;
using KZLib.KZAttribute;
using KZLib.KZData;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;


#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	[RequireComponent(typeof(Animator))]
	public class MotionController : SerializedMonoBehaviour
	{
		[SerializeField]
		protected Animator m_animator = null;

		[SerializeField,KZRichText]
		protected string m_currentStateName = null;

		private readonly Dictionary<int,MotionEvent> m_motionEventDict = new();

		public void PlayAnimation(string stateName,int layer,float speed = 1.0f,float normalizedTime = 0.0f)
		{
			if(!m_animator)
			{
				return;
			}

			SetSpeed(speed);

			m_currentStateName = stateName;

			m_animator.Play(stateName,layer,normalizedTime);
		}
		
		public void PlayAnimationInTime(string stateName,int layer,float duration)
		{
			if(!m_animator)
			{
				return;
			}

			var clipLength = FindAnimationClipLength(stateName);

			SetSpeed(clipLength/duration);

			m_currentStateName = stateName;

			m_animator.Play(stateName,layer);
		}

		public void SetSpeed(float speed = 1.0f)
		{
			if(!m_animator)
			{
				return;
			}

			m_animator.speed = speed;
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

			var motionPrt = ProtoManager.In.GetProto<MotionProto>(motionNum);

			m_motionEventDict.Clear();

			var motionEventArray = motionPrt.EventArray;

			for(var i=0;i<motionEventArray.Length;i++)
			{
				var motionEvent = motionEventArray[i];
				
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
		[BoxGroup("Animation",ShowLabel = false)]
		[VerticalGroup("Animation/0"),SerializeField,ValueDropdown(nameof(StateNameGroup))]
		protected string m_stateName = null;

		private bool m_isPlaying = false;
		private float m_lastTime = 0.0f;

		[HorizontalGroup("Animation/1"),Button(Name = "",Icon = SdfIconType.PlayFill,ButtonHeight = 30),EnableIf(nameof(IsValidState))]
		protected void _OnPlayAnimationInEditor()
		{
			if(!m_animator)
			{
				return;
			}

			EditorApplication.update -= _UpdateInEditor;
			EditorApplication.update += _UpdateInEditor;

			m_animator.Play(m_stateName);

			m_lastTime = Convert.ToSingle(EditorApplication.timeSinceStartup);
			m_isPlaying = true;
		}

		[HorizontalGroup("Animation/1"),Button(Name = "",Icon = SdfIconType.StopFill,ButtonHeight = 30),EnableIf(nameof(IsValidState))]
		protected void _OnStopAnimationInEditor()
		{
			m_isPlaying = false;

			EditorApplication.update -= _UpdateInEditor;
		}

		private void _UpdateInEditor()
		{
			if(!m_animator)
			{
				return;
			}

			if(!m_isPlaying)
			{
				return;
			}

			var currentTime = Convert.ToSingle(EditorApplication.timeSinceStartup);

			var deltaTime = currentTime-m_lastTime;

			m_lastTime = currentTime;

			m_animator.Update(deltaTime);
			SceneView.RepaintAll();
		}

		private readonly List<string> m_stateNameList = new();

		private IEnumerable StateNameGroup
		{
			get
			{
				if(m_animator && m_stateNameList.IsNullOrEmpty())
				{
					m_stateNameList.AddRange(m_animator.FindStateNameGroup());
				}

				return m_stateNameList;
			}
		}

		private bool IsValidState => !m_stateName.IsEmpty();
#endif
	}
}