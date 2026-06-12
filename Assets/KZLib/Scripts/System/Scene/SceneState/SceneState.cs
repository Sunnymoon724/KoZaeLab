using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace KZLib
{
	/// <summary>
	/// Unity scene load/unload and scene content setup/teardown.
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

			var isSceneLoaded = false;

			try
			{
				var loadedScene = await _LoadSceneAsync(onUpdateProgress);

				isSceneLoaded = true;

				onUpdateProgress?.Invoke(c_progressHalf);

				void _UpdateProgress(float progress)
				{
					onUpdateProgress?.Invoke(c_progressHalf+progress*c_progressHalf);
				}

				await SetupAsync(_UpdateProgress);

				SceneManager.SetActiveScene(loadedScene);

				onUpdateProgress?.Invoke(1.0f);
			}
			catch
			{
				if(isSceneLoaded)
				{
					await _RevertInitializeAsync();
				}

				throw;
			}
		}

		private async UniTask _RevertInitializeAsync()
		{
			try
			{
				await TeardownAsync(null);
			}
			catch(Exception exception)
			{
				LogChannel.Scene.W($"Teardown failed during initialize revert: {exception.Message}");
			}

			await _UnloadSceneAsync(SceneName,null,0.0f);
		}

		private async UniTask<Scene> _LoadSceneAsync(Action<float> onUpdateProgress)
		{
			var operation = SceneManager.LoadSceneAsync(SceneName,LoadSceneMode.Additive) ?? throw new InvalidOperationException($"{SceneName} load operation is null. Scene must be added to build settings.");
			operation.allowSceneActivation = false;

			while(operation.progress < c_sceneLoadThreshold)
			{
				onUpdateProgress?.Invoke(operation.progress*c_progressHalf);

				await UniTask.Yield();
			}

			operation.allowSceneActivation = true;

			await operation.ToUniTask();

			var loadedScene = SceneManager.GetSceneByName(SceneName);

			if(!loadedScene.IsValid())
			{
				throw new InvalidOperationException($"{SceneName} scene is not valid after load.");
			}

			return loadedScene;
		}

		public async UniTask ReleaseAsync(string previousSceneName,Action<float> onUpdateProgress)
		{
			if(!previousSceneName.IsEmpty())
			{
				var previousScene = SceneManager.GetSceneByName(previousSceneName);

				if(previousScene.IsValid())
				{
					SceneManager.SetActiveScene(previousScene);
				}
				else
				{
					LogChannel.Scene.W($"{previousSceneName} is not in the scene.");
				}
			}

			onUpdateProgress?.Invoke(0.0f);

			void _UpdateProgress(float progress)
			{
				onUpdateProgress?.Invoke(progress*c_progressHalf);
			}

			await TeardownAsync(_UpdateProgress);

			await _UnloadSceneAsync(SceneName,onUpdateProgress,c_progressHalf);

			onUpdateProgress?.Invoke(1.0f);
		}

		private static async UniTask _UnloadSceneAsync(string sceneName,Action<float> onUpdateProgress,float progressStart)
		{
			var operation = SceneManager.UnloadSceneAsync(sceneName) ?? throw new InvalidOperationException($"{sceneName} unload operation is null.");

			while(!operation.isDone)
			{
				onUpdateProgress?.Invoke(progressStart+operation.progress*c_progressHalf);

				await UniTask.Yield();
			}
		}

		protected async virtual UniTask SetupAsync(Action<float> onUpdateProgress) { await UniTask.Yield(); }
		protected async virtual UniTask TeardownAsync(Action<float> onUpdateProgress) { await UniTask.Yield(); }
	}
}
