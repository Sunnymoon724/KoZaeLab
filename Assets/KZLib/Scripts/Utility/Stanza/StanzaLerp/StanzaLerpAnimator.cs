using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Utilities
{
	[RequireComponent(typeof(Animator))]
	public class StanzaLerpAnimator : StanzaLerp
	{
		[SerializeField]
		private Animator m_animator = null;

		[SerializeField,ValueDropdown(nameof(StateNameGroup))]
		private string m_animationName = null;

		protected override void _Initialize()
		{
			base._Initialize();

			m_animator.speed = 0.0f;
			m_animator.Play(m_animationName,0,0.0f);
		}

		protected async override UniTask _DoPlayAsync(Stanza.Param param)
		{
			if(param is Param progressParam)
			{
				if(progressParam.Duration == null)
				{
					m_animator.Play(m_animationName,0,1.0f);
#if UNITY_EDITOR
					m_animator.Update(Time.deltaTime);
#endif
					return;
				}
			}

			await base._DoPlayAsync(param);
		}

		protected override void _SetProgress(float progress)
		{
			if(!IsExistAnimator || !m_animator.isActiveAndEnabled)
			{
				return;
			}

			m_animator.Play(m_animationName,0,progress);

	#if UNITY_EDITOR
			m_animator.Update(Time.deltaTime);
	#endif
		}

		private bool IsExistAnimator
		{
			get
			{
				if(!m_animator)
				{
					return false;
				}

				if(!m_animator.runtimeAnimatorController)
				{
					return false;
				}

				return true;
			}
		}

		private readonly List<string> m_stateNameList = new();

		private IEnumerable<string> StateNameGroup
		{
			get
			{
				if(!m_animator)
				{
					return System.Array.Empty<string>();
				}

				m_stateNameList.Clear();
				m_stateNameList.AddRange(m_animator.FindStateNameGroup());

				return m_stateNameList;
			}
		}

		private void Reset()
		{
			if(!m_animator)
			{
				m_animator = GetComponent<Animator>();
			}
		}
	}
}