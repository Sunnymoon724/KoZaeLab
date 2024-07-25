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
		private bool m_SceneChanging = false;
		public bool IsSceneChanging => m_SceneChanging;

		[HorizontalGroup("0",Order = 0),ShowInInspector,KZRichText,LabelText("현재 씬 이름")]
		protected string CurrentSceneName
		{
			get
			{
				var current = GetCurrentScene();

				return current == null ? "씬이 없습니다." : current.SceneName;
			}
		}

		[HorizontalGroup("1",Order = 1),SerializeField,DisplayAsString,LabelText("씬 스택"),ListDrawerSettings(ShowFoldout = false,IsReadOnly = true)]
		private Stack<SceneBinding> m_SceneStack = new();

		protected override void Release()
		{
			GameUtility.ClearUnloadedAssetMemory();
		}

		public void ReloadScene(TransitionData _data,bool _loading,SceneBinding.BindingParam _param = null)
		{
			var current = GetCurrentScene();

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

			var current = GetCurrentScene();

			await current.PlayAsync();
		}

		public void AddScene(string _sceneName,TransitionData _data,bool _loading,SceneBinding.BindingParam _param = null)
		{
			AddSceneAsync(_sceneName,_data,_loading,_param).Forget();
		}

		public async UniTask AddSceneAsync(string _sceneName,TransitionData _data,bool _loading,SceneBinding.BindingParam _param = null)
		{
			await PlaySceneAsync(_data,_loading,async (progress)=> { await AddSceneAsync(_sceneName,progress,_param); });

			var current = GetCurrentScene();

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

			GameUtility.LockInput();

			await UIMgr.In.PlayTransitionOutInAsync(_data,async ()=>
			{
				if(_loading)
				{
					var panel = UIMgr.In.Open<LoadingPanelUI>(UITag.LoadingPanelUI);

					// 점점 밝아짐
					await UIMgr.In.PlayTransitionInAsync(_data,true);

					var count = (float)_onTaskArray.Length;
					var percent = 0.0f;

					foreach(var task in _onTaskArray)
					{
						await task.Invoke((progress)=>
						{
							percent += progress/count;

							panel.SetLoadingProgress(percent);
						});
					}

					// 점점 어두워짐
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
			});

			GameUtility.UnLockInput();

			m_SceneChanging = false;
		}

		private async UniTask AddSceneAsync(string _sceneName,Action<float> _onProgress,SceneBinding.BindingParam _param)
		{
			if(_sceneName.IsEmpty())
			{
				throw new NullReferenceException("씬이 없습니다.");
			}

			_onProgress?.Invoke(0.0f);

			LogTag.Scene.I("{0} 생성을 시작합니다.",_sceneName);

			var sceneType = Type.GetType(_sceneName) ?? throw new NullReferenceException(string.Format("{0}이 존재하지 않습니다.",_sceneName));
			var sceneBiding = (SceneBinding) Activator.CreateInstance(sceneType) ?? throw new NullReferenceException(string.Format("{0} 생성이 실패했습니다.",_sceneName));

			m_SceneStack.Push(sceneBiding);

			await sceneBiding.InitializeAsync((progress)=>
			{
				_onProgress?.Invoke(progress*0.99f);
			},_param);

			LogTag.Scene.I("{0} 생성이 끝났습니다.",_sceneName);

			_onProgress?.Invoke(1.0f);
		}

		private async UniTask RemoveSceneAsync(Action<float> _onProgress)
		{
			var current = GetCurrentScene();

			if(current == null)
			{
				return;
			}

			_onProgress?.Invoke(0.0f);

			var sceneName = current.SceneName;

			LogTag.Scene.I("{0} 제거를 시작합니다.",sceneName);

			m_SceneStack.Pop();

			var previous = GetCurrentScene();

			// 현재 씬을 지우고 이전 씬의 액티브를 킨다.
			await current.ReleaseAsync(previous?.SceneName,(progress)=>
			{
				_onProgress?.Invoke(progress*0.99f);
			});

#if UNITY_EDITOR
			EditorUtility.UnloadUnusedAssetsImmediate(true);
#endif
			LogTag.Scene.I("{0} 제거가 끝났습니다.",sceneName);

			GameUtility.ClearUnloadedAssetMemory();

			_onProgress?.Invoke(1.0f);
		}

		private SceneBinding GetCurrentScene()
		{
			if(m_SceneStack.IsNullOrEmpty())
			{
				return null;
			}

			return m_SceneStack.Peek();
		}
	}
}