using KZLib.Utilities;

namespace KZLib.Samples.Main
{
	[SingletonMBConfig(AutoCreate = false,DontDestroy = false)]
	public class TitleSampleManager : SingletonMB<TitleSampleManager>
	{
		private void Start()
		{
			LogChannel.Test.I("Start");
		}
	}
}