#if UNITY_EDITOR
using System.Collections.Generic;
using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

[ExecuteInEditMode]
public class MotionEditorCon : MonoBehaviour
{
	private GameObject m_pivot = null;
	private GameObject m_target = null;

	[HorizontalGroup("Target",Order = 0),ShowInInspector]
	private GameObject Target
	{
		get => m_target;
		set
		{
			if(value == null)
			{
				m_pivot = value;

				if(m_target != null)
				{
					m_target.DestroyObject();
				}

				m_target = value;

				return;
			}

			if(!value.HasComponent<Animator>())
			{
				Logger.System.W("Prefab must have animator component");

				return;
			}

			if(!value.HasComponent<MotionCon>())
			{
				Logger.System.W("Prefab must have motionController component");

				return;
			}

			if(m_pivot == value)
			{
				return;
			}

			if(m_target != null)
			{
				m_target.DestroyObject();
			}

			m_pivot = value;
			m_target = value.CopyObject() as GameObject;

			m_animator = m_target.GetComponent<Animator>();
		}
	}

	private bool IsExistTarget => Target;

	private Animator m_animator = null;

	[HorizontalGroup("StateName",Order = 1),SerializeField,ValueDropdown(nameof(StateNameGroup)),ShowIf(nameof(IsExistTarget))]
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

			foreach(var controllerLayer in controller.layers)
			{
				foreach(var animatorState in controllerLayer.stateMachine.states)
				{
					var state = animatorState.state;

					if(state.motion == null)
					{
						continue;
					}

					yield return state.name;
				}
			}
		}
	}

	private void OnEnable()
	{
		EditorApplication.update += _UpdateInEditMode;
	}

	private void OnDisable()
	{
		EditorApplication.update -= _UpdateInEditMode;
	}

	[HorizontalGroup("Button",Order = 5),Button("",ButtonHeight = 30,Icon = SdfIconType.PlayFill)]
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

	[HorizontalGroup("Button",Order = 5),Button("",ButtonHeight = 30,Icon = SdfIconType.StopFill)]
	protected void OnStopAnimationInEditMode()
	{
		m_isPlaying = false;
	}

	private void _UpdateInEditMode()
	{
		if(!m_isPlaying || !m_animator)
		{
			return;
		}

		var currentTime = EditorApplication.timeSinceStartup;
		var deltaTime = (float)(currentTime-m_lastTime);

		m_lastTime = currentTime;

		m_animator.Update(deltaTime);

		SceneView.RepaintAll();
	}

	[HorizontalGroup("Button2",Order = 6),Button("ExportToMotionProto",ButtonHeight = 30)]
	protected void ExportToMotionProto()
	{
		
	}
}
#endif