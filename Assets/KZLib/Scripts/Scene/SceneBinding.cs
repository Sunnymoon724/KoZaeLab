using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace KZLib
{
	/// <summary>
	/// 유니티 씬 + 기타 에셋들을 불러 초기화 하기 위한 용도
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

			while(handle.progress < 0.9f)
			{
				_onProgress?.Invoke(handle.progress*0.5f);

				await UniTask.Yield();
			}

			handle.allowSceneActivation = true;

			await handle;

			SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneName));

			await InitializeInnerAsync(progress =>
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
				throw new InvalidOperationException(string.Format("{0}이라는 이름의 싼은 없습니다.",_previous));
			}

			_onProgress?.Invoke(0.0f);

			SceneManager.SetActiveScene(scene);

			await ReleaseInnerAsync((progress)=>
			{
				_onProgress?.Invoke(progress*0.5f);
			});

			var handle = SceneManager.UnloadSceneAsync(SceneName);

			while(!handle.isDone)
			{
				_onProgress?.Invoke(0.5f+handle.progress*0.5f);

				await UniTask.Yield();
			}

			_onProgress?.Invoke(1.0f);
		}

		protected async virtual UniTask InitializeInnerAsync(Action<float> _onProgress,BindingParam _param) { await UniTask.Yield(); }
		protected async virtual UniTask ReleaseInnerAsync(Action<float> _onProgress) { await UniTask.Yield(); }

		public async virtual UniTask PlayAsync() { await UniTask.Yield(); }
	}
}