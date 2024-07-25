
public partial class GameSettings : InnerBaseSettings<GameSettings>
{
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