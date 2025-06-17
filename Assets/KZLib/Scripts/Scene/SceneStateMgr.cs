using System;
using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using UnityEngine;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using TransitionPanel;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	public class SceneStateMgr : AutoSingletonMB<SceneStateMgr>
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

		protected override void Initialize()
		{
			Application.lowMemory += _OnLowMemory;
		}

		protected override void Release()
		{
			Application.lowMemory -= _OnLowMemory;

			CommonUtility.ClearUnloadedAssetMemory();
		}

		public void ReloadSceneWithLoading(TransitionInfo info,SceneState.StateParam param = null)
		{
			_ReloadScene(info,true,param);
		}

		public void ReloadSceneNoLoading(TransitionInfo info,SceneState.StateParam param = null)
		{
			_ReloadScene(info,false,param);
		}

		private void _ReloadScene(TransitionInfo info,bool isLoading,SceneState.StateParam param = null)
		{
			var current = CurrentScene;

			if(current == null)
			{
				return;
			}

			_ChangeScene(current.SceneName,info,isLoading,param);
		}

		public void ChangeSceneWithLoading(string sceneName,TransitionInfo info,SceneState.StateParam param = null)
		{
			_ChangeScene(sceneName,info,true,param);
		}

		public void ChangeSceneNoLoading(string sceneName,TransitionInfo info,SceneState.StateParam param = null)
		{
			_ChangeScene(sceneName,info,false,param);
		}

		private void _ChangeScene(string sceneName,TransitionInfo info,bool isLoading,SceneState.StateParam param = null)
		{
			_ChangeSceneAsync(sceneName,info,isLoading,param).Forget();
		}

		public async UniTask ChangeSceneWithLoadingAsync(string sceneName,TransitionInfo info,SceneState.StateParam param = null)
		{
			await _ChangeSceneAsync(sceneName,info,true,param);
		}

		public async UniTask ChangeSceneNoLoadingAsync(string sceneName,TransitionInfo info,SceneState.StateParam param = null)
		{
			await _ChangeSceneAsync(sceneName,info,false,param);
		}

		private async UniTask _ChangeSceneAsync(string sceneName,TransitionInfo info,bool isLoading,SceneState.StateParam param = null)
		{
			await _PlaySceneAsync(info,isLoading,async (progress)=> { await _CreateSceneAsync(sceneName,progress,param); },async (progress)=> { await _DestroySceneAsync(false,progress); });
		}

		public void AddSceneWithLoading(string sceneName,TransitionInfo info,SceneState.StateParam param = null)
		{
			_AddScene(sceneName,info,true,param);
		}

		public void AddSceneNoLoading(string sceneName,TransitionInfo info,SceneState.StateParam param = null)
		{
			_AddScene(sceneName,info,false,param);
		}

		private void _AddScene(string sceneName,TransitionInfo info,bool isLoading,SceneState.StateParam param = null)
		{
			_AddSceneAsync(sceneName,info,isLoading,param).Forget();
		}

		public async UniTask AddSceneWithLoadingAsync(string sceneName,TransitionInfo info,SceneState.StateParam param = null)
		{
			await _AddSceneAsync(sceneName,info,true,param);
		}

		public async UniTask AddSceneNoLoadingAsync(string sceneName,TransitionInfo info,SceneState.StateParam param = null)
		{
			await _AddSceneAsync(sceneName,info,false,param);
		}

		private async UniTask _AddSceneAsync(string sceneName,TransitionInfo info,bool isLoading,SceneState.StateParam param = null)
		{
			await _PlaySceneAsync(info,isLoading,async (progress)=> { await _CreateSceneAsync(sceneName,progress,param); });
		}

		public void RemoveSceneWithLoading(TransitionInfo info)
		{
			_RemoveScene(info,true);
		}

		public void RemoveSceneNoLoading(TransitionInfo info)
		{
			_RemoveScene(info,false);
		}

		private void _RemoveScene(TransitionInfo info,bool isLoading)
		{
			_RemoveSceneAsync(info,isLoading).Forget();
		}

		public async UniTask RemoveSceneWithLoadingAsync(TransitionInfo info)
		{
			await _RemoveSceneAsync(info,true);
		}

		public async UniTask RemoveSceneNoLoadingAsync(TransitionInfo info)
		{
			await _RemoveSceneAsync(info,false);
		}

		private async UniTask _RemoveSceneAsync(TransitionInfo info,bool isLoading)
		{
			await _PlaySceneAsync(info,isLoading,async (progress)=> { await _DestroySceneAsync(true,progress); });
		}

		private async UniTask _PlaySceneAsync(TransitionInfo info,bool isLoading,params Func<Action<float>,UniTask>[] onPlayTaskArray)
		{
			if(IsSceneChanging)
			{
				return;
			}

			m_isSceneChanging = true;

			CommonUtility.LockInput();

			// darker
			await UIMgr.In.PlayTransitionOutAsync(info,false);

			if(isLoading)
			{
				var panel = UIMgr.In.Open<LoadingPanelUI>(Global.LOADING_PANEL_UI);

				// brighter
				await UIMgr.In.PlayTransitionInAsync(info,false);

				var count = (float) onPlayTaskArray.Length;
				var percent = 0.0f;

				foreach(var task in onPlayTaskArray)
				{
					await task.Invoke((progress)=>
					{
						percent += progress/count;

						panel.SetLoadingProgress(percent);
					});
				}

				// darker
				await UIMgr.In.PlayTransitionOutAsync(info,false);

				UIMgr.In.Close(Global.LOADING_PANEL_UI);
			}
			else
			{
				foreach(var task in onPlayTaskArray)
				{
					await task.Invoke(null);
				}
			}

			await UIMgr.In.PlayTransitionInAsync(info,true);

			CommonUtility.UnLockInput();

			m_isSceneChanging = false;
		}

		private async UniTask _CreateSceneAsync(string sceneName,Action<float> onUpdateProgress,SceneState.StateParam param)
		{
			if(sceneName.IsEmpty())
			{
				Logger.System.E("Scene name is empty.");

				return;
			}

			onUpdateProgress?.Invoke(0.0f);

			Logger.System.I($"{sceneName} create start.");

			var sceneType = Type.GetType($"{sceneName}, Assembly-CSharp");

			if(sceneType == null)
			{
				Logger.System.E($"{sceneName} is not exists.");

				return;
			}

			if(Activator.CreateInstance(sceneType) is not SceneState sceneState)
			{
				Logger.System.E($"{sceneName} create failed.");

				return;
			}

			m_sceneStateStack.Push(sceneState);

			await sceneState.InitializeAsync((progress)=>
			{
				onUpdateProgress?.Invoke(progress*0.99f);
			},param);

			Logger.System.I($"{sceneName} create end.");

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

			Logger.System.I($"{current.SceneName} destroy start.");

			// remove current scene
			m_sceneStateStack.Pop();

			var previousSceneName = activePreviousScene ? CurrentScene?.SceneName : null;

            // Release current scene & Active on previous scene (if you want)
            await current.ReleaseAsync(previousSceneName,(progress)=>
			{
				onUpdateProgress?.Invoke(progress*0.99f);
			});

#if UNITY_EDITOR
			EditorUtility.UnloadUnusedAssetsImmediate(true);
#endif
			Logger.System.I($"{current.SceneName} destroy end.");

			_OnLowMemory();

			onUpdateProgress?.Invoke(1.0f);
		}

		private void _OnLowMemory()
		{
			if(Time.unscaledTime-m_lastUnloadTime < c_unloadMinTime)
			{
				return;
			}

			m_lastUnloadTime = Time.unscaledTime;

			CommonUtility.ClearUnloadedAssetMemory();
		}
	}
}