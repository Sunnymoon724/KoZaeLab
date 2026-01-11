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
			SceneManager.sceneLoaded += _OnUnloadSceneAssetBundle;
		}

		protected override void _Release()
		{
			base._Release();

			Application.lowMemory -= _OnLowMemory;
			SceneManager.sceneLoaded -= _OnUnloadSceneAssetBundle;

			CommonUtility.ClearUnloadedAssetMemory();

#if UNITY_EDITOR
			EditorUtility.UnloadUnusedAssetsImmediate(true);
#endif
		}

		public void ReloadSceneWithLoading(CommonUINameTag transitionNameTag)
		{
			_ReloadScene(transitionNameTag,true);
		}

		public void ReloadSceneNoLoading(CommonUINameTag transitionNameTag)
		{
			_ReloadScene(transitionNameTag,false);
		}

		private void _ReloadScene(CommonUINameTag transitionNameTag,bool isLoading)
		{
			var current = CurrentScene;

			if(current == null)
			{
				return;
			}

			_ChangeScene(current.SceneName,transitionNameTag,isLoading);
		}

		public void ChangeSceneWithLoading(string sceneName,CommonUINameTag transitionNameTag)
		{
			_ChangeScene(sceneName,transitionNameTag,true);
		}

		public void ChangeSceneNoLoading(string sceneName,CommonUINameTag transitionNameTag)
		{
			_ChangeScene(sceneName,transitionNameTag,false);
		}

		private void _ChangeScene(string sceneName,CommonUINameTag transitionNameTag,bool isLoading)
		{
			_ChangeSceneAsync(sceneName,transitionNameTag,isLoading).Forget();
		}

		public async UniTask ChangeSceneWithLoadingAsync(string sceneName,CommonUINameTag transitionNameTag)
		{
			await _ChangeSceneAsync(sceneName,transitionNameTag,true);
		}

		public async UniTask ChangeSceneNoLoadingAsync(string sceneName,CommonUINameTag transitionNameTag)
		{
			await _ChangeSceneAsync(sceneName,transitionNameTag,false);
		}

		private async UniTask _ChangeSceneAsync(string sceneName,CommonUINameTag transitionNameTag,bool isLoading)
		{
			async UniTask _CreateTaskAsync(Action<float> onUpdateProgress)
			{
				await _CreateSceneAsync(sceneName,onUpdateProgress);
			}

			async UniTask _DestroyTaskAsync(Action<float> onUpdateProgress)
			{
				await _DestroySceneAsync(false,onUpdateProgress);
			}

			await _PlaySceneAsync(transitionNameTag,isLoading,_CreateTaskAsync,_DestroyTaskAsync);
		}

		public void AddSceneWithLoading(string sceneName,CommonUINameTag transitionNameTag)
		{
			_AddScene(sceneName,transitionNameTag,true);
		}

		public void AddSceneNoLoading(string sceneName,CommonUINameTag transitionNameTag)
		{
			_AddScene(sceneName,transitionNameTag,false);
		}

		private void _AddScene(string sceneName,CommonUINameTag transitionNameTag,bool isLoading)
		{
			_AddSceneAsync(sceneName,transitionNameTag,isLoading).Forget();
		}

		public async UniTask AddSceneWithLoadingAsync(string sceneName,CommonUINameTag transitionNameTag)
		{
			await _AddSceneAsync(sceneName,transitionNameTag,true);
		}

		public async UniTask AddSceneNoLoadingAsync(string sceneName,CommonUINameTag transitionNameTag)
		{
			await _AddSceneAsync(sceneName,transitionNameTag,false);
		}

		private async UniTask _AddSceneAsync(string sceneName,CommonUINameTag transitionNameTag,bool isLoading)
		{
			async UniTask _CreateTaskAsync(Action<float> onUpdateProgress)
			{
				await _CreateSceneAsync(sceneName,onUpdateProgress);
			}

			await _PlaySceneAsync(transitionNameTag,isLoading,_CreateTaskAsync);
		}

		public void RemoveSceneWithLoading(CommonUINameTag transitionNameTag)
		{
			_RemoveScene(transitionNameTag,true);
		}

		public void RemoveSceneNoLoading(CommonUINameTag transitionNameTag)
		{
			_RemoveScene(transitionNameTag,false);
		}

		private void _RemoveScene(CommonUINameTag transitionNameTag,bool isLoading)
		{
			_RemoveSceneAsync(transitionNameTag,isLoading).Forget();
		}

		public async UniTask RemoveSceneWithLoadingAsync(CommonUINameTag transitionNameTag)
		{
			await _RemoveSceneAsync(transitionNameTag,true);
		}

		public async UniTask RemoveSceneNoLoadingAsync(CommonUINameTag transitionNameTag)
		{
			await _RemoveSceneAsync(transitionNameTag,false);
		}

		private async UniTask _RemoveSceneAsync(CommonUINameTag transitionNameTag,bool isLoading)
		{
			async UniTask _DestroyTaskAsync(Action<float> onUpdateProgress)
			{
				await _DestroySceneAsync(true,onUpdateProgress);
			}

			await _PlaySceneAsync(transitionNameTag,isLoading,_DestroyTaskAsync);
		}

		private async UniTask _PlaySceneAsync(CommonUINameTag transitionNameTag,bool isLoading,params Func<Action<float>,UniTask>[] onPlayTaskArray)
		{
			if(IsSceneChanging)
			{
				return;
			}

			m_isSceneChanging = true;

			CommonUtility.LockInput();

			// darker
			await UIManager.In.PlayTransitionOutAsync(transitionNameTag,false);

			if(isLoading)
			{
				var panel = UIManager.In.Open(CommonUINameTag.LoadingPanel) as LoadingPanel;

				// brighter
				await UIManager.In.PlayTransitionInAsync(transitionNameTag,false);

				var count = (float) onPlayTaskArray.Length;
				var percent = 0.0f;

				for(var i=0;i<onPlayTaskArray.Length;i++)
				{
					void _UpdateProgress(float progress)
					{
						percent += progress/count;

						panel.SetLoadingProgress(percent);
					}

					await onPlayTaskArray[i].Invoke(_UpdateProgress);
				}

				// darker
				await UIManager.In.PlayTransitionOutAsync(transitionNameTag,false);

				UIManager.In.Close(CommonUINameTag.LoadingPanel);
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

			void _UpdateProgress(float progress)
			{
				_UpdateProgressCommon(onUpdateProgress,progress);
			}

			await sceneState.InitializeAsync(_UpdateProgress);

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

			void _UpdateProgress(float progress)
			{
				_UpdateProgressCommon(onUpdateProgress,progress);
			}

            // Release current scene & Active on previous scene (if you want)
            await current.ReleaseAsync(previousSceneName,_UpdateProgress);

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
		
		private void _UpdateProgressCommon(Action<float> onUpdateProgress,float progress)
		{
			onUpdateProgress?.Invoke(progress*0.99f);
		}

		private async UniTask _CreateSceneCommonAsync(string sceneName,Action<float> onUpdateProgress)
		{
			await _CreateSceneAsync(sceneName,onUpdateProgress);
		}
	}
}