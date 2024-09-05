using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZSchedule
{
	[RequireComponent(typeof(Animator))]
	public class AnimatorProgressSchedule : ProgressSchedule
	{
		[SerializeField]
		private Animator m_Animator = null;

		[SerializeField,ValueDropdown(nameof(GetAnimatorNameList))]
		private string m_AnimationName = null;

		protected override void Initialize()
		{
			base.Initialize();

			m_Animator.speed = 0.0f;

			SetAnimatorTime(0.0f);
		}

		protected async override UniTask DoPlayScheduleAsync(ScheduleParam _param)
		{
			if(_param is ProgressParam param && param.Duration == null)
			{
				SetAnimatorTime(1.0f);
				UpdateAnimatorInEditor();

				return;
			}

			await base.DoPlayScheduleAsync(_param);
		}

		protected override void SetProgress(float _progress)
		{
			if(!IsExistAnimator || !m_Animator.isActiveAndEnabled)
			{
				return;
			}

			SetAnimatorTime(_progress);
			UpdateAnimatorInEditor();
		}

		private void SetAnimatorTime(float _time)
		{
			m_Animator.Play(m_AnimationName,0,_time);
		}

		private void UpdateAnimatorInEditor()
		{
#if UNITY_EDITOR
			m_Animator.Update(Time.deltaTime);
#endif
		}

		private bool IsExistAnimator => m_Animator && m_Animator.runtimeAnimatorController;

		private IEnumerable GetAnimatorNameList()
		{
			var nameList = new List<string>();

			if(!IsExistAnimator)
			{
				m_AnimationName = null;

				return nameList;
			}

#if UNITY_EDITOR
			var controller = m_Animator.GetAnimatorController(out var _);

			foreach(var layer in controller.layers)
			{
				foreach(var child in layer.stateMachine.states)
				{
					nameList.Add(child.state.name);
				}
			}
#endif
			return nameList;
		}

		protected override void Reset()
		{
			base.Reset();

			if(!m_Animator)
			{
				m_Animator = GetComponent<Animator>();
			}
		}
	}
}