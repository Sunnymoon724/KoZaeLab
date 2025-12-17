using System;
using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using UnityEngine;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using KZLib.KZData;

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

		public void ReloadSceneWithLoading(UINameType transitionNameType)
		{
			_ReloadScene(transitionNameType,true);
		}

		public void ReloadSceneNoLoading(UINameType transitionNameType)
		{
			_ReloadScene(transitionNameType,false);
		}

		private void _ReloadScene(UINameType transitionNameType,bool isLoading)
		{
			var current = CurrentScene;

			if(current == null)
			{
				return;
			}

			_ChangeScene(current.SceneName,transitionNameType,isLoading);
		}

		public void ChangeSceneWithLoading(string sceneName,UINameType transitionNameType)
		{
			_ChangeScene(sceneName,transitionNameType,true);
		}

		public void ChangeSceneNoLoading(string sceneName,UINameType transitionNameType)
		{
			_ChangeScene(sceneName,transitionNameType,false);
		}

		private void _ChangeScene(string sceneName,UINameType transitionNameType,bool isLoading)
		{
			_ChangeSceneAsync(sceneName,transitionNameType,isLoading).Forget();
		}

		public async UniTask ChangeSceneWithLoadingAsync(string sceneName,UINameType transitionNameType)
		{
			await _ChangeSceneAsync(sceneName,transitionNameType,true);
		}

		public async UniTask ChangeSceneNoLoadingAsync(string sceneName,UINameType transitionNameType)
		{
			await _ChangeSceneAsync(sceneName,transitionNameType,false);
		}

		private async UniTask _ChangeSceneAsync(string sceneName,UINameType transitionNameType,bool isLoading)
		{
			await _PlaySceneAsync(transitionNameType,isLoading,async (progress)=> { await _CreateSceneAsync(sceneName,progress); },async (progress)=> { await _DestroySceneAsync(false,progress); });
		}

		public void AddSceneWithLoading(string sceneName,UINameType transitionNameType)
		{
			_AddScene(sceneName,transitionNameType,true);
		}

		public void AddSceneNoLoading(string sceneName,UINameType transitionNameType)
		{
			_AddScene(sceneName,transitionNameType,false);
		}

		private void _AddScene(string sceneName,UINameType transitionNameType,bool isLoading)
		{
			_AddSceneAsync(sceneName,transitionNameType,isLoading).Forget();
		}

		public async UniTask AddSceneWithLoadingAsync(string sceneName,UINameType transitionNameType)
		{
			await _AddSceneAsync(sceneName,transitionNameType,true);
		}

		public async UniTask AddSceneNoLoadingAsync(string sceneName,UINameType transitionNameType)
		{
			await _AddSceneAsync(sceneName,transitionNameType,false);
		}

		private async UniTask _AddSceneAsync(string sceneName,UINameType transitionNameType,bool isLoading)
		{
			await _PlaySceneAsync(transitionNameType,isLoading,async (progress)=> { await _CreateSceneAsync(sceneName,progress); });
		}

		public void RemoveSceneWithLoading(UINameType transitionNameType)
		{
			_RemoveScene(transitionNameType,true);
		}

		public void RemoveSceneNoLoading(UINameType transitionNameType)
		{
			_RemoveScene(transitionNameType,false);
		}

		private void _RemoveScene(UINameType transitionNameType,bool isLoading)
		{
			_RemoveSceneAsync(transitionNameType,isLoading).Forget();
		}

		public async UniTask RemoveSceneWithLoadingAsync(UINameType transitionNameType)
		{
			await _RemoveSceneAsync(transitionNameType,true);
		}

		public async UniTask RemoveSceneNoLoadingAsync(UINameType transitionNameType)
		{
			await _RemoveSceneAsync(transitionNameType,false);
		}

		private async UniTask _RemoveSceneAsync(UINameType transitionNameType,bool isLoading)
		{
			await _PlaySceneAsync(transitionNameType,isLoading,async (progress)=> { await _DestroySceneAsync(true,progress); });
		}

		private async UniTask _PlaySceneAsync(UINameType transitionNameType,bool isLoading,params Func<Action<float>,UniTask>[] onPlayTaskArray)
		{
			if(IsSceneChanging)
			{
				return;
			}

			m_isSceneChanging = true;

			CommonUtility.LockInput();

			// darker
			await UIManager.In.PlayTransitionOutAsync(transitionNameType,false);

			if(isLoading)
			{
				var panel = UIManager.In.Open(UINameType.LoadingPanelUI) as LoadingPanelUI;

				// brighter
				await UIManager.In.PlayTransitionInAsync(transitionNameType,false);

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
				await UIManager.In.PlayTransitionOutAsync(transitionNameType,false);

				UIManager.In.Close(UINameType.LoadingPanelUI);
			}
			else
			{
				foreach(var task in onPlayTaskArray)
				{
					await task.Invoke(null);
				}
			}

			// brighter
			await UIManager.In.PlayTransitionInAsync(transitionNameType,true);

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