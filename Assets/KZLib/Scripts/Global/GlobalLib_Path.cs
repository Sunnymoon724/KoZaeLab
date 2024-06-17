
public partial struct Global
{
	//! 데이터 경로 -> 전부 Assets 이후
	public const string DEFAULT_SETTING_WINDOW_FOLDER_PATH 		= "Scripts/InEditor/SettingsWindow";

	public const string DEFAULT_LANGUAGE_TRANSLATE_FOLDER_PATH	= "Resources/Texts/Translations";

	public const string DEFAULT_METADATA_SCRIPT_FOLDER_PATH		= "Scripts/Data/MetaData";
	public const string DEFAULT_METADATA_ASSET_FOLDER_PATH		= "Resources/ScriptableObjects/MetaData";

	public const string IN_SIDE_SO_PATH    						= "Resources/ScriptableObjects/{0}";
	public const string OUT_SIDE_SO_PATH						= "WorkResources/ScriptableObjects/{0}";

	public const string DEFAULT_EFFECT_PREFAB_PATH				= "Resources/Prefabs/Efx";
	public const string DEFAULT_UI_PREFAB_PATH					= "Resources/Prefabs/UI";

	public static readonly string IN_SIDE_NETWORK_PATH			= string.Format(IN_SIDE_SO_PATH,"Network/{0}");
	public static readonly string OUT_SIDE_NETWORK_PATH			= string.Format(OUT_SIDE_SO_PATH,"Network/{0}");
}