using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace KZLib
{
	/// <summary>
	/// Unity Scene + Initialize asset 
	/// </summary>
	public interface ISceneState<TParam>
	{
		UniTask InitializeAsync(Action<float> onUpdateProgress,TParam param);
		UniTask ReleaseAsync(string previousSceneName,Action<float> onUpdateProgress);

		string SceneName { get; }
	}

	public abstract class SceneState : ISceneState<SceneState.StateParam>
	{
		public record StateParam();

		private const string c_mainScene = "MainScene";

		public string SceneName => GetType().Name;

		public async UniTask InitializeAsync(Action<float> onUpdateProgress,StateParam param)
		{
			onUpdateProgress?.Invoke(0.0f);

			var operation = SceneManager.LoadSceneAsync(SceneName,LoadSceneMode.Additive);
			operation.allowSceneActivation = false;

			while(operation.progress < 0.9f)
			{
				onUpdateProgress?.Invoke(operation.progress*0.5f);

				await UniTask.Yield();
			}

			operation.allowSceneActivation = true;

			await operation;

			SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneName));

			await InitializeInnerAsync(progress =>
			{
				onUpdateProgress?.Invoke(0.5f+progress*0.5f);
			},param);

			onUpdateProgress?.Invoke(1.0f);
		}

		public async UniTask ReleaseAsync(string previousSceneName,Action<float> onUpdateProgress)
		{
			var scene = SceneManager.GetSceneByName(previousSceneName.IsEmpty() ? c_mainScene : previousSceneName);

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

			var operation = SceneManager.UnloadSceneAsync(SceneName);

			while(!operation.isDone)
			{
				onUpdateProgress?.Invoke(0.5f+operation.progress*0.5f);

				await UniTask.Yield();
			}

			onUpdateProgress?.Invoke(1.0f);
		}

		protected async virtual UniTask InitializeInnerAsync(Action<float> onUpdateProgress,StateParam param) { await UniTask.Yield(); }
		protected async virtual UniTask ReleaseInnerAsync(Action<float> onUpdateProgress) { await UniTask.Yield(); }
	}
}