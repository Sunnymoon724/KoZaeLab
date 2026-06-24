using KZLib.Utilities;

namespace KZLib.Scenes
{
	public interface ISceneTransient
	{
		string TargetSceneName { get; }
	}

	public abstract class SceneTransientStore<TTransient> : TransientStore<TTransient> where TTransient : class,ISceneTransient
	{
		public static TTransient ConsumeValid(string sceneName)
		{
			var data = Consume();

			if(data == null)
			{
				LogChannel.Data.W("Transient data does not exist.");

				return null;
			}

			if(data.TargetSceneName == sceneName)
			{
				return data;
			}

			LogChannel.Data.W($"Transient target scene ({data.TargetSceneName}) does not match the current scene ({sceneName}).");

			return null;
		}
	}
}