using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZAttribute;

#if UNITY_EDITOR

using UnityEditor;

#endif

public partial class GameSettings : InnerBaseSettings<GameSettings>
{
	[SerializeField,HideInInspector]
	private string m_DefaultSettingPath = Global.DEFAULT_SETTING_WINDOW_FOLDER_PATH;

	[TitleGroup("경로 설정",BoldTitle = false,Order = 1)]
	[BoxGroup("경로 설정/0",ShowLabel = false),ShowInInspector,LabelText("설정 창 경로"),PropertyTooltip("새로운 설정을 추가할 때의 기본 경로"),KZFolderPath]
	public string DefaultSettingPath { get => m_DefaultSettingPath; private set => m_DefaultSettingPath = value; }

	[SerializeField,HideInInspector]
	private string m_MetaScriptPath = Global.DEFAULT_METADATA_SCRIPT_FOLDER_PATH;
	
	[PropertySpace(5)]
	[BoxGroup("경로 설정/0",ShowLabel = false),ShowInInspector,LabelText("메타 데이터 스크립트 경로"),KZFolderPath]
	public string MetaScriptPath { get => m_MetaScriptPath; private set => m_MetaScriptPath = value; }

	[SerializeField,HideInInspector]
	private string m_MetaAssetPath = Global.DEFAULT_METADATA_ASSET_FOLDER_PATH;
	
	[PropertySpace(5)]
	[BoxGroup("경로 설정/0",ShowLabel = false),ShowInInspector,LabelText("메타 데이터 에셋 경로"),KZFolderPath]
	public string MetaAssetPath { get => m_MetaAssetPath; private set => m_MetaAssetPath = value; }

	[SerializeField,HideInInspector]
	private string m_LanguagePath = Global.DEFAULT_LANGUAGE_FOLDER_PATH;

	[BoxGroup("경로 설정/0",ShowLabel = false),ShowInInspector,LabelText("언어 데이터 에셋 경로"),KZFolderPath]
	public string LanguagePath { get => m_LanguagePath; private set => m_LanguagePath = value; }

	[SerializeField,HideInInspector]
	private string m_UIPrefabPath = Global.DEFAULT_UI_PREFAB_PATH;
	
	[PropertySpace(5)]
	[BoxGroup("경로 설정/0",ShowLabel = false),ShowInInspector,LabelText("UI 기본 경로"),KZFolderPath]
	public string UIPrefabPath { get => m_UIPrefabPath; private set => m_UIPrefabPath = value; }

	[SerializeField,HideInInspector]
	private string m_EffectPrefabPath = Global.DEFAULT_EFFECT_PREFAB_PATH;
	
	[BoxGroup("경로 설정/0",ShowLabel = false),ShowInInspector,LabelText("이펙트 기본 경로"),KZFolderPath]
	public string EffectPrefabPath { get => m_EffectPrefabPath; private set => m_EffectPrefabPath = value; }

	private void InitializePath()
	{
#if UNITY_EDITOR
		CommonUtility.CreateFolder(Global.DEFAULT_SETTING_WINDOW_FOLDER_PATH);

		AssetDatabase.Refresh();
#endif
	}
}