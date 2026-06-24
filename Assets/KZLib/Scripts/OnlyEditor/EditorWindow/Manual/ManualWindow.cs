#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;

namespace KZLib.Windows
{
	/// <summary>
	/// Odin menu shell that hosts manual editor utilities such as easing preview and mesh tools.
	/// </summary>
	public class ManualWindow : OdinMenuEditorWindow
	{
		/// <summary>
		/// Builds the left-hand tool menu and binds each entry to an embedded Odin editor window instance.
		/// </summary>
		protected override OdinMenuTree BuildMenuTree()
		{
			var menuTree = new OdinMenuTree
			{
				{ "Easing Graph", CreateInstance<EasingGraphWindow>() },
				{ "Mesh Finder", CreateInstance<MeshFinderWindow>() },
				{ "Pixel Editor", CreateInstance<PixelEditorWindow>() },
			};

			menuTree.Config.DrawSearchToolbar = true;
			menuTree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
			menuTree.Selection.SupportsMultiSelect = false;

			return menuTree;
		}
	}
}
#endif
