using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZAttribute;

#if UNITY_EDITOR

using UnityEditor;

#endif

public partial class GameSettings : InnerBaseSettings<GameSettings>
{
	private const string SETTING_WINDOW_FOLDER_PATH = "Scripts/InEditor/SettingsWindow";

	[SerializeField,HideInInspector]
	private string m_DefaultSettingPath = SETTING_WINDOW_FOLDER_PATH;

	[TitleGroup("경로 설정",BoldTitle = false,Order = 1)]
	[BoxGroup("경로 설정/0",ShowLabel = false),ShowInInspector,LabelText("설정 창 경로"),PropertyTooltip("새로운 설정을 추가할 때의 기본 경로"),KZFolderPath]
	public string DefaultSettingPath { get => m_DefaultSettingPath; private set => m_DefaultSettingPath = value; }

	[SerializeField,HideInInspector]
	private string m_MetaScriptPath = "Scripts/Data/MetaData";
	
	[PropertySpace(5)]
	[BoxGroup("경로 설정/0",ShowLabel = false),ShowInInspector,LabelText("메타 데이터 스크립트 경로"),KZFolderPath]
	public string MetaScriptPath { get => m_MetaScriptPath; private set => m_MetaScriptPath = value; }

	[SerializeField,HideInInspector]
	private string m_MetaAssetPath = "Resources/ScriptableObjects/MetaData";
	
	[PropertySpace(5)]
	[BoxGroup("경로 설정/0",ShowLabel = false),ShowInInspector,LabelText("메타 데이터 에셋 경로"),KZFolderPath]
	public string MetaAssetPath { get => m_MetaAssetPath; private set => m_MetaAssetPath = value; }

	[SerializeField,HideInInspector]
	private string m_LanguagePath = "Resources/Texts/Languages";

	[BoxGroup("경로 설정/0",ShowLabel = false),ShowInInspector,LabelText("언어 데이터 에셋 경로"),KZFolderPath]
	public string LanguagePath { get => m_LanguagePath; private set => m_LanguagePath = value; }

	[SerializeField,HideInInspector]
	private string m_UIPrefabPath = "Resources/Prefabs/UI";
	
	[PropertySpace(5)]
	[BoxGroup("경로 설정/0",ShowLabel = false),ShowInInspector,LabelText("UI 기본 경로"),KZFolderPath]
	public string UIPrefabPath { get => m_UIPrefabPath; private set => m_UIPrefabPath = value; }

	[SerializeField,HideInInspector]
	private string m_EffectPrefabPath = "Resources/Prefabs/Efx";
	
	[BoxGroup("경로 설정/0",ShowLabel = false),ShowInInspector,LabelText("이펙트 기본 경로"),KZFolderPath]
	public string EffectPrefabPath { get => m_EffectPrefabPath; private set => m_EffectPrefabPath = value; }

	private void InitializePath()
	{
#if UNITY_EDITOR
		FileUtility.CreateFolder(FileUtility.GetAbsolutePath(SETTING_WINDOW_FOLDER_PATH,true));

		AssetDatabase.Refresh();
#endif
	}
}