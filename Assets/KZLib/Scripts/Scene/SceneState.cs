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
		public string SceneName => GetType().Name;

		public async UniTask InitializeAsync(Action<float> onUpdateProgress)
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

			await operation.ToUniTask();

			await InitializeInnerAsync(progress =>
			{
				onUpdateProgress?.Invoke(0.5f+progress*0.5f);
			});

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
					LogSvc.System.E($"{previousSceneName} is not in the scene.");

					return;
				}

				SceneManager.SetActiveScene(previousScene);
			}

			onUpdateProgress?.Invoke(0.0f);

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

		protected async virtual UniTask InitializeInnerAsync(Action<float> onUpdateProgress) { await UniTask.Yield(); }
		protected async virtual UniTask ReleaseInnerAsync(Action<float> onUpdateProgress) { await UniTask.Yield(); }
	}
}