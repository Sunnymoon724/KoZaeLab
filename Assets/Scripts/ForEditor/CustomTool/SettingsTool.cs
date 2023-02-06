#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;

//? AUTO_GENERATED CODE

public partial class SettingsTool : OdinMenuEditorWindow
{
	partial void AddOdinMenuTree(ref OdinMenuTree _tree)
	{
		_tree.Add("언어 설정",LanguageSettings.In);
		_tree.Add("메타 설정",MetaSettings.In);
		// _tree.Add("인증 설정",AuthSettings.In);
		_tree.Add("빌드 설정",BuildSettings.In);
	}
}
#endif