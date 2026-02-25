#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using KZLib.UI;

using Object = UnityEngine.Object;

namespace KZLib.EditorInternal.Menus
{
	public static partial class DefaultMenuItem
	{
		#region UI
		#region Empty Panel
		[MenuItem("GameObject/UI/Empty Panel",false,MenuOrder.GameObject.UI)]
		private static void _OnCreateEmptyPanel()
		{
			_OnCreateUI("EmptyPanel",true,null);
		}
		#endregion Empty Panel

		#region Shape
		[MenuItem("GameObject/UI/Shape",false,MenuOrder.GameObject.UI)]
		private static void _OnCreateShape()
		{
			_OnCreateUI("Shape",false,typeof(ShapeDrawing));
		}
		#endregion Shape

		// #region UILine
		// [MenuItem("GameObject/UI/Line",false,MenuOrder.GameObject.UI)]
		// private static void _OnCreateLine()
		// {
		// 	_OnCreateUI("Line",false,typeof(UILine));
		// }
		// #endregion UILine

		private static void _OnCreateUI(string componentName,bool isExpand,Type componentType)
		{
			Undo.IncrementCurrentGroup();

			var group = Undo.GetCurrentGroup();
			var panel = _CreatePanel(componentName,isExpand);

			if(componentType != null)
			{
				panel.AddComponent(componentType);
			}

			Undo.RegisterCreatedObjectUndo(panel,$"Create {componentName}");

			_LinkCanvas(panel);

			Undo.CollapseUndoOperations(group);
		}

		#region Carousel
		[MenuItem("GameObject/UI/Carousel",false,MenuOrder.GameObject.UI)]
		private static void _OnCreateCarousel()
		{
			Undo.IncrementCurrentGroup();

			var group = Undo.GetCurrentGroup();
			var carouselPanel = _CreatePanel("Carousel",false);

			Undo.RegisterCreatedObjectUndo(carouselPanel,"Create Carousel");

			var viewportPanel = _CreatePanel("Viewport");

			Undo.RegisterCreatedObjectUndo(viewportPanel,"Create Viewport");

			viewportPanel.AddComponent<Image>();

			var viewportMask = viewportPanel.AddComponent<Mask>();
			viewportMask.showMaskGraphic = false;

			carouselPanel.transform.SetChild(viewportPanel.transform,false);

			var contentPanel = _CreatePanel("Content",true);

			Undo.RegisterCreatedObjectUndo(contentPanel,"Create Content");

			viewportPanel.transform.SetChild(contentPanel.transform,false);

			var carousel = carouselPanel.AddComponent<Carousel>();
			carousel.SetScrollRect();

			_LinkCanvas(carouselPanel);

			Undo.CollapseUndoOperations(group);
		}
		#endregion Carousel

		private static GameObject _CreatePanel(string name,bool isExpand = false)
		{
			var panel = new GameObject(name);
			var rectTrans = panel.AddComponent<RectTransform>();

			if(isExpand)
			{
				rectTrans.ExpandAnchorSize();
			}

			return panel;
		}

		private static void _LinkCanvas(GameObject child)
		{
			var selected = Selection.activeGameObject;

			if(selected)
			{
				selected.transform.SetChild(child.transform,false);
			}
			else
			{
				var canvas = _FindCanvas();

				canvas.transform.SetChild(child.transform,false);
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
					selected.transform.SetChild(panel.transform,false);
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
		private static bool _CanCopyHierarchy()
		{
			return Selection.gameObjects.Length == 1;
		}

		#endregion Copy Hierarchy
	}
}	
#endif