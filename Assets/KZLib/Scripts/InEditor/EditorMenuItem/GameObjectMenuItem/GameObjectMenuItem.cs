#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using KZLib.KZDevelop;

namespace KZLib.KZMenu
{
	public class GameObjectMenuItem
	{
		[MenuItem("GameObject/UI/Empty Panel",false,0)]
		private static void OnCreateEmptyPanel()
		{
			Undo.IncrementCurrentGroup();
			var group = Undo.GetCurrentGroup();

			var panel = CreatePanel("Empty Panel");

			Undo.RegisterCreatedObjectUndo(panel.gameObject,"Create EmptyPanel");

			LinkCanvas(panel.gameObject);

			Undo.CollapseUndoOperations(group);
		}

		private static RectTransform CreatePanel(string _name)
		{
			var panel = new GameObject(_name);
			var rect = panel.AddComponent<RectTransform>();

			return rect;
		}

		private static void LinkCanvas(GameObject _object)
		{
			var selected = Selection.activeGameObject;
			var canvas = FindCanvas(selected);

			canvas.transform.SetUIChild(_object.transform);

			if(selected)
			{
				selected.transform.SetUIChild(canvas.transform);
			}
		}

		private static Canvas FindCanvas(GameObject _selected)
		{
			var current = _selected ? _selected.GetComponentInParent<Canvas>() : Object.FindObjectOfType<Canvas>(true);

			if(!current)
			{
				var panel = new GameObject("Canvas");
				
				panel.AddComponent<Canvas>();
				panel.AddComponent<CanvasScaler>();
				panel.AddComponent<GraphicRaycaster>();
				panel.layer = CommonUtility.GetLayerByName("UI",true);

				current = panel.GetComponent<Canvas>();
				current.renderMode = RenderMode.ScreenSpaceOverlay;

				if(_selected)
				{
					_selected.transform.SetUIChild(panel.transform);
				}
			}

			return current;
		}

		#region Create Category Line
		[MenuItem("GameObject/Create Category Line",false,0)]
		private static void CreateCategoryLine()
		{
			Undo.IncrementCurrentGroup();
			var group = Undo.GetCurrentGroup();

			var line = new GameObject(Global.CATEGORY_LINE) { tag = Global.CATEGORY_TAG };
			Undo.RegisterCreatedObjectUndo(line.gameObject,"Create CategoryLine");

			if(Selection.activeGameObject)
			{
				if(Selection.activeGameObject.GetComponent<RectTransform>())
				{
					Selection.activeGameObject.transform.SetUIChild(line.transform);

					line.AddComponent<RectTransform>();
				}
				else
				{
					Selection.activeGameObject.transform.SetChild(line.transform);
				}
			}

			line.transform.localPosition = Vector3.zero;

			Undo.CollapseUndoOperations(group);
		}
		#endregion Create Category Line

		#region Create Focus Scroller
		[MenuItem("GameObject/UI/Focus Scroller",false,20)]
		private static void OnCreateFocusScroller()
		{
			Undo.IncrementCurrentGroup();
			var group = Undo.GetCurrentGroup();

			var scroller = CreatePanel("Scroller");
			Undo.RegisterCreatedObjectUndo(scroller.gameObject,"Create Scroller");

			scroller.gameObject.AddComponent<Image>();

			var viewport = CreatePanel("Viewport");
			Undo.RegisterCreatedObjectUndo(viewport.gameObject,"Create Viewport");

			viewport.ExpandAnchorSize();
			viewport.pivot = new Vector2(0.0f,1.0f);

			viewport.gameObject.AddComponent<Image>();
			viewport.gameObject.AddComponent<Mask>();
			scroller.transform.SetUIChild(viewport.transform);


			var content = CreatePanel("Content");
			Undo.RegisterCreatedObjectUndo(content.gameObject,"Create Content");

			content.ExpandAnchorSize();
			content.pivot = new Vector2(0.0f,1.0f);

			viewport.transform.SetUIChild(content.transform);
			scroller.gameObject.AddComponent<FocusScroller>();

			LinkCanvas(scroller.gameObject);

			Undo.CollapseUndoOperations(group);
		}
		#endregion Create Focus Scroller
	}
}	
#endif