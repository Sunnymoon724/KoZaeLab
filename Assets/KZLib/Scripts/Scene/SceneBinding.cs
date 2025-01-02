using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace KZLib
{
	/// <summary>
	/// Unity Scene + Initialize asset 
	/// </summary>
	public interface ISceneBinding<TParam>
	{
		UniTask InitializeAsync(Action<float> onUpdateProgress,TParam param);
		UniTask ReleaseAsync(string previousSceneName,Action<float> onUpdateProgress);

		string SceneName { get; }

		UniTask PlayAsync();
	}

	public abstract class SceneBinding : ISceneBinding<SceneBinding.BindingParam>
	{
		public record BindingParam();

		private const string MAIN_SCENE = "MainScene";

		public abstract string SceneName { get; }

		public async UniTask InitializeAsync(Action<float> onUpdateProgress,BindingParam bindingParam)
		{
			onUpdateProgress?.Invoke(0.0f);

			var handle = SceneManager.LoadSceneAsync(SceneName,LoadSceneMode.Additive);
			handle.allowSceneActivation = false;

			while(handle.progress < 0.9f)
			{
				onUpdateProgress?.Invoke(handle.progress*0.5f);

				await UniTask.Yield();
			}

			handle.allowSceneActivation = true;

			await handle;

			SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneName));

			await InitializeInnerAsync(progress =>
			{
				onUpdateProgress?.Invoke(0.5f+progress*0.5f);
			},bindingParam);

			onUpdateProgress?.Invoke(1.0f);
		}

		public async UniTask ReleaseAsync(string previousSceneName,Action<float> onUpdateProgress)
		{
			var scene = SceneManager.GetSceneByName(previousSceneName.IsEmpty() ? MAIN_SCENE : previousSceneName);

			if(!scene.IsValid())
			{
				LogTag.System.E($"{previousSceneName} is not in the scene.");

				return;
			}

			onUpdateProgress?.Invoke(0.0f);

			SceneManager.SetActiveScene(scene);

			await ReleaseInnerAsync((progress)=>
			{
				onUpdateProgress?.Invoke(progress*0.5f);
			});

			var handle = SceneManager.UnloadSceneAsync(SceneName);

			while(!handle.isDone)
			{
				onUpdateProgress?.Invoke(0.5f+handle.progress*0.5f);

				await UniTask.Yield();
			}

			onUpdateProgress?.Invoke(1.0f);
		}

		protected async virtual UniTask InitializeInnerAsync(Action<float> onUpdateProgress,BindingParam bindingParam) { await UniTask.Yield(); }
		protected async virtual UniTask ReleaseInnerAsync(Action<float> onUpdateProgress) { await UniTask.Yield(); }

		public async virtual UniTask PlayAsync() { await UniTask.Yield(); }
	}
}