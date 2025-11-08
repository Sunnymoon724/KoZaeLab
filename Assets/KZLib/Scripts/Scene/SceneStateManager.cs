using System;
using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using UnityEngine;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

		private SceneState CurrentScene => m_sceneStateStack.Count > 0 ? m_sceneStateStack.Peek() : null;

		private float m_lastUnloadTime = 0.0f;

		protected override void Initialize()
		{
			Application.lowMemory += _OnLowMemory;
			SceneManager.sceneLoaded += _OnUnloadSceneAssetBundle;
		}

		protected override void Release()
		{
			Application.lowMemory -= _OnLowMemory;
			SceneManager.sceneLoaded -= _OnUnloadSceneAssetBundle;

			CommonUtility.ClearUnloadedAssetMemory();

#if UNITY_EDITOR
			EditorUtility.UnloadUnusedAssetsImmediate(true);
#endif
		}

		public void ReloadSceneWithLoading(string transitionName)
		{
			_ReloadScene(transitionName,true);
		}

		public void ReloadSceneNoLoading(string transitionName)
		{
			_ReloadScene(transitionName,false);
		}

		private void _ReloadScene(string transitionName,bool isLoading)
		{
			var current = CurrentScene;

			if(current == null)
			{
				return;
			}

			_ChangeScene(current.SceneName,transitionName,isLoading);
		}

		public void ChangeSceneWithLoading(string sceneName,string transitionName)
		{
			_ChangeScene(sceneName,transitionName,true);
		}

		public void ChangeSceneNoLoading(string sceneName,string transitionName)
		{
			_ChangeScene(sceneName,transitionName,false);
		}

		private void _ChangeScene(string sceneName,string transitionName,bool isLoading)
		{
			_ChangeSceneAsync(sceneName,transitionName,isLoading).Forget();
		}

		public async UniTask ChangeSceneWithLoadingAsync(string sceneName,string transitionName)
		{
			await _ChangeSceneAsync(sceneName,transitionName,true);
		}

		public async UniTask ChangeSceneNoLoadingAsync(string sceneName,string transitionName)
		{
			await _ChangeSceneAsync(sceneName,transitionName,false);
		}

		private async UniTask _ChangeSceneAsync(string sceneName,string transitionName,bool isLoading)
		{
			await _PlaySceneAsync(transitionName,isLoading,async (progress)=> { await _CreateSceneAsync(sceneName,progress); },async (progress)=> { await _DestroySceneAsync(false,progress); });
		}

		public void AddSceneWithLoading(string sceneName,string transitionName)
		{
			_AddScene(sceneName,transitionName,true);
		}

		public void AddSceneNoLoading(string sceneName,string transitionName)
		{
			_AddScene(sceneName,transitionName,false);
		}

		private void _AddScene(string sceneName,string transitionName,bool isLoading)
		{
			_AddSceneAsync(sceneName,transitionName,isLoading).Forget();
		}

		public async UniTask AddSceneWithLoadingAsync(string sceneName,string transitionName)
		{
			await _AddSceneAsync(sceneName,transitionName,true);
		}

		public async UniTask AddSceneNoLoadingAsync(string sceneName,string transitionName)
		{
			await _AddSceneAsync(sceneName,transitionName,false);
		}

		private async UniTask _AddSceneAsync(string sceneName,string transitionName,bool isLoading)
		{
			await _PlaySceneAsync(transitionName,isLoading,async (progress)=> { await _CreateSceneAsync(sceneName,progress); });
		}

		public void RemoveSceneWithLoading(string transitionName)
		{
			_RemoveScene(transitionName,true);
		}

		public void RemoveSceneNoLoading(string transitionName)
		{
			_RemoveScene(transitionName,false);
		}

		private void _RemoveScene(string transitionName,bool isLoading)
		{
			_RemoveSceneAsync(transitionName,isLoading).Forget();
		}

		public async UniTask RemoveSceneWithLoadingAsync(string transitionName)
		{
			await _RemoveSceneAsync(transitionName,true);
		}

		public async UniTask RemoveSceneNoLoadingAsync(string transitionName)
		{
			await _RemoveSceneAsync(transitionName,false);
		}

		private async UniTask _RemoveSceneAsync(string transitionName,bool isLoading)
		{
			await _PlaySceneAsync(transitionName,isLoading,async (progress)=> { await _DestroySceneAsync(true,progress); });
		}

		private async UniTask _PlaySceneAsync(string transitionName,bool isLoading,params Func<Action<float>,UniTask>[] onPlayTaskArray)
		{
			if(IsSceneChanging)
			{
				return;
			}

			m_isSceneChanging = true;

			CommonUtility.LockInput();

			// darker
			await UIManager.In.PlayTransitionOutAsync(transitionName,false);

			if(isLoading)
			{
				var panel = UIManager.In.Open<LoadingPanelUI>(Global.LOADING_PANEL_UI);

				// brighter
				await UIManager.In.PlayTransitionInAsync(transitionName,false);

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
				await UIManager.In.PlayTransitionOutAsync(transitionName,false);

				UIManager.In.Close(Global.LOADING_PANEL_UI);
			}
			else
			{
				foreach(var task in onPlayTaskArray)
				{
					await task.Invoke(null);
				}
			}

			// brighter
			await UIManager.In.PlayTransitionInAsync(transitionName,true);

			CommonUtility.UnLockInput();

			m_isSceneChanging = false;
		}

		private async UniTask _CreateSceneAsync(string sceneName,Action<float> onUpdateProgress)
		{
			if(sceneName.IsEmpty())
			{
				LogSvc.System.E("Scene name is empty.");

				return;
			}

			onUpdateProgress?.Invoke(0.0f);

			LogSvc.System.I($"{sceneName} create start.");

			var sceneType = Type.GetType($"{sceneName}, Assembly-CSharp");

			if(sceneType == null)
			{
				LogSvc.System.E($"{sceneName} is not exists.");

				return;
			}

			if(Activator.CreateInstance(sceneType) is not SceneState sceneState)
			{
				LogSvc.System.E($"{sceneName} create failed.");

				return;
			}

			m_sceneStateStack.Push(sceneState);

			await sceneState.InitializeAsync((progress)=>
			{
				onUpdateProgress?.Invoke(progress*0.99f);
			});

			LogSvc.System.I($"{sceneName} create end.");

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

			LogSvc.System.I($"{current.SceneName} destroy start.");

			// remove current scene
			m_sceneStateStack.Pop();

			var previousSceneName = activePreviousScene ? CurrentScene?.SceneName : null;

            // Release current scene & Active on previous scene (if you want)
            await current.ReleaseAsync(previousSceneName,(progress)=>
			{
				onUpdateProgress?.Invoke(progress*0.99f);
			});
			
			// TODO 씬 전환시 사용하지 않는 어셋 삭제
			// AssetBundleManager.Instance.UnloadLoadeAssetBundle();
			CommonUtility.ClearUnloadedAssetMemory();

#if UNITY_EDITOR
			EditorUtility.UnloadUnusedAssetsImmediate(true);
#endif
			LogSvc.System.I($"{current.SceneName} destroy end.");

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
		
		private void _OnUnloadSceneAssetBundle(Scene scene, LoadSceneMode mode)
		{
			// TODO 어드레서블로 바뀐거 넣기
#if !UNITY_EDITOR
			AssetBundleManager.Instance.UnLoadAssetBundle(scene.name + ConfigData.AssetBundleExpend, false);
#endif
		}
	}
}