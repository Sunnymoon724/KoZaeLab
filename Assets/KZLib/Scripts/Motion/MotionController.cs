using UnityEngine;
using KZLib.Data;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using UnityEngine.UI;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	[RequireComponent(typeof(Animator))]
	public class MotionController : MonoBehaviour
	{
		[SerializeField]
		private Animator m_animator = null;

		private readonly Dictionary<int,MotionEntry> m_motionEntryDict = new();

		public void SetAnimator(string animatorPath)
		{
			if(!m_animator)
			{
				return;
			}

			m_animator.runtimeAnimatorController = ResourceManager.In.GetAnimatorOverrideController(animatorPath);

			m_animator.Rebind();
		}

		public void PlayAnimation(string stateName,int layer = 0,float speed = 1.0f,float normalizedTime = 0.0f)
		{
			if(!_CheckAnimation(stateName,layer))
			{
				return;
			}

			_PlayAnimationInner(stateName,layer,speed,normalizedTime);
		}

		public void PlayAnimationInTime(string stateName,int layer,float duration,float normalizedTime)
		{
			if(!_CheckAnimation(stateName,layer))
			{
				return;
			}

			var clipLength = FindAnimationClipLength(stateName);

			_PlayAnimationInner(stateName,layer,clipLength/duration,normalizedTime);
		}

		private bool _CheckAnimation(string stateName,int layer)
		{
			if(!m_animator)
			{
				return false;
			}

			if(!m_animator.HasState(layer,Animator.StringToHash(stateName)))
			{
				LogChannel.Develop.W($"{stateName} is not exist in {m_animator.name}");

				return false;
			}

			return true;
		}

		private void _PlayAnimationInner(string stateName,int layer,float speed,float normalizedTime)
		{
			SetSpeed(speed);

			m_animator.Play(stateName,layer,normalizedTime);
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

			var motionPrt = ProtoManager.In.GetProto<IMotionProto>(motionNum);

			m_motionEntryDict.Clear();

			var motionEntryArray = motionPrt.MotionEntryArray;

			for(var i=0;i<motionEntryArray.Length;i++)
			{
				var motionEntry = motionEntryArray[i];

				m_motionEntryDict.Add(motionEntry.Order,motionEntry);
			}

			PlayAnimation(motionPrt.StateName,layer,0.0f);
		}

		public async UniTask WaitForAnimationFinishAsync(string stateName,int layer,CancellationToken cancellationToken)
		{
			if(!m_animator)
			{
				return;
			}

			await m_animator.WaitForAnimationFinishAsync(stateName,layer,cancellationToken);
		}

		protected void _OnPlayEffect(int order)
		{
			if(m_motionEntryDict.TryGetValue(order,out var motionEntry))
			{
				_PlayMotionEntry(motionEntry);
			}
		}

		protected virtual void _PlayMotionEntry(MotionEntry motionEntry) { }

		protected void Reset()
		{
			if(!m_animator)
			{
				m_animator = GetComponent<Animator>();
			}
		}

#if UNITY_EDITOR
		public void SetState(string stateName,int layer = 0,float normalizedTime = 0.0f)
		{
			if(!m_animator)
			{
				return;
			}

			m_animator.Play(stateName,layer,normalizedTime);
			m_animator.Update(0.0f);
		}


		[BoxGroup("Animation",ShowLabel = false)]
		[VerticalGroup("Animation/0"),SerializeField,ValueDropdown(nameof(StateNameGroup))]
		protected string m_stateName = null;

		private bool m_isPlayingInEditor = false;
		private float m_lastTime = 0.0f;

		[HorizontalGroup("Animation/1"),Button(Name = "",Icon = SdfIconType.PlayFill,ButtonHeight = 30),EnableIf(nameof(IsValidState))]
		protected void _OnPlayAnimationInEditor()
		{
			if(!m_animator)
			{
				return;
			}

			EditorApplication.update -= _OnUpdateInEditor;
			EditorApplication.update += _OnUpdateInEditor;

			m_animator.Play(m_stateName);

			m_lastTime = Convert.ToSingle(EditorApplication.timeSinceStartup);
			m_isPlayingInEditor = true;
		}

		[HorizontalGroup("Animation/1"),Button(Name = "",Icon = SdfIconType.StopFill,ButtonHeight = 30),EnableIf(nameof(IsValidState))]
		protected void _OnStopAnimationInEditor()
		{
			m_isPlayingInEditor = false;

			EditorApplication.update -= _OnUpdateInEditor;
		}

		private void _OnUpdateInEditor()
		{
			if(!m_animator)
			{
				return;
			}

			if(!m_isPlayingInEditor)
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