#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;

//? DO NOT EDIT IT.

public partial class SettingEditorWindow : OdinMenuEditorWindow
{
	partial void AddOdinMenuTree(ref OdinMenuTree _tree)
	{
		_tree.Add("언어 세팅",LocalizationSetting.In);
		_tree.Add("시트 세팅",SheetSetting.In);
		_tree.Add("리포트 설정",ReportSetting.In);
	}
}
#endif