using System;
using System.Collections.Generic;

namespace KZLib
{
	/// <summary>
	/// SceneState factory registry. Game project registers scene types before SceneStateManager creates them.
	/// </summary>
	public static class SceneStateRegistry
	{
		private static readonly Dictionary<string,Func<SceneState>> s_factoryDict = new();

		public static void Register<TScene>() where TScene : SceneState,new()
		{
			var sceneName = typeof(TScene).Name;

			static TScene _CreateScene()
			{
				return new TScene();
			}

			Register(sceneName,_CreateScene);
		}

		public static void Register(string sceneName,Func<SceneState> factory)
		{
			if(sceneName.IsEmpty())
			{
				throw new ArgumentException("Scene name is empty.");
			}

			if(factory == null)
			{
				throw new ArgumentNullException(nameof(factory));
			}

			if(s_factoryDict.ContainsKey(sceneName))
			{
				throw new InvalidOperationException($"{sceneName} is already registered.");
			}

			s_factoryDict.Add(sceneName,factory);
		}

		public static SceneState Create(string sceneName)
		{
			if(sceneName.IsEmpty())
			{
				throw new ArgumentException("Scene name is empty.");
			}

			if(!s_factoryDict.TryGetValue(sceneName,out var factory))
			{
				throw new InvalidOperationException($"{sceneName} is not registered.");
			}

			var sceneState = factory.Invoke();

			if(sceneState == null)
			{
				throw new InvalidOperationException($"{sceneName} factory returned null.");
			}

			return sceneState;
		}

		public static bool TryCreate(string sceneName,out SceneState sceneState)
		{
			sceneState = null;

			if(sceneName.IsEmpty() || !s_factoryDict.TryGetValue(sceneName,out var factory))
			{
				return false;
			}

			sceneState = factory.Invoke();

			return sceneState != null;
		}
	}
}
