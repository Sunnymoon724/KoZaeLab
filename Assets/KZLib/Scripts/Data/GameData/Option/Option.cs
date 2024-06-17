using KZLib;

public record ScreenResolutionData(int Width,int Height,bool IsFull);
public record SoundVolumeData(float Level,bool Mute);

namespace GameData
{
	/// <summary>
	/// 디폴트 옵션은 게임세팅에서 가져온다.
	/// 여기 저장되는건 유저 옵션들
	/// </summary>
	public partial class Option : IGameData
	{
		private class Handler : SaveDataHandler
		{
			protected override string TABLE_NAME => "Option_Table";

			protected override bool NewSave => true;
		}

		private static Handler s_SaveHandler = null;

		public void Initialize()
		{
			s_SaveHandler = new();

			InitializeSound();
			InitializeGraphic();
			InitializeLanguage();
			InitializeNative();

			Initialize_Partial();
		}

		public void Release()
		{
			ReleaseSound();
			ReleaseGraphic();
			ReleaseLanguage();
			ReleaseNative();

			Release_Partial();
		}

		partial void Initialize_Partial();
		partial void Release_Partial();
	}
}