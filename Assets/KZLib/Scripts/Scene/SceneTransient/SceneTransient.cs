using KZLib.Utilities;

namespace KZLib.Data
{
	public interface ISceneTransient
	{
		string TargetSceneName { get; }
	}

	public abstract class SceneTransientStore : TransientStore<ISceneTransient>
	{
		public static TTransient ConsumeValidData<TTransient>(string sceneName) where TTransient : class,ISceneTransient
		{
			var data = Consume();

			if(data == null)
			{
				LogChannel.Data.W("data is not exist.");

				return null;
			}

			if(data.TargetSceneName == sceneName)
			{
				return data as TTransient;
			}

			LogChannel.Data.W($"data's target scene name({data.TargetSceneName}) is not match with current scene name({sceneName}).");

			return null;
		}
	}
}