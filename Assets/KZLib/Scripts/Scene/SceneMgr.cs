using System;
using System.Collections.Generic;
using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using UnityEngine;
using Cysharp.Threading.Tasks;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	public class SceneMgr : AutoSingletonMB<SceneMgr>
	{
		private const float UNLOAD_MIN_TIME = 300.0f;

		private bool m_SceneChanging = false;
		public bool IsSceneChanging => m_SceneChanging;

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
		private Stack<SceneBinding> m_SceneStack = new();

		private float m_LastUnloadTime = 0.0f;

		protected override void Initialize()
		{
			Application.lowMemory += OnLowMemory;
		}

		protected override void Release()
		{
			Application.lowMemory -= OnLowMemory;

			CommonUtility.ClearUnloadedAssetMemory();
		}

		public void ReloadScene(TransitionData _data,bool _loading,SceneBinding.BindingParam _param = null)
		{
			var current = CurrentScene;

			if(current == null)
			{
				return;
			}

			ChangeScene(current.SceneName,_data,_loading,_param);
		}

		public void ChangeScene(string _sceneName,TransitionData _data,bool _loading,SceneBinding.BindingParam _param = null)
		{
			ChangeSceneAsync(_sceneName,_data,_loading,_param).Forget();
		}

		public async UniTask ChangeSceneAsync(string _sceneName,TransitionData _data,bool _loading,SceneBinding.BindingParam _param = null)
		{
			await PlaySceneAsync(_data,_loading,RemoveSceneAsync,async (progress)=> { await AddSceneAsync(_sceneName,progress,_param); });

			var current = CurrentScene;

			await current.PlayAsync();
		}

		public void AddScene(string _sceneName,TransitionData _data,bool _loading,SceneBinding.BindingParam _param = null)
		{
			AddSceneAsync(_sceneName,_data,_loading,_param).Forget();
		}

		public async UniTask AddSceneAsync(string _sceneName,TransitionData _data,bool _loading,SceneBinding.BindingParam _param = null)
		{
			await PlaySceneAsync(_data,_loading,async (progress)=> { await AddSceneAsync(_sceneName,progress,_param); });

			var current = CurrentScene;

			await current.PlayAsync();
		}

		public void RemoveCurrentScene(TransitionData _data,bool _loading)
		{
			RemoveCurrentSceneAsync(_data,_loading).Forget();
		}

		public async UniTask RemoveCurrentSceneAsync(TransitionData _data,bool _loading)
		{
			await PlaySceneAsync(_data,_loading,RemoveSceneAsync);
		}

		private async UniTask PlaySceneAsync(TransitionData _data,bool _loading,params Func<Action<float>,UniTask>[] _onTaskArray)
		{
			if(m_SceneChanging)
			{
				return;
			}

			m_SceneChanging = true;

			CommonUtility.LockInput();

			// darker
			await UIMgr.In.PlayTransitionOutAsync(_data,false);

			if(_loading)
			{
				var panel = UIMgr.In.Open<LoadingPanelUI>(UITag.LoadingPanelUI);

				// brighter
				await UIMgr.In.PlayTransitionInAsync(_data,false);

				var count = (float) _onTaskArray.Length;
				var percent = 0.0f;

				foreach(var task in _onTaskArray)
				{
					await task.Invoke((progress)=>
					{
						percent += progress/count;

						panel.SetLoadingProgress(percent);
					});
				}

				// darker
				await UIMgr.In.PlayTransitionOutAsync(_data,false);

				UIMgr.In.Close(UITag.LoadingPanelUI);
			}
			else
			{
				foreach(var task in _onTaskArray)
				{
					await task.Invoke(null);
				}
			}

			await UIMgr.In.PlayTransitionInAsync(_data,true);

			CommonUtility.UnLockInput();

			m_SceneChanging = false;
		}

		private async UniTask AddSceneAsync(string _sceneName,Action<float> _onProgress,SceneBinding.BindingParam _param)
		{
			if(_sceneName.IsEmpty())
			{
				LogTag.System.E("Scene name is empty.");

				return;
			}

			_onProgress?.Invoke(0.0f);

			LogTag.System.I($"{_sceneName} create start.");

			var sceneType = Type.GetType(_sceneName) ?? throw new NullReferenceException($"{_sceneName} is not exists.");
			var sceneBiding = (SceneBinding) Activator.CreateInstance(sceneType) ?? throw new NullReferenceException($"{_sceneName} create failed.");

			m_SceneStack.Push(sceneBiding);

			await sceneBiding.InitializeAsync((progress)=>
			{
				_onProgress?.Invoke(progress*0.99f);
			},_param);

			LogTag.System.I($"{_sceneName} create end.");

			_onProgress?.Invoke(1.0f);
		}

		private async UniTask RemoveSceneAsync(Action<float> _onProgress)
		{
			var current = CurrentScene;

			if(current == null)
			{
				return;
			}

			_onProgress?.Invoke(0.0f);

			var sceneName = current.SceneName;

			LogTag.System.I($"{sceneName} remove start.");

			m_SceneStack.Pop();

			var previous = CurrentScene;

			// Release current scene & Active on previous scene
			await current.ReleaseAsync(previous?.SceneName,(progress)=>
			{
				_onProgress?.Invoke(progress*0.99f);
			});

#if UNITY_EDITOR
			EditorUtility.UnloadUnusedAssetsImmediate(true);
#endif
			LogTag.System.I($"{sceneName} remove end.");

			CommonUtility.ClearUnloadedAssetMemory();

			_onProgress?.Invoke(1.0f);
		}

		private SceneBinding CurrentScene => m_SceneStack.IsNullOrEmpty() ? null : m_SceneStack.Peek();

		private void OnLowMemory()
		{
			if(Time.unscaledTime-m_LastUnloadTime < UNLOAD_MIN_TIME)
			{
				return;
			}

			m_LastUnloadTime = Time.unscaledTime;
			Resources.UnloadUnusedAssets();
		}
	}
}