using System;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Timeline을 이용한 컷씬 활용
/// </summary>

namespace KZLib
{
	public class TimelineMgr : Singleton<TimelineMgr>
	{
		private PlayableDirector m_Director = null;

		private GameObject m_CutSceneObject = null;

		private string m_CurrentCutScenePath = string.Empty;

		private bool m_PlayingCutScene = false;

		private bool m_CutScenePause = false;

		public bool IsCutScenePause => m_CutScenePause;

		public bool IsPlayingCutScene => m_PlayingCutScene;

		public double DirectorTime => m_Director != null ? m_Director.time : -1.0;

		protected override void Release(bool _disposing)
		{
			if(m_Disposed)
			{
				return;
			}

			if(_disposing)
			{
				RemoveCutScene();
			}

			base.Release(_disposing);
		}

		public void LoadCutScene(string _cutScenePath,bool _useSkip)
		{
			if(_cutScenePath.IsEmpty())
			{
				throw new NullReferenceException("컷씬의 경로가 없습니다.");
			}

			if(m_CurrentCutScenePath == _cutScenePath)
			{
				// 이미 재생 중
				return;
			}

			m_CutSceneObject = ResMgr.In.GetObject(_cutScenePath);

			if(m_CutSceneObject == null)
			{
				throw new NullReferenceException("컷씬이 없습니다.");
			}

			m_Director = m_CutSceneObject.GetComponentInChildren<PlayableDirector>();

			if(m_Director == null)
			{
				throw new NullReferenceException("컷씬에 디렉터가 없습니다.");
			}

			m_Director.extrapolationMode = DirectorWrapMode.None;
			m_Director.stopped += EndCutScene;

			m_CurrentCutScenePath = _cutScenePath;

			if(_useSkip)
			{
				// UIMgr.In.OpenOnce<Pan_Skip>(UI_TYPE.Pan_Skip,new Pan_Skip.ParamData(true,5.0f,board));
			}
			
			PlayCutScene();
		}

		private void PlayCutScene()
		{
			GameUtility.LockInput();

			m_CutScenePause = false;
			m_PlayingCutScene = true;

			m_Director.Play();
		}

		private void EndCutScene(PlayableDirector _director)
		{
			m_CutScenePause = false;
			
			RemoveCutScene();

			// 링크 끊고 스킵 창 지우기
			// UIMgr.In.Close(UI_TYPE.Pan_Skip);

			m_PlayingCutScene = false;

			GameUtility.UnLockInput();
		}

		private void RemoveCutScene()
		{
			if(m_Director == null)
			{
				return;
			}

			Tools.DestroyObject(m_CutSceneObject);

			m_CurrentCutScenePath = string.Empty;
			m_Director = null;
			m_CutSceneObject = null;
		}
	}
}