using System;
using KZLib.UI;
using KZLib.Utilities;
using UnityEngine;
using UnityEngine.Playables;

namespace KZLib
{
	/// <summary>
	/// Loads, plays, skips, and tears down cut scenes via <see cref="PlayableDirector"/>.
	/// <see cref="LoadCutScene"/> <c>onComplete(true)</c> = played to end; <c>false</c> = skipped, interrupted, replaced, or externally destroyed.
	/// External destroy is reported by <see cref="FollowTargetWatcher"/> on the cut scene root.
	/// <see cref="KZInputKit"/> locks once per play session (<see cref="m_playingCutScene"/>); same-path replay does not lock again.
	/// Opens <c>SkipPanel</c> only when free, configured (<see cref="SkipPanel.IsSkipConfigured"/>), and owned via <see cref="m_ownsSkipPanel"/>.
	/// </summary>
	public class CutSceneManager : Singleton<CutSceneManager>
	{
		private PlayableDirector m_director = null;
		private GameObject m_cutSceneObject = null;

		private string m_currentCutScenePath = string.Empty;
		private bool m_playingCutScene = false;
		private bool m_isEndingSession = false;
		private bool m_isRemovingCutScene = false;
		private bool m_skipped = false;

		private bool m_useSkip = false;
		private bool m_ownsSkipPanel = false;
		private Action<bool> m_onComplete = null;

		public bool IsPlayingCutScene => m_playingCutScene;
		public double DirectorTime => m_director != null ? m_director.time : -1.0;

		private CutSceneManager() { }

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_onComplete = null;
				_EndCutSceneSession(invokeCallback : false);
			}

			base._Release(disposing);
		}

		public void LoadCutScene(string cutScenePath,bool useSkip,Action<bool> onComplete = null)
		{
			if(cutScenePath.IsEmpty())
			{
				throw new ArgumentNullException(nameof(cutScenePath),"CutScenePath must be assigned.");
			}

			//? Same path: replay without reload; input lock stays from the current session
			if(m_currentCutScenePath == cutScenePath)
			{
				m_onComplete = onComplete;
				_SyncSkipPanel(useSkip);
				_PlayCutScene();

				return;
			}

			//? Replacing another cut scene ends the previous session with completed:false
			if(m_director != null)
			{
				_EndCutSceneSession(invokeCallback : true,completed : false);
			}

			m_onComplete = onComplete;
			m_skipped = false;

			m_cutSceneObject = ResourceManager.In.GetObject(cutScenePath);

			if(m_cutSceneObject == null)
			{
				throw new ArgumentException($"CutScene is null. [{cutScenePath}].",nameof(cutScenePath));
			}

			//? GetComponentInChildren skips inactive directors — keep them active in the prefab
			m_director = m_cutSceneObject.GetComponentInChildren<PlayableDirector>();

			if(m_director == null)
			{
				m_cutSceneObject.DestroyObject();
				m_cutSceneObject = null;

				throw new InvalidOperationException($"Director does not exist in {cutScenePath}.");
			}

			m_cutSceneObject.GetOrAddComponent<FollowTargetWatcher>().Bind(_OnCutSceneDestroyedExternally);

			m_director.extrapolationMode = DirectorWrapMode.None;

			m_currentCutScenePath = cutScenePath;
			m_useSkip = useSkip && _TryOpenSkipPanel();

			_PlayCutScene();
		}

		private void _OnCutSceneStopped(PlayableDirector _)
		{
			_EndCutSceneSession(invokeCallback : true,completed : !m_skipped);
		}

		private void _OnCutSceneDestroyedExternally(GameObject cutSceneObject)
		{
			//? Ignore manager teardown or stale callbacks after reload
			if(m_isRemovingCutScene || m_cutSceneObject != cutSceneObject)
			{
				return;
			}

			if(m_director != null)
			{
				m_director.stopped -= _OnCutSceneStopped;
			}

			m_currentCutScenePath = string.Empty;
			m_director = null;
			m_cutSceneObject = null;

			_EndCutSceneSession(invokeCallback : true,completed : false,removeCutScene : false);
		}

		private void _PlayCutScene()
		{
			if(!m_playingCutScene)
			{
				KZInputKit.LockInput();
			}

			m_playingCutScene = true;
			m_skipped = false;

			//? Detach before Stop so same-path replay does not end the session
			m_director.stopped -= _OnCutSceneStopped;
			m_director.Stop();
			m_director.time = 0.0;
			m_director.Play();
			m_director.stopped += _OnCutSceneStopped;

			//? Zero-length timelines may fire stopped synchronously after Play
		}

		public void Stop()
		{
			if(m_director == null)
			{
				return;
			}

			m_skipped = true;
			m_director.Stop();
		}

		private void _SyncSkipPanel(bool useSkip)
		{
			if(m_useSkip == useSkip)
			{
				return;
			}

			if(m_useSkip)
			{
				_CloseSkipPanel();
			}

			m_useSkip = useSkip && _TryOpenSkipPanel();
		}

		private bool _TryOpenSkipPanel()
		{
			if(!UIManager.HasInstance)
			{
				LogChannel.UI.W("UIManager is not available. Cut scene skip was not opened.");

				return false;
			}

			if(UIManager.In.IsOpened(CommonUINameTag.SkipPanel))
			{
				throw new InvalidOperationException("SkipPanel is already in use. End the other session before starting a cut scene with skip.");
			}

			var skipPanel = UIManager.In.Register(CommonUINameTag.SkipPanel) as SkipPanel;

			if(skipPanel == null)
			{
				LogChannel.UI.E("SkipPanel is not registered.");

				return false;
			}

			if(!skipPanel.IsSkipConfigured)
			{
				LogChannel.UI.E("SkipPanel is not configured for skip. Set a non-zero show duration and hide duration at least 0.02s.");

				return false;
			}

			UIManager.In.Open(CommonUINameTag.SkipPanel,new SkipPanel.Param(Stop));

			m_ownsSkipPanel = true;

			return true;
		}

		private void _CloseSkipPanel()
		{
			if(!m_ownsSkipPanel || !UIManager.HasInstance)
			{
				return;
			}

			UIManager.In.Close(CommonUINameTag.SkipPanel);

			m_ownsSkipPanel = false;
		}

		private void _EndCutSceneSession(bool invokeCallback,bool completed = false,bool removeCutScene = true)
		{
			//? stopped and OnDestroy can overlap in the same frame
			if(m_isEndingSession)
			{
				return;
			}

			m_isEndingSession = true;
			m_skipped = false;

			if(removeCutScene)
			{
				_RemoveCutScene();
			}

			_CloseSkipPanel();

			m_useSkip = false;

			if(m_playingCutScene)
			{
				KZInputKit.UnLockInput();
			}

			m_playingCutScene = false;

			var onComplete = m_onComplete;
			m_onComplete = null;
			m_isEndingSession = false;

			//? onComplete may call LoadCutScene on the natural-end path
			if(invokeCallback)
			{
				onComplete?.Invoke(completed);
			}
		}

		private void _RemoveCutScene()
		{
			if(m_director == null)
			{
				return;
			}

			m_director.stopped -= _OnCutSceneStopped;

			if(m_cutSceneObject)
			{
				m_cutSceneObject.GetComponent<FollowTargetWatcher>()?.Unbind(_OnCutSceneDestroyedExternally);
			}

			m_isRemovingCutScene = true;

			m_cutSceneObject.DestroyObject();

			m_isRemovingCutScene = false;

			m_currentCutScenePath = string.Empty;
			m_director = null;
			m_cutSceneObject = null;
		}
	}
}
