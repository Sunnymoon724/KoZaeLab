#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace KZLib.EditorInternal.Menus
{
	/// <summary>
	/// <c>GameObject</c> hierarchy menu entries.
	/// </summary>
	public static partial class DefaultMenuItem
	{
		#region Copy Hierarchy
		[MenuItem("GameObject/Copy Hierarchy",false,MenuOrder.Hierarchy.COPY)]
		private static void _OnCopyHierarchy()
		{
			var selected = Selection.activeGameObject;

			if(!selected)
			{
				return;
			}

			KZPlatformKit.CopyToClipBoard(selected.transform.BuildHierarchyPath());
		}

		[MenuItem("GameObject/Copy Hierarchy",true,MenuOrder.Hierarchy.COPY)]
		private static bool _CanCopyHierarchy()
		{
			return Selection.gameObjects.Length == 1;
		}
		#endregion Copy Hierarchy

		#region Category Line
		[MenuItem("GameObject/Create Category Line",false,MenuOrder.Hierarchy.CATEGORY_LINE)]
		private static void _OnCreateCategoryLine()
		{
			Undo.IncrementCurrentGroup();

			var group = Undo.GetCurrentGroup();
			var line = new GameObject("##");

			Undo.RegisterCreatedObjectUndo(line,"Create Category Line");

			var selected = Selection.activeGameObject;

			if(selected)
			{
				var root = selected.transform.root;
				var siblingIndex = root.GetSiblingIndex();

				line.transform.SetSiblingIndex(siblingIndex+1);
			}

			Undo.CollapseUndoOperations(group);
		}

		[MenuItem("GameObject/Create Category Line",true,MenuOrder.Hierarchy.CATEGORY_LINE)]
		private static bool _CanCreateCategoryLine()
		{
			return !EditorApplication.isPlayingOrWillChangePlaymode;
		}
		#endregion Category Line
	}
}
#endif