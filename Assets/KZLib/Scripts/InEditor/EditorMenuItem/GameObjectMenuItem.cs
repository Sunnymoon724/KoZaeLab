#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using KZLib.KZDevelop;

namespace KZLib.KZMenu
{
	public class GameObjectMenuItem
	{
		#region UI
		#region Empty Panel
		[MenuItem("GameObject/UI/Empty Panel",false,1000)]
		private static void OnCreateEmptyPanel()
		{
			Undo.IncrementCurrentGroup();

			var group = Undo.GetCurrentGroup();
			var panel = CreatePanel("EmptyPanel",true);

			Undo.RegisterCreatedObjectUndo(panel,"Create EmptyPanel");

			LinkCanvas(panel);

			Undo.CollapseUndoOperations(group);
		}
		#endregion Empty Panel

		#region UIShape
		[MenuItem("GameObject/UI/Shape",false,1021)]
		private static void OnCreateShape()
		{
			Undo.IncrementCurrentGroup();

			var group = Undo.GetCurrentGroup();
			var shape = CreatePanel("Shape");

			shape.AddComponent<UIShape>();

			Undo.RegisterCreatedObjectUndo(shape,"Create Shape");

			LinkCanvas(shape);

			Undo.CollapseUndoOperations(group);
		}
		#endregion UIShape

		#region Focus Scroller
		[MenuItem("GameObject/UI/Focus Scroller",false,1025)]
		private static void OnCreateFocusScroller()
		{
			Undo.IncrementCurrentGroup();

			var group = Undo.GetCurrentGroup();
			var scroller = CreatePanel("Scroller",false);

			Undo.RegisterCreatedObjectUndo(scroller,"Create Scroller");

			scroller.AddComponent<Image>();

			var viewport = CreatePanel("Viewport",true);

			Undo.RegisterCreatedObjectUndo(viewport,"Create Viewport");

			viewport.GetComponent<RectTransform>().pivot = new Vector2(0.0f,1.0f);

			viewport.AddComponent<Image>();
			viewport.AddComponent<Mask>();
			scroller.transform.SetUIChild(viewport.transform);

			var content = CreatePanel("Content",true);

			Undo.RegisterCreatedObjectUndo(content,"Create Content");

			content.GetComponent<RectTransform>().pivot = new Vector2(0.0f,1.0f);

			viewport.transform.SetUIChild(content.transform);

			scroller.AddComponent<FocusScroller>();

			LinkCanvas(scroller);

			Undo.CollapseUndoOperations(group);
		}
		#endregion Focus Scroller

		private static GameObject CreatePanel(string name,bool isExpand = false)
		{
			var panel = new GameObject(name);
			var rect = panel.AddComponent<RectTransform>();

			if(isExpand)
			{
				rect.ExpandAnchorSize();
			}

			return panel;
		}

		private static void LinkCanvas(GameObject child)
		{
			var selected = Selection.activeGameObject;

			if(selected)
			{
				selected.transform.SetUIChild(child.transform);
			}
			else
			{
				var canvas = FindCanvas();

				canvas.transform.SetUIChild(child.transform);
			}

			Selection.activeObject = child;
		}

		private static Canvas FindCanvas(GameObject selected = null)
		{
			var canvas = selected ? selected.GetComponentInParent<Canvas>() : Object.FindObjectOfType<Canvas>(true);

			if(!canvas)
			{
				var panel = new GameObject("Canvas");

				panel.AddComponent<Canvas>();
				panel.AddComponent<CanvasScaler>();
				panel.AddComponent<GraphicRaycaster>();
				panel.layer = "UI".FindLayerByName(true);

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
		[MenuItem("GameObject/Create Category Line",false,0)]
		private static void OnCreateCategoryLine()
		{
			Undo.IncrementCurrentGroup();

			var group = Undo.GetCurrentGroup();
			var line = new GameObject(Global.CATEGORY_LINE) { tag = Global.CATEGORY_TAG };

			Undo.RegisterCreatedObjectUndo(line,"Create CategoryLine");

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
		[MenuItem("GameObject/Copy Hierarchy",false,-10)]
		private static void OnCopyHierarchy()
		{
			var selected = Selection.activeGameObject;

			if(!selected)
			{
				return;
			}

			CommonUtility.CopyToClipBoard(selected.transform.FindHierarchy());
		}

		[MenuItem("GameObject/Copy Hierarchy",true)]
		private static bool IsCopyHierarchy()
		{
			return Selection.gameObjects.Length == 1;
		}

		#endregion Copy Hierarchy
	}
}	
#endif