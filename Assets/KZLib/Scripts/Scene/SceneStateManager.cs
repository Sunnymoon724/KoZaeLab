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
	public class SceneStateManager : AutoSingletonMB<SceneStateManager>
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

		public void ReloadScene(TransitionData transitionData,bool isLoading,SceneState.StateParam param = null)
		{
			var current = CurrentScene;

			if(current == null)
			{
				return;
			}

			ChangeScene(current.SceneName,transitionData,isLoading,param);
		}

		public void ChangeScene(string sceneName,TransitionData transitionData,bool isLoading,SceneState.StateParam param = null)
		{
			ChangeSceneAsync(sceneName,transitionData,isLoading,param).Forget();
		}

		public async UniTask ChangeSceneAsync(string sceneName,TransitionData transitionData,bool isLoading,SceneState.StateParam param = null)
		{
			await PlaySceneAsync(transitionData,isLoading,RemoveSceneAsync,async (progress)=> { await AddSceneAsync(sceneName,progress,param); });

			var current = CurrentScene;

			await current.PlayAsync();
		}

		public void AddScene(string sceneName,TransitionData transitionData,bool isLoading,SceneState.StateParam param = null)
		{
			AddSceneAsync(sceneName,transitionData,isLoading,param).Forget();
		}

		public async UniTask AddSceneAsync(string sceneName,TransitionData transitionData,bool isLoading,SceneState.StateParam param = null)
		{
			await PlaySceneAsync(transitionData,isLoading,async (progress)=> { await AddSceneAsync(sceneName,progress,param); });

			var current = CurrentScene;

			await current.PlayAsync();
		}

		public void RemoveCurrentScene(TransitionData transitionData,bool isLoading)
		{
			RemoveCurrentSceneAsync(transitionData,isLoading).Forget();
		}

		public async UniTask RemoveCurrentSceneAsync(TransitionData transitionData,bool isLoading)
		{
			await PlaySceneAsync(transitionData,isLoading,RemoveSceneAsync);
		}

		private async UniTask PlaySceneAsync(TransitionData transitionData,bool isLoading,params Func<Action<float>,UniTask>[] onTaskArray)
		{
			if(m_isSceneChanging)
			{
				return;
			}

			m_isSceneChanging = true;

			CommonUtility.LockInput();

			// darker
			await UIManager.In.PlayTransitionOutAsync(transitionData,false);

			if(isLoading)
			{
				var panel = UIManager.In.Open<LoadingPanelUI>(UITag.LoadingPanelUI);

				// brighter
				await UIManager.In.PlayTransitionInAsync(transitionData,false);

				var count = (float) onTaskArray.Length;
				var percent = 0.0f;

				foreach(var task in onTaskArray)
				{
					await task.Invoke((progress)=>
					{
						percent += progress/count;

						panel.SetLoadingProgress(percent);
					});
				}

				// darker
				await UIManager.In.PlayTransitionOutAsync(transitionData,false);

				UIManager.In.Close(UITag.LoadingPanelUI);
			}
			else
			{
				foreach(var task in onTaskArray)
				{
					await task.Invoke(null);
				}
			}

			await UIManager.In.PlayTransitionInAsync(transitionData,true);

			CommonUtility.UnLockInput();

			m_isSceneChanging = false;
		}

		private async UniTask AddSceneAsync(string sceneName,Action<float> onUpdateProgress,SceneState.StateParam param)
		{
			if(sceneName.IsEmpty())
			{
				LogTag.System.E("Scene name is empty.");

				return;
			}

			onUpdateProgress?.Invoke(0.0f);

			LogTag.System.I($"{sceneName} create start.");

			var sceneType = Type.GetType($"{sceneName}, Assembly-CSharp") ?? throw new NullReferenceException($"{sceneName} is not exists.");
			var sceneState = (SceneState) Activator.CreateInstance(sceneType) ?? throw new NullReferenceException($"{sceneName} create failed.");

			m_sceneStateStack.Push(sceneState);

			await sceneState.InitializeAsync((progress)=>
			{
				onUpdateProgress?.Invoke(progress*0.99f);
			},param);

			LogTag.System.I($"{sceneName} create end.");

			onUpdateProgress?.Invoke(1.0f);
		}

		private async UniTask RemoveSceneAsync(Action<float> onUpdateProgress)
		{
			var current = CurrentScene;

			if(current == null)
			{
				return;
			}

			onUpdateProgress?.Invoke(0.0f);

			var sceneName = current.SceneName;

			LogTag.System.I($"{sceneName} remove start.");

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
			LogTag.System.I($"{sceneName} remove end.");

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