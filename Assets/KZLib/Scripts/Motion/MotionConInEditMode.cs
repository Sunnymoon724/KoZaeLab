#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.Animations;

namespace KZLib
{
	[ExecuteInEditMode]
	public class MotionConInEditMode : MotionCon
	{
		[SerializeField,ValueDropdown(nameof(StateNameGroup))]
		private string m_stateName = null;
		private bool m_isPlaying = false;
		private double m_lastTime = 0.0d;

		private IEnumerable<string> StateNameGroup
		{
			get
			{
				if(!m_animator)
				{
					yield break;
				}

				var controller = m_animator.runtimeAnimatorController as AnimatorController;

				foreach(var layer in controller.layers)
				{
					foreach(var child in layer.stateMachine.states)
					{
						yield return child.state.name;
					}
				}
			}
		}

		private void OnEnable()
		{
			EditorApplication.update += UpdateInEditMode;
		}

		private void OnDisable()
		{
			EditorApplication.update -= UpdateInEditMode;
		}

		[HorizontalGroup("0"),Button("",ButtonHeight = 30,Icon = SdfIconType.PlayFill)]
		protected void OnPlayAnimationInEditMode()
		{
			if(!m_animator)
			{
				Logger.System.W("Animator is null");

				return;
			}

			if(m_stateName.IsEmpty())
			{
				return;
			}

			m_animator.Play(m_stateName);

			m_lastTime = EditorApplication.timeSinceStartup;
			m_isPlaying = true;
		}

		[HorizontalGroup("0"),Button("",ButtonHeight = 30,Icon = SdfIconType.StopFill)]
		protected void OnStopAnimationInEditMode()
		{
			m_isPlaying = false;
		}

		void UpdateInEditMode()
		{
			if(!m_isPlaying || !m_animator)
			{
				return;
			}

			double currentTime = EditorApplication.timeSinceStartup;
			float deltaTime = (float)(currentTime - m_lastTime);
			m_lastTime = currentTime;

			m_animator.Update(deltaTime);
			SceneView.RepaintAll();
		}
	}
}
#endif