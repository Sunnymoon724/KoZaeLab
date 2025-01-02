using UnityEngine;
using System.Linq;
using KZLib.KZAttribute;

namespace KZLib
{
	// TODO 이벤트 부분 개발 중

	/// <summary>
	/// 매 프레임 체크하며 애니메이션 실행함 (애니메이션 확인도 추가)
	/// </summary>
	[RequireComponent(typeof(Animator)),ExecuteInEditMode]
	public abstract class AnimatorCon : BaseComponent
	{
		[SerializeField]
		protected Animator m_Animator = null;

		[SerializeField,KZRichText]
		private string m_CurrentStateName = null;

		private float m_CurrentTime = 0.0f;
		private float m_Duration = 0.0f;

		public bool IsPlaying { get; private set; }
		public bool IsLoop { get; private set; }

		private bool IsExistAnimator
		{
			get
			{
				if(!m_Animator)
				{
					return false;
				}

				if(!m_Animator.runtimeAnimatorController)
				{
					return false;
				}

				return true;
			}
		}

		protected override void Reset()
		{
			base.Reset();

			if(!m_Animator)
			{
				m_Animator = GetComponent<Animator>();
			}
		}

		private void Update()
		{
			if(!IsPlaying)
			{
				return;
			}

			UpdateAnimation();
		}

		private float DeltaTime => m_Animator.updateMode == AnimatorUpdateMode.UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

		public void PlayAnimation(string _stateName,float? _duration = null,bool _isLoop = false,int _metaId = -1)
		{
			if(!IsExistAnimator)
			{
				return;
			}

			m_Animator.speed = 0.0f;
			m_CurrentTime = 0.0f;
			m_CurrentStateName = _stateName;

			if(_duration <= 0.0f)
			{
				_PlayAnimation(m_CurrentStateName,0,1.0f);

				m_Duration = 0.0f;
				IsLoop = false;
				IsPlaying = false;
			}
			else
			{
				

				if(_metaId > 0)
				{
					// 액션 이벤트 추가
					// var data = MetaDataMgr.In.Get<MetaData.ColorData>(_metaId);
				}

				m_Duration = _duration ?? GetClipLength(m_CurrentStateName);

				IsLoop = _isLoop;
				IsPlaying = true;
			}
		}

		public bool IsExistAnimationClip(string _stateName)
		{
			return m_Animator.runtimeAnimatorController.animationClips.Any(x => x.name.Contains(_stateName));
		}

		private void _PlayAnimation(string _stateName,int _layer,float _time)
		{
			m_Animator.Play(_stateName,_layer,_time);
	#if UNITY_EDITOR
			m_Animator.Update(Time.deltaTime);
	#endif
		}

		private float GetClipLength(string _stateName)
		{
			foreach(var clip in m_Animator.runtimeAnimatorController.animationClips)
			{
				if(clip.name.Contains(_stateName))
				{
					return clip.length;
				}
			}

			return 0.0f;
		}

		private void UpdateAnimation()
		{
			m_CurrentTime += DeltaTime;

			var percent = m_CurrentTime/m_Duration;
			var frame = Mathf.FloorToInt(m_CurrentTime*Global.FRAME_RATE_60);

			// 메타데이터 사용 시 해당 프레임으로 이벤트 체크

			_PlayAnimation(m_CurrentStateName,0,percent);

			if(percent >= 1.0f)
			{
				if(IsLoop)
				{
					m_CurrentTime = 0.0f;
				}
				else
				{
					IsPlaying = false;
				}
			}
		}
	}
}