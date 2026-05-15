using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace KZLib
{
	/// <summary>
	/// Unity Scene + Initialize asset 
	/// </summary>
	public interface ISceneState
	{
		UniTask InitializeAsync(Action<float> onUpdateProgress);
		UniTask ReleaseAsync(string previousSceneName,Action<float> onUpdateProgress);

		string SceneName { get; }
	}

	public abstract class SceneState : ISceneState
	{
		private const float c_sceneLoadThreshold = 0.9f;
		private const float c_progressHalf = 0.5f;
		public string SceneName => GetType().Name;

		public async UniTask InitializeAsync(Action<float> onUpdateProgress)
		{
			onUpdateProgress?.Invoke(0.0f);

			var operation = SceneManager.LoadSceneAsync(SceneName,LoadSceneMode.Additive);

			operation.allowSceneActivation = false;

			while(operation.progress < c_sceneLoadThreshold)
			{
				onUpdateProgress?.Invoke(operation.progress*c_progressHalf);

				await UniTask.Yield();
			}

			onUpdateProgress?.Invoke(c_progressHalf);

			operation.allowSceneActivation = true;

			await operation.ToUniTask();

			void _UpdateProgress(float progress)
			{
				onUpdateProgress?.Invoke(c_progressHalf+progress*c_progressHalf);
			}

			await InitializeInnerAsync(_UpdateProgress);

			SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneName));

			onUpdateProgress?.Invoke(1.0f);
		}

		public async UniTask ReleaseAsync(string previousSceneName,Action<float> onUpdateProgress)
		{
			if(!previousSceneName.IsEmpty())
			{
				var previousScene = SceneManager.GetSceneByName(previousSceneName);

				if(!previousScene.IsValid())
				{
					LogChannel.Scene.E($"{previousSceneName} is not in the scene.");

					return;
				}

				SceneManager.SetActiveScene(previousScene);
			}

			onUpdateProgress?.Invoke(0.0f);

			void _UpdateProgress(float progress)
			{
				onUpdateProgress?.Invoke(progress*c_progressHalf);
			}

			await ReleaseInnerAsync(_UpdateProgress);

			var operation = SceneManager.UnloadSceneAsync(SceneName);

			while(!operation.isDone)
			{
				onUpdateProgress?.Invoke(c_progressHalf+operation.progress*c_progressHalf);

				await UniTask.Yield();
			}

			onUpdateProgress?.Invoke(1.0f);
		}

		protected async virtual UniTask InitializeInnerAsync(Action<float> onUpdateProgress) { await UniTask.Yield(); }
		protected async virtual UniTask ReleaseInnerAsync(Action<float> onUpdateProgress) { await UniTask.Yield(); }
	}
}