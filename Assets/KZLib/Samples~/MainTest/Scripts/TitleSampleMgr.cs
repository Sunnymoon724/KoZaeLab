using KZLib.KZUtility;

namespace KZLib.KZSample
{
	public class TitleSampleMgr : SingletonMB<TitleSampleMgr>
	{
		private void Start()
		{
			LogSvc.Client.I("Start");
		}
	}
}