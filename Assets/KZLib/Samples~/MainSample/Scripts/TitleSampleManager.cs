using KZLib.KZUtility;

namespace KZLib.KZSample.Main
{
	[SingletonConfig(AutoCreate = false,DontDestroy = false)]
	public class TitleSampleManager : SingletonMB<TitleSampleManager>
	{
		private void Start()
		{
			LogSvc.Client.I("Start");
		}
	}
}