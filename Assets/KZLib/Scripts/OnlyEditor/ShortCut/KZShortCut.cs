#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace KZLib.EditorInternal
{
	public class KZShortCut
	{
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