
public partial class GameSettings : InSideSingletonSO<GameSettings>
{
	//? 일반 설정
	private const int GENERAL_ORDER = 0;
	//? 경로 설정
	private const int PATH_ORDER = 1;
	//? 그래픽 설정
	private const int GRAPHIC_ORDER = 2;
	//? HUD 설정
	private const int HUD_ORDER = 3;
	//? 프리셋 설정
	private const int PRESET_ORDER = 4;

#if UNITY_EDITOR
	protected override void Initialize()
	{
		base.Initialize();

		InitializeGeneral();

		InitializePath();

		InitializeGraphic();

		Initialize_Partial();
	}

	partial void Initialize_Partial();
#endif
}