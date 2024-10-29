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

		private static GameObject CreatePanel(string _name,bool _expand = false)
		{
			var panel = new GameObject(_name);
			var rect = panel.AddComponent<RectTransform>();

			if(_expand)
			{
				rect.ExpandAnchorSize();
			}

			return panel;
		}

		private static void LinkCanvas(GameObject _object)
		{
			var selected = Selection.activeGameObject;

			if(selected)
			{
				selected.transform.SetUIChild(_object.transform);
			}
			else
			{
				var canvas = FindCanvas();

				canvas.transform.SetUIChild(_object.transform);
			}

			Selection.activeObject = _object;
		}

		private static Canvas FindCanvas(GameObject _selected = null)
		{
			var canvas = _selected ? _selected.GetComponentInParent<Canvas>() : Object.FindObjectOfType<Canvas>(true);

			if(!canvas)
			{
				var panel = new GameObject("Canvas");

				panel.AddComponent<Canvas>();
				panel.AddComponent<CanvasScaler>();
				panel.AddComponent<GraphicRaycaster>();
				panel.layer = "UI".GetLayerByName(true);

				canvas = panel.GetComponent<Canvas>();
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;

				if(_selected)
				{
					_selected.transform.SetUIChild(panel.transform);
				}
			}

			return canvas;
		}
		#endregion UI

		#region Category Line
		[MenuItem("GameObject/Create Category Line",false,0)]
		private static void CreateCategoryLine()
		{
			Undo.IncrementCurrentGroup();

			var group = Undo.GetCurrentGroup();
			var line = new GameObject(Global.CATEGORY_LINE) { tag = Global.CATEGORY_TAG };

			Undo.RegisterCreatedObjectUndo(line,"Create CategoryLine");

			var selection = Selection.activeGameObject;

			if(selection)
			{
				var root = selection.transform.root;

				line.transform.SetSiblingIndex(root.GetSiblingIndex()+1);
			}

			Undo.CollapseUndoOperations(group);
		}
		#endregion Category Line
	}
}	
#endif