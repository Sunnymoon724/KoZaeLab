#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace KZLib.EditorInternal
{
	/// <summary>
	/// Registers Unity editor shortcuts via <see cref="ShortcutAttribute"/> for faster Scene View and editor workflows.
	/// </summary>
	public class KZShortCut
	{
		/// <summary>Aligns the active Scene View pivot to world origin (0, 0, 0). Shortcut: Alt+F.</summary>
		[Shortcut("Look Center",KeyCode.F,ShortcutModifiers.Alt)]
		protected static void OnLookCenter()
		{
			var view = SceneView.lastActiveSceneView;

			if(view != null)
			{
				view.LookAt(Vector3.zero);
			}
		}
	}
}
#endif