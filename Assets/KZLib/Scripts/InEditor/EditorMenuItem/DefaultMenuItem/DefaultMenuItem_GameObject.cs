#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using KZLib.KZDevelop;

namespace KZLib.KZMenu
{
	public static partial class DefaultMenuItem
	{
		#region UI
		#region Empty Panel
		[MenuItem("GameObject/UI/Empty Panel",false,MenuOrder.GameObject.UI)]
		private static void _OnCreateEmptyPanel()
		{
			Undo.IncrementCurrentGroup();

			var group = Undo.GetCurrentGroup();
			var panel = _CreatePanel("EmptyPanel",true);

			Undo.RegisterCreatedObjectUndo(panel,"Create EmptyPanel");

			_LinkCanvas(panel);

			Undo.CollapseUndoOperations(group);
		}
		#endregion Empty Panel

		#region UIShape
		[MenuItem("GameObject/UI/Shape",false,MenuOrder.GameObject.UI)]
		private static void _OnCreateShape()
		{
			Undo.IncrementCurrentGroup();

			var group = Undo.GetCurrentGroup();
			var shape = _CreatePanel("Shape");

			shape.AddComponent<UIShape>();

			Undo.RegisterCreatedObjectUndo(shape,"Create Shape");

			_LinkCanvas(shape);

			Undo.CollapseUndoOperations(group);
		}
		#endregion UIShape

		#region Focus Scroller
		[MenuItem("GameObject/UI/Focus Scroller",false,MenuOrder.GameObject.UI)]
		private static void _OnCreateFocusScroller()
		{
			Undo.IncrementCurrentGroup();

			var group = Undo.GetCurrentGroup();
			var scroller = _CreatePanel("Scroller",false);

			Undo.RegisterCreatedObjectUndo(scroller,"Create Scroller");

			scroller.AddComponent<Image>();

			var viewport = _CreatePanel("Viewport",true);

			Undo.RegisterCreatedObjectUndo(viewport,"Create Viewport");

			viewport.GetComponent<RectTransform>().pivot = new Vector2(0.0f,1.0f);

			viewport.AddComponent<Image>();
			viewport.AddComponent<Mask>();
			scroller.transform.SetUIChild(viewport.transform);

			var content = _CreatePanel("Content",true);

			Undo.RegisterCreatedObjectUndo(content,"Create Content");

			content.GetComponent<RectTransform>().pivot = new Vector2(0.0f,1.0f);

			viewport.transform.SetUIChild(content.transform);

			scroller.AddComponent<FocusScroller>();

			_LinkCanvas(scroller);

			Undo.CollapseUndoOperations(group);
		}
		#endregion Focus Scroller

		private static GameObject _CreatePanel(string name,bool isExpand = false)
		{
			var panel = new GameObject(name);
			var rect = panel.AddComponent<RectTransform>();

			if(isExpand)
			{
				rect.ExpandAnchorSize();
			}

			return panel;
		}

		private static void _LinkCanvas(GameObject child)
		{
			var selected = Selection.activeGameObject;

			if(selected)
			{
				selected.transform.SetUIChild(child.transform);
			}
			else
			{
				var canvas = _FindCanvas();

				canvas.transform.SetUIChild(child.transform);
			}

			Selection.activeObject = child;
		}

		private static Canvas _FindCanvas(GameObject selected = null)
		{
			var canvas = selected ? selected.GetComponentInParent<Canvas>() : Object.FindAnyObjectByType<Canvas>(FindObjectsInactive.Include);

			if(!canvas)
			{
				var uiName = "UI";
				var panel = new GameObject("Canvas");

				panel.AddComponent<Canvas>();
				panel.AddComponent<CanvasScaler>();
				panel.AddComponent<GraphicRaycaster>();
				panel.layer = uiName.FindLayerByName(true);

				canvas = panel.GetComponent<Canvas>();
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;

				if(selected)
				{
					selected.transform.SetUIChild(panel.transform);
				}
			}

			return canvas;
		}
		#endregion UI

		#region Category Line
		[MenuItem("GameObject/Create Category Line",false,MenuOrder.GameObject.CATEGORY)]
		private static void _OnCreateCategoryLine()
		{
			Undo.IncrementCurrentGroup();

			var group = Undo.GetCurrentGroup();
			var line = new GameObject("     ") { tag = "Category" };

			Undo.RegisterCreatedObjectUndo(line,"Create Category Line");

			var selected = Selection.activeGameObject;

			if(selected)
			{
				var root = selected.transform.root;

				line.transform.SetSiblingIndex(root.GetSiblingIndex()+1);
			}

			Undo.CollapseUndoOperations(group);
		}
		#endregion Category Line

		#region Copy Hierarchy
		[MenuItem("GameObject/Copy Hierarchy",false,MenuOrder.GameObject.HIERARCHY)]
		private static void _OnCopyHierarchy()
		{
			var selected = Selection.activeGameObject;

			if(!selected)
			{
				return;
			}

			CommonUtility.CopyToClipBoard(selected.transform.FindHierarchy());
		}

		[MenuItem("GameObject/Copy Hierarchy",true,MenuOrder.GameObject.HIERARCHY)]
		private static bool _IsCopyHierarchy()
		{
			return Selection.gameObjects.Length == 1;
		}

		#endregion Copy Hierarchy
	}
}	
#endif