#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;

//? DO NOT EDIT IT.

public partial class SettingsEditorWindow : OdinMenuEditorWindow
{
	partial void AddOdinMenuTree(ref OdinMenuTree _tree)
	{
		_tree.Add("언어 설정",LocalizationSettings.In);
		_tree.Add("시트 설정",SheetSettings.In);
		_tree.Add("리포트 설정",ReportSettings.In);
	}
}
#endif