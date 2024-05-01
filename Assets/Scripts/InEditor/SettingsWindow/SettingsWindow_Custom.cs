#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;

//? AUTO_GENERATED CODE

namespace KZLib.KZWindow
{
	public partial class SettingsWindow : OdinMenuEditorWindow
	{
		partial void AddOdinMenuTree(ref OdinMenuTree _tree)
		{
			_tree.Add("통신 설정",NetworkSettings.In);
		}
	}
}
#endif