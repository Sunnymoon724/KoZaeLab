#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;

namespace KZLib.KZWindow
{
	public partial class ManualWindow : OdinMenuEditorWindow
	{
		protected override OdinMenuTree BuildMenuTree()
		{
			var tree = new OdinMenuTree
			{
				{ "메쉬 파인더",CreateInstance<MeshFindWindow>() },
				{ "스프라이트 에디터",CreateInstance<SpriteEditWindow>() },
				{ "프리셋 에디터",CreateInstance<PresetEditWindow>() },
			};

			tree.Config.DrawSearchToolbar = true;
			tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
			tree.Selection.SupportsMultiSelect = false;

			return tree;
		}
	}
}
#endif