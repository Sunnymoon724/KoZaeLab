#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;

namespace KZLib.KZWindow
{
	public class ManualWindow : OdinMenuEditorWindow
	{
		protected override OdinMenuTree BuildMenuTree()
		{
			var tree = new OdinMenuTree
			{
				{ "Easing",CreateInstance<EasingWindow>() },
				{ "Mesh Find",CreateInstance<MeshFindWindow>() },
				{ "Sprite Edit",CreateInstance<SpriteEditWindow>() },
				{ "Preset Edit",CreateInstance<PresetEditWindow>() },
			};

			tree.Config.DrawSearchToolbar = true;
			tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
			tree.Selection.SupportsMultiSelect = false;

			return tree;
		}
	}
}
#endif