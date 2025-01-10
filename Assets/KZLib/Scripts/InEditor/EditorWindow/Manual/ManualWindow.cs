#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;

namespace KZLib.KZWindow
{
	public class ManualWindow : OdinMenuEditorWindow
	{
		protected override OdinMenuTree BuildMenuTree()
		{
			var menuTree = new OdinMenuTree
			{
				{ "Easing Graph",CreateInstance<EasingGraphWindow>() },
				{ "Mesh Finder",CreateInstance<MeshFinderWindow>() },
				{ "Pixel Editor",CreateInstance<PixelEditorWindow>() },
			};

			menuTree.Config.DrawSearchToolbar = true;
			menuTree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
			menuTree.Selection.SupportsMultiSelect = false;

			return menuTree;
		}
	}
}
#endif