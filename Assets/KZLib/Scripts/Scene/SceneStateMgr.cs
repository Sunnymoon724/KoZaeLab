using System;
using System.Collections.Generic;
using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using UnityEngine;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using TransitionPanel;

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

		private float m_lastUnloadTime = 0.0f;

		protected override void Initialize()
		{
			Application.lowMemory += OnLowMemory;
		}

		protected override void Release()
		{
			Application.lowMemory -= OnLowMemory;

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
			await _PlaySceneAsync(info,isLoading,_DestroySceneAsync,async (progress)=> { await _CreateSceneAsync(sceneName,progress,param); });
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
			await _PlaySceneAsync(info,isLoading,_DestroySceneAsync);
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
				var panel = UIMgr.In.Open<LoadingPanelUI>(UITag.LoadingPanelUI);

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

				UIMgr.In.Close(UITag.LoadingPanelUI);
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
				LogTag.System.E("Scene name is empty.");

				return;
			}

			onUpdateProgress?.Invoke(0.0f);

			LogTag.System.I($"{sceneName} create start.");

			var sceneType = Type.GetType($"{sceneName}, Assembly-CSharp");

			if(sceneType == null)
			{
				LogTag.System.E($"{sceneName} is not exists.");

				return;
			}

			if(Activator.CreateInstance(sceneType) is not SceneState sceneState)
			{
				LogTag.System.E($"{sceneName} create failed.");

				return;
			}

			m_sceneStateStack.Push(sceneState);

			await sceneState.InitializeAsync((progress)=>
			{
				onUpdateProgress?.Invoke(progress*0.99f);
			},param);

			LogTag.System.I($"{sceneName} create end.");

			onUpdateProgress?.Invoke(1.0f);
		}

		private async UniTask _DestroySceneAsync(Action<float> onUpdateProgress)
		{
			var current = CurrentScene;

			if(current == null)
			{
				return;
			}

			onUpdateProgress?.Invoke(0.0f);

			var sceneName = current.SceneName;

			LogTag.System.I($"{sceneName} destroy start.");

			m_sceneStateStack.Pop();

			var previous = CurrentScene;

			// Release current scene & Active on previous scene
			await current.ReleaseAsync(previous?.SceneName,(progress)=>
			{
				onUpdateProgress?.Invoke(progress*0.99f);
			});

#if UNITY_EDITOR
			EditorUtility.UnloadUnusedAssetsImmediate(true);
#endif
			LogTag.System.I($"{sceneName} destroy end.");

			OnLowMemory();

			onUpdateProgress?.Invoke(1.0f);
		}

		private SceneState CurrentScene => m_sceneStateStack.Count > 0 ? m_sceneStateStack.Peek() : null;

		private void OnLowMemory()
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