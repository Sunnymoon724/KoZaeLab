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
			SceneManager.sceneLoaded += _OnUnloadSceneAssetBundle;
		}

		protected override void _Release()
		{
			base._Release();

			Application.lowMemory -= _OnLowMemory;
			SceneManager.sceneLoaded -= _OnUnloadSceneAssetBundle;

			KZMemoryKit.ClearUnloadedAssetMemory();

#if UNITY_EDITOR
			EditorUtility.UnloadUnusedAssetsImmediate(true);
#endif
		}

		public void ReloadScene(SceneChangeInfo changeInfo)
		{
			var current = CurrentScene;

			if(current == null)
			{
				return;
			}

			_ChangeSceneAsync(current.SceneName,changeInfo).Forget();
		}

		public UniTask ReloadSceneAsync(SceneChangeInfo changeInfo)
		{
			var current = CurrentScene;

			if(current == null)
			{
				return UniTask.CompletedTask;
			}

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

			await _PlaySceneAsync(changeInfo,_CreateTaskAsync,_DestroyTaskAsync);
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
			_RemoveSceneAsync(changeInfo).Forget();
		}

		public UniTask RemoveSceneAsync(SceneChangeInfo changeInfo)
		{
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
				return;
			}

			m_isSceneChanging = true;

			KZInputKit.LockInput();

			var transitionNameTag = changeInfo.TransitionNameTag;

			// darker
			await UIManager.In.PlayTransitionOutAsync(transitionNameTag,false);

			if(changeInfo.UseLoading)
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

			KZInputKit.UnLockInput();

			m_isSceneChanging = false;
		}

		private async UniTask _CreateSceneAsync(string sceneName,Action<float> onUpdateProgress)
		{
			if(sceneName.IsEmpty())
			{
				LogChannel.Scene.E("Scene name is empty.");

				return;
			}

			onUpdateProgress?.Invoke(0.0f);

			LogChannel.Scene.I($"{sceneName} create start.");

			var sceneType = Type.GetType($"{sceneName}, Assembly-CSharp");

			if(sceneType == null)
			{
				LogChannel.Scene.E($"{sceneName} is not exists.");

				return;
			}

			if(Activator.CreateInstance(sceneType) is not SceneState sceneState)
			{
				LogChannel.Scene.E($"{sceneName} create failed.");

				return;
			}

			m_sceneStateStack.Push(sceneState);

			void _UpdateProgress(float progress)
			{
				_UpdateProgressCommon(onUpdateProgress,progress);
			}

			await sceneState.InitializeAsync(_UpdateProgress);

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
			KZMemoryKit.ClearUnloadedAssetMemory();

#if UNITY_EDITOR
			EditorUtility.UnloadUnusedAssetsImmediate(true);
#endif
			LogChannel.Scene.I($"{current.SceneName} destroy end.");

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

			KZMemoryKit.ClearUnloadedAssetMemory();
		}
		
		private void _OnUnloadSceneAssetBundle(Scene scene,LoadSceneMode mode)
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