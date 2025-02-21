using System;
using System.Collections.Generic;
using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using UnityEngine;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using TransitionPanel;
using System.Reflection;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	public class SceneMgr : AutoSingletonMB<SceneMgr>
	{
		private const float c_unload_min_time = 300.0f;

		private bool m_sceneChanging = false;
		public bool IsSceneChanging => m_sceneChanging;

		[HorizontalGroup("Name",Order = 0),ShowInInspector,KZRichText,LabelText("Current Scene Name")]
		protected string CurrentSceneName
		{
			get
			{
				var current = CurrentScene;

				return current == null ? "Scene is none." : current.SceneName;
			}
		}

		[HorizontalGroup("Stack",Order = 1),SerializeField,DisplayAsString,LabelText("Scene Stack"),ListDrawerSettings(ShowFoldout = false,IsReadOnly = true)]
		private Stack<SceneBinding> m_sceneStack = new();

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

		public void ReloadScene(TransitionData transitionData,bool isLoading,SceneBinding.BindingParam bindingParam = null)
		{
			var current = CurrentScene;

			if(current == null)
			{
				return;
			}

			ChangeScene(current.SceneName,transitionData,isLoading,bindingParam);
		}

		public void ChangeScene(string sceneName,TransitionData transitionData,bool isLoading,SceneBinding.BindingParam bindingParam = null)
		{
			ChangeSceneAsync(sceneName,transitionData,isLoading,bindingParam).Forget();
		}

		public async UniTask ChangeSceneAsync(string sceneName,TransitionData transitionData,bool isLoading,SceneBinding.BindingParam bindingParam = null)
		{
			await PlaySceneAsync(transitionData,isLoading,RemoveSceneAsync,async (progress)=> { await AddSceneAsync(sceneName,progress,bindingParam); });

			var current = CurrentScene;

			await current.PlayAsync();
		}

		public void AddScene(string sceneName,TransitionData transitionData,bool isLoading,SceneBinding.BindingParam bindingParam = null)
		{
			AddSceneAsync(sceneName,transitionData,isLoading,bindingParam).Forget();
		}

		public async UniTask AddSceneAsync(string sceneName,TransitionData transitionData,bool isLoading,SceneBinding.BindingParam bindingParam = null)
		{
			await PlaySceneAsync(transitionData,isLoading,async (progress)=> { await AddSceneAsync(sceneName,progress,bindingParam); });

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
			if(m_sceneChanging)
			{
				return;
			}

			m_sceneChanging = true;

			CommonUtility.LockInput();

			// darker
			await UIMgr.In.PlayTransitionOutAsync(transitionData,false);

			if(isLoading)
			{
				var panel = UIMgr.In.Open<LoadingPanelUI>(UITag.LoadingPanelUI);

				// brighter
				await UIMgr.In.PlayTransitionInAsync(transitionData,false);

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
				await UIMgr.In.PlayTransitionOutAsync(transitionData,false);

				UIMgr.In.Close(UITag.LoadingPanelUI);
			}
			else
			{
				foreach(var task in onTaskArray)
				{
					await task.Invoke(null);
				}
			}

			await UIMgr.In.PlayTransitionInAsync(transitionData,true);

			CommonUtility.UnLockInput();

			m_sceneChanging = false;
		}

		private async UniTask AddSceneAsync(string sceneName,Action<float> onUpdateProgress,SceneBinding.BindingParam bindingParam)
		{
			if(sceneName.IsEmpty())
			{
				LogTag.System.E("Scene name is empty.");

				return;
			}

			onUpdateProgress?.Invoke(0.0f);

			LogTag.System.I($"{sceneName} create start.");

			LogTag.Test.I(Assembly.GetExecutingAssembly());

			var sceneType = Type.GetType(sceneName) ?? throw new NullReferenceException($"{sceneName} is not exists.");
			var sceneBiding = (SceneBinding) Activator.CreateInstance(sceneType) ?? throw new NullReferenceException($"{sceneName} create failed.");

			m_sceneStack.Push(sceneBiding);

			await sceneBiding.InitializeAsync((progress)=>
			{
				onUpdateProgress?.Invoke(progress*0.99f);
			},bindingParam);

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

			m_sceneStack.Pop();

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

			CommonUtility.ClearUnloadedAssetMemory();

			onUpdateProgress?.Invoke(1.0f);
		}

		private SceneBinding CurrentScene => m_sceneStack.IsNullOrEmpty() ? null : m_sceneStack.Peek();

		private void OnLowMemory()
		{
			if(Time.unscaledTime-m_lastUnloadTime < c_unload_min_time)
			{
				return;
			}

			m_lastUnloadTime = Time.unscaledTime;
			Resources.UnloadUnusedAssets();
		}
	}
}