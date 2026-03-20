using KZLib.Utilities;

namespace KZLib.Samples.Main
{
	[SingletonConfig(AutoCreate = false,DontDestroy = false)]
	public class TitleSampleManager : SingletonMB<TitleSampleManager>
	{
		private void Start()
		{
			LogChannel.Test.I("Start");
		}
	}
}