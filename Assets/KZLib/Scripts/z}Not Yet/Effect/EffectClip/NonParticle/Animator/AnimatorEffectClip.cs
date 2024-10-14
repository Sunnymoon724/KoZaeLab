// using UnityEngine;
// using Cysharp.Threading.Tasks;
// using System.Collections;
// using Sirenix.OdinInspector;
// using System.Collections.Generic;

// #if UNITY_EDITOR

// using UnityEditor.Animations;

// #endif

// public class AnimatorEffectClip : EffectClip
// {
// 	[SerializeField]
// 	private Animator m_Animator = null;

// 	[SerializeField,ValueDropdown(nameof(GetAnimatorNameList))]
// 	private string m_AnimationName = null;

// 	protected async override UniTask PlayTaskAsync()
// 	{
// 		if(m_AnimationName.IsEmpty())
// 		{
// 			return;
// 		}

// 		m_Animator.Play(m_AnimationName);

// 		await base.PlayTaskAsync();
// 	}

// 	private IEnumerable GetAnimatorNameList()
// 	{
// 		var nameList = new List<string>();

// 		if(!m_Animator)
// 		{
// 			m_AnimationName = null;

// 			return nameList;
// 		}

// #if UNITY_EDITOR
// 		var controller = m_Animator.runtimeAnimatorController as AnimatorController;

// 		foreach(var layer in controller.layers)
// 		{
// 			foreach(var child in layer.stateMachine.states)
// 			{
// 				nameList.Add(child.state.name);
// 			}
// 		}
// #endif
// 		return nameList;
// 	}

// 	protected override void Reset()
// 	{
// 		base.Reset();

// 		if(!m_Animator)
// 		{
// 			m_Animator = GetComponent<Animator>();
// 		}

// 		m_Duration = m_Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

// #if UNITY_EDITOR
// 		var controller = m_Animator.runtimeAnimatorController as AnimatorController;
// 		var stateMachine = controller.layers[0].stateMachine;

// 		m_AnimationName = stateMachine.states[0].state.name;
// #endif
// 	}
// }