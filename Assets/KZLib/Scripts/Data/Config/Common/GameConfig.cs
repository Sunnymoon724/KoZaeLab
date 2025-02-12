using KZLib.KZData;

namespace ConfigData
{
	public class GameConfig : IConfig
	{
		public string ProtoFolderPath { get; private set; } = "Resources/Text/Proto";
		public string LanguageFolderPath { get; private set; } = "Resources/Text/Language";
		public string ConfigFolderPath { get; private set; } = "Resources/Text/Config";
		public string UIPrefabPath { get; private set; } = "Resources/Prefab/UI";
		public string FXPrefabPath { get; private set; } = "Resources/Prefab/FX";

		public bool IsLocalResource { get; private set; } = true;
		public bool IsLocalSave { get; private set; } = true;

		public bool UseHeadUpDisplay { get; private set; } = true;
	}
}