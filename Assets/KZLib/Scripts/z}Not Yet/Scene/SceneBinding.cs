using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace KZLib
{
	/// <summary>
	/// 유니티 씬을 불러와야 하는 경우면 IScene안에서 부르고 거기서 처리하도록 한다.
	/// </summary>
	public interface ISceneBinding<TParam>
	{
		UniTask InitializeAsync(Action<float> _onProgress,TParam _param);
		UniTask ReleaseAsync(string _previous,Action<float> _onProgress);

		string SceneName { get; }

		UniTask PlayAsync();
	}

	public abstract class SceneBinding : ISceneBinding<SceneBinding.BindingParam>
	{
		public record BindingParam();

		private const string MAIN_SCENE = "MainScene";

		public abstract string SceneName { get; }

		public async UniTask InitializeAsync(Action<float> _onProgress,BindingParam _param)
		{
			_onProgress?.Invoke(0.0f);

			var handle = SceneManager.LoadSceneAsync(SceneName,LoadSceneMode.Additive);
			handle.allowSceneActivation = false;

			while(!handle.isDone)
			{
				_onProgress?.Invoke(handle.progress*0.5f);

				// 로딩이 완료되었지만 씬이 아직 활성화되지 않은 상태
				if(handle.progress >= 0.9f)
				{
					await UniTask.WaitForSeconds(0.1f,true);

					handle.allowSceneActivation = true;
				}

				await UniTask.Yield();
			}

			SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneName));

			await InitializeInnerAsync((progress)=>
			{
				_onProgress?.Invoke(0.5f+progress*0.5f);
			},_param);

			_onProgress?.Invoke(1.0f);
		}

		public async UniTask ReleaseAsync(string _previous,Action<float> _onProgress)
		{
			var scene = SceneManager.GetSceneByName(_previous.IsEmpty() ? MAIN_SCENE : _previous);

			if(!scene.IsValid())
			{
				throw new NullReferenceException("씬이 없습니다.");
			}

			_onProgress?.Invoke(0.0f);

			SceneManager.SetActiveScene(scene);

			var handle = SceneManager.UnloadSceneAsync(SceneName);

			while(!handle.isDone)
			{
				_onProgress?.Invoke(handle.progress*0.5f);

				await UniTask.Yield();
			}

			await ReleaseInnerAsync((progress)=>
			{
				_onProgress?.Invoke(0.5f+progress*0.5f);
			});

			_onProgress?.Invoke(1.0f);
		}

		protected async virtual UniTask InitializeInnerAsync(Action<float> _onProgress,BindingParam _param) { await UniTask.Yield(); }
		protected async virtual UniTask ReleaseInnerAsync(Action<float> _onProgress) { await UniTask.Yield(); }

		public async virtual UniTask PlayAsync() { await UniTask.Yield(); }
	}
}