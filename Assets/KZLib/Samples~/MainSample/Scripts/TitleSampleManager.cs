using KZLib.KZUtility;

namespace KZLib.KZSample.Main
{
	public class TitleSampleManager : SingletonMB<TitleSampleManager>
	{
		private void Start()
		{
			LogSvc.Client.I("Start");
		}
	}
}