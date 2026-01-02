using System;
using KZLib.KZData;
using KZLib.KZUtility;
using UnityEngine;
using UnityEngine.Playables;

namespace KZLib
{
	public class TimelineManager : Singleton<TimelineManager>
	{
		private bool m_disposed = false;

		private PlayableDirector m_director = null;
		private GameObject m_cutSceneObject = null;

		private string m_currentCutScenePath = string.Empty;
		private bool m_playingCutScene = false;
		private bool m_cutScenePause = false;
		
		private bool m_useSkip = false;

		public bool IsCutScenePause => m_cutScenePause;
		public bool IsPlayingCutScene => m_playingCutScene;
		public double DirectorTime => m_director != null ? m_director.time : -1.0;

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				_RemoveCutScene();
			}

			m_disposed = true;

			base.Release(disposing);
		}

		public void LoadCutScene(string cutScenePath,bool useSkip)
		{
			if(cutScenePath.IsEmpty())
			{
				throw new NullReferenceException("CutScenePath is null.");
			}

			if(m_currentCutScenePath == cutScenePath)
			{
				// already played
				return;
			}

			m_cutSceneObject = ResourceManager.In.GetObject(cutScenePath);

			if(m_cutSceneObject == null)
			{
				throw new NullReferenceException($"CutScene is null. [{cutScenePath}]");
			}

			m_director = m_cutSceneObject.GetComponentInChildren<PlayableDirector>();

			if(m_director == null)
			{
				throw new NullReferenceException($"director is not exist in {cutScenePath}");
			}
			
			void _EndCutScene(PlayableDirector _)
			{
				m_cutScenePause = false;

				_RemoveCutScene();
				
				if(m_useSkip)
				{
					UIManager.In.Close(CommonUINameTag.SkipPanelUI);
				}

				m_playingCutScene = false;

				CommonUtility.UnLockInput();
			}

			m_director.extrapolationMode = DirectorWrapMode.None;
			m_director.stopped += _EndCutScene;

			m_currentCutScenePath = cutScenePath;
			m_useSkip = useSkip;

			if(useSkip)
			{
				UIManager.In.Open(CommonUINameTag.SkipPanelUI,new SkipPanelUI.Param(Stop));
			}

			_PlayCutScene();
		}

		private void _PlayCutScene()
		{
			CommonUtility.LockInput();

			m_cutScenePause = false;
			m_playingCutScene = true;

			m_director.Play();
		}

		public void Stop()
		{
			m_director.Stop();
		}

		private void _RemoveCutScene()
		{
			if(m_director == null)
			{
				return;
			}

			m_cutSceneObject.DestroyObject();

			m_currentCutScenePath = string.Empty;
			m_director = null;
			m_cutSceneObject = null;
		}
	}
}