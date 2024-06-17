#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace KZEditor
{
	public partial class KZShortCut
	{
		[Shortcut("Look Center",KeyCode.F,ShortcutModifiers.Alt)]
		private static void OnOpenEffectTestScene()
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