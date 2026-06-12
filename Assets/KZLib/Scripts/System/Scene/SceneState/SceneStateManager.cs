using System;
using KZLib.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using Cysharp.Threading.Tasks;
using KZLib.Utilities;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	public record SceneChangeInfo(CommonUINameTag TransitionNameTag,bool UseLoading = true);

	[SingletonConfig(AutoCreate = true,DontDestroy = true)]
	public class SceneStateManager : SingletonMB<SceneStateManager>
	{
		private const float c_unloadMinTime = 300.0f;

		private bool m_isSceneChanging = false;
		public bool IsSceneChanging => m_isSceneChanging;

		[HorizontalGroup("Name",Order = 0),ShowInInspector,KZRichText]
		protected string CurrentSceneName
		{
			get
			{
				var current = CurrentScene;

				return current == null ? "Scene is none." : current.SceneName;
			}
		}

		[HorizontalGroup("Stack",Order = 1),SerializeField,DisplayAsString,ListDrawerSettings(ShowFoldout = false,IsReadOnly = true)]
		private Stack<SceneState> m_sceneStateStack = new();

		private SceneState CurrentScene => m_sceneStateStack.Count > 0 ? m_sceneStateStack.Peek() : null;

		private float m_lastUnloadTime = 0.0f;

		protected override void _Initialize()
		{
			base._Initialize();

			Application.lowMemory += _OnLowMemory;
			SceneManager.sceneUnloaded += _OnUnloadSceneResources;
		}

		protected override void _Release()
		{
			_DrainSceneStack();

			Application.lowMemory -= _OnLowMemory;
			SceneManager.sceneUnloaded -= _OnUnloadSceneResources;

			base._Release();

			KZMemoryKit.ClearUnloadedAssetMemory();

#if UNITY_EDITOR
			EditorUtility.UnloadUnusedAssetsImmediate(true);
#endif
		}

		public void ReloadScene(SceneChangeInfo changeInfo)
		{
			var current = CurrentScene ?? throw new InvalidOperationException("No current scene to reload.");

			_ChangeSceneAsync(current.SceneName,changeInfo).Forget();
		}

		public UniTask ReloadSceneAsync(SceneChangeInfo changeInfo)
		{
			var current = CurrentScene ?? throw new InvalidOperationException("No current scene to reload.");

			return _ChangeSceneAsync(current.SceneName,changeInfo);
		}

		public void ChangeScene(string sceneName,SceneChangeInfo changeInfo)
		{
			_ChangeSceneAsync(sceneName,changeInfo).Forget();
		}

		public UniTask ChangeSceneAsync(string sceneName,SceneChangeInfo changeInfo)
		{
			return _ChangeSceneAsync(sceneName,changeInfo);
		}

		private async UniTask _ChangeSceneAsync(string sceneName,SceneChangeInfo changeInfo)
		{
			async UniTask _CreateTaskAsync(Action<float> onUpdateProgress)
			{
				await _CreateSceneAsync(sceneName,onUpdateProgress);
			}

			async UniTask _DestroyTaskAsync(Action<float> onUpdateProgress)
			{
				await _DestroySceneAsync(false,onUpdateProgress);
			}

			await _PlaySceneAsync(changeInfo,_DestroyTaskAsync,_CreateTaskAsync);
		}

		public void AddScene(string sceneName,SceneChangeInfo changeInfo)
		{
			_AddSceneAsync(sceneName,changeInfo).Forget();
		}

		public UniTask AddSceneAsync(string sceneName,SceneChangeInfo changeInfo)
		{
			return _AddSceneAsync(sceneName,changeInfo);
		}

		private async UniTask _AddSceneAsync(string sceneName,SceneChangeInfo changeInfo)
		{
			async UniTask _CreateTaskAsync(Action<float> onUpdateProgress)
			{
				await _CreateSceneAsync(sceneName,onUpdateProgress);
			}

			await _PlaySceneAsync(changeInfo,_CreateTaskAsync);
		}

		public void RemoveScene(SceneChangeInfo changeInfo)
		{
			_ = CurrentScene ?? throw new InvalidOperationException("No current scene to remove.");

			_RemoveSceneAsync(changeInfo).Forget();
		}

		public UniTask RemoveSceneAsync(SceneChangeInfo changeInfo)
		{
			_ = CurrentScene ?? throw new InvalidOperationException("No current scene to remove.");

			return _RemoveSceneAsync(changeInfo);
		}

		private async UniTask _RemoveSceneAsync(SceneChangeInfo changeInfo)
		{
			async UniTask _DestroyTaskAsync(Action<float> onUpdateProgress)
			{
				await _DestroySceneAsync(true,onUpdateProgress);
			}

			await _PlaySceneAsync(changeInfo,_DestroyTaskAsync);
		}

		private async UniTask _PlaySceneAsync(SceneChangeInfo changeInfo,params Func<Action<float>,UniTask>[] onPlayTaskArray)
		{
			if(IsSceneChanging)
			{
				LogChannel.Scene.W("Scene change is already in progress.");

				throw new InvalidOperationException("Scene change is already in progress.");
			}

			m_isSceneChanging = true;

			KZInputKit.LockInput();

			var transitionNameTag = changeInfo.TransitionNameTag;
			var needsTransitionIn = false;

			try
			{
				// darker
				await UIManager.In.PlayTransitionOutAsync(transitionNameTag,false);

				needsTransitionIn = true;

				if(changeInfo.UseLoading)
				{
					var panel = UIManager.In.Open(CommonUINameTag.LoadingPanel) as LoadingPanel;

					if(panel == null)
					{
						throw new NullReferenceException("LoadingPanel is null.");
					}

					try
					{
						// brighter
						await UIManager.In.PlayTransitionInAsync(transitionNameTag,false);

						needsTransitionIn = false;

						var count = (float) onPlayTaskArray.Length;

						for(var i=0;i<onPlayTaskArray.Length;i++)
						{
							var taskIndex = i;

							void _UpdateProgress(float progress)
							{
								panel.SetLoadingProgress((taskIndex+progress)/count);
							}

							await onPlayTaskArray[i].Invoke(_UpdateProgress);
						}

						// darker
						await UIManager.In.PlayTransitionOutAsync(transitionNameTag,false);

						needsTransitionIn = true;
					}
					finally
					{
						UIManager.In.Close(CommonUINameTag.LoadingPanel);
					}
				}
				else
				{
					for(var i=0;i<onPlayTaskArray.Length;i++)
					{
						await onPlayTaskArray[i].Invoke(null);
					}
				}

				// brighter
				await UIManager.In.PlayTransitionInAsync(transitionNameTag,true);

				needsTransitionIn = false;
			}
			catch
			{
				if(needsTransitionIn)
				{
					try
					{
						await UIManager.In.PlayTransitionInAsync(transitionNameTag,true);
					}
					catch(Exception exception)
					{
						LogChannel.Scene.W($"Failed to restore transition: {exception.Message}");
					}
				}

				throw;
			}
			finally
			{
				KZInputKit.UnLockInput();

				m_isSceneChanging = false;
			}
		}

		private async UniTask _CreateSceneAsync(string sceneName,Action<float> onUpdateProgress)
		{
			if(sceneName.IsEmpty())
			{
				throw new ArgumentException("Scene name is empty.");
			}

			onUpdateProgress?.Invoke(0.0f);

			LogChannel.Scene.I($"{sceneName} create start.");

			var sceneState = SceneStateRegistry.Create(sceneName);

			void _UpdateProgress(float progress)
			{
				_UpdateProgressCommon(onUpdateProgress,progress);
			}

			await sceneState.InitializeAsync(_UpdateProgress);

			m_sceneStateStack.Push(sceneState);

			LogChannel.Scene.I($"{sceneName} create end.");

			onUpdateProgress?.Invoke(1.0f);
		}

		private async UniTask _DestroySceneAsync(bool activePreviousScene,Action<float> onUpdateProgress)
		{
			var current = CurrentScene;

			if(current == null)
			{
				return;
			}

			onUpdateProgress?.Invoke(0.0f);

			LogChannel.Scene.I($"{current.SceneName} destroy start.");

			var previousSceneName = _FetchPreviousSceneName(activePreviousScene);

			void _UpdateProgress(float progress)
			{
				_UpdateProgressCommon(onUpdateProgress,progress);
			}

			await current.ReleaseAsync(previousSceneName,_UpdateProgress);

			m_sceneStateStack.Pop();

			KZMemoryKit.ClearUnloadedAssetMemory();

#if UNITY_EDITOR
			EditorUtility.UnloadUnusedAssetsImmediate(true);
#endif
			LogChannel.Scene.I($"{current.SceneName} destroy end.");

			_OnLowMemory();

			onUpdateProgress?.Invoke(1.0f);
		}

		private string _FetchPreviousSceneName(bool activePreviousScene)
		{
			if(!activePreviousScene || m_sceneStateStack.Count <= 1)
			{
				return null;
			}

			var stackArray = m_sceneStateStack.ToArray();

			return stackArray[1].SceneName;
		}

		private void _DrainSceneStack()
		{
			while(m_sceneStateStack.Count > 0)
			{
				var current = m_sceneStateStack.Peek();
				var previousSceneName = _FetchPreviousSceneName(true);

				current.ReleaseAsync(previousSceneName,null).GetAwaiter().GetResult();

				m_sceneStateStack.Pop();
			}
		}

		private void _OnLowMemory()
		{
			if(Time.unscaledTime-m_lastUnloadTime < c_unloadMinTime)
			{
				return;
			}

			m_lastUnloadTime = Time.unscaledTime;

			KZMemoryKit.ClearUnloadedAssetMemory();
		}

		private void _OnUnloadSceneResources(Scene scene)
		{
			if(AddressablesManager.HasInstance)
			{
				AddressablesManager.In.ReleaseResources(scene.name);
			}
		}

		private void _UpdateProgressCommon(Action<float> onUpdateProgress,float progress)
		{
			onUpdateProgress?.Invoke(progress*0.99f);
		}
	}
}