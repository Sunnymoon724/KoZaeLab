using KZLib.Utilities;

namespace KZLib.Data
{
	public interface ISceneTransient
	{
		string TargetSceneName { get; }
	}

	public abstract class SceneTransientStore<TTransient> : TransientStore<TTransient> where TTransient : class,ISceneTransient
	{
		public static TTransient ConsumeValidData(string sceneName)
		{
			var data = Consume();

			if(data == null)
			{
				LogChannel.Data.W("data is not exist.");

				return null;
			}

			if(data.TargetSceneName == sceneName)
			{
				return data;
			}

			LogChannel.Data.W($"data's target scene name({data.TargetSceneName}) is not match with current scene name({sceneName}).");

			return null;
		}
	}
}