using UnityEngine;
using KZLib.KZAttribute;
using KZLib.KZData;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections;
using UnityEditor.Animations;

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
		[BoxGroup("Animation",ShowLabel = false)]
		[VerticalGroup("Animation/0"),SerializeField,ValueDropdown(nameof(StateNameList))]
		private string m_stateName = null;

		private bool m_isPlaying = false;
		private double m_lastTime = 0.0d;

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

			m_lastTime = EditorApplication.timeSinceStartup;
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

			var currentTime = EditorApplication.timeSinceStartup;

			m_lastTime = currentTime;

			m_animator.Update((float)(currentTime-m_lastTime));
			SceneView.RepaintAll();
		}

		private readonly ValueDropdownList<string> m_stateNameList = new();

		private IEnumerable StateNameList
		{
			get
			{
				if(m_animator && m_stateNameList.IsNullOrEmpty())
				{
					var controller = m_animator.runtimeAnimatorController as AnimatorController;
					
					if(controller != null)
                    {
                        foreach(var layer in controller.layers)
						{
							foreach(var child in layer.stateMachine.states)
							{
								m_stateNameList.Add(child.state.name);
							}
						}
                    }
				}

				return m_stateNameList;
			}
		}
		
		private bool IsValidState => !m_stateName.IsEmpty();
#endif
	}
}