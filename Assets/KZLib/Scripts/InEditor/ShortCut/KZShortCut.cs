#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace KZLib.KZEditor
{
#pragma warning disable IDE0051
	public class KZShortCut
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
#pragma warning restore IDE0051
}
#endif