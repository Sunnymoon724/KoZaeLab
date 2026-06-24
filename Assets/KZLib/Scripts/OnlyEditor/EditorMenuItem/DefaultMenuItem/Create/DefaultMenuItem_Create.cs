#if UNITY_EDITOR
using System;
using KZLib.EditorTools;
using KZLib.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

using Object = UnityEngine.Object;

namespace KZLib.EditorInternal.Menus
{
	/// <summary>
	/// <c>GameObject</c> object creation menu entries.
	/// </summary>
	public static partial class DefaultMenuItem
	{
		#region UI
		#region Empty Panel
		[MenuItem("GameObject/UI/Empty Panel",false,MenuOrder.Create.UI.EMPTY_PANEL)]
		private static void _OnCreateEmptyPanel()
		{
			_OnCreateUI("EmptyPanel",true,null);
		}
		#endregion Empty Panel

		#region Shape
		[MenuItem("GameObject/UI/Shape",false,MenuOrder.Create.UI.SHAPE)]
		private static void _OnCreateShape()
		{
			_OnCreateUI("Shape",false,typeof(ShapeDrawing));
		}
		#endregion Shape

		#region Carousel
		[MenuItem("GameObject/UI/Carousel",false,MenuOrder.Create.UI.CAROUSEL)]
		private static void _OnCreateCarousel()
		{
			Undo.IncrementCurrentGroup();

			var group = Undo.GetCurrentGroup();
			var carouselPanel = _CreatePanel("Carousel",false);
			var viewportPanel = _CreatePanel("Viewport");

			viewportPanel.AddComponent<Image>();

			var viewportMask = viewportPanel.AddComponent<Mask>();
			viewportMask.showMaskGraphic = false;

			_SetChildUndo(carouselPanel.transform,viewportPanel.transform,false);

			var contentPanel = _CreatePanel("Content",true);

			_SetChildUndo(viewportPanel.transform,contentPanel.transform,false);

			var carousel = carouselPanel.AddComponent<Carousel>();
			carousel.SetScrollRect();

			Undo.RegisterCreatedObjectUndo(carouselPanel,"Create Carousel");

			_LinkCanvas(carouselPanel);

			Undo.CollapseUndoOperations(group);
		}
		#endregion Carousel

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
			var parent = _ResolveUIParent();

			_SetChildUndo(parent,child.transform,false);

			child.SetAllLayer(parent.gameObject.layer);

			_EnsureEventSystem();

			Selection.activeObject = child;
		}

		private static Transform _ResolveUIParent()
		{
			var selected = Selection.activeGameObject;

			if(selected && selected.GetComponentInParent<Canvas>())
			{
				return selected.transform;
			}

			return _FindCanvas().transform;
		}

		private static void _SetChildUndo(Transform parent,Transform child,bool worldPositionStays = true,bool isSameLayer = true)
		{
			Undo.SetTransformParent(child,parent,"ReParent Child");

			parent.SetChild(child,worldPositionStays,isSameLayer);
		}

		private static void _EnsureEventSystem()
		{
			if(Object.FindAnyObjectByType<EventSystem>(FindObjectsInactive.Include))
			{
				return;
			}

			var eventSystem = new GameObject("EventSystem");

			eventSystem.AddComponent<EventSystem>();
			eventSystem.AddComponent<InputSystemUIInputModule>();

			Undo.RegisterCreatedObjectUndo(eventSystem,"Create EventSystem");
		}

		private static Canvas _FindCanvas()
		{
			var canvas = Object.FindAnyObjectByType<Canvas>(FindObjectsInactive.Include);

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

				Undo.RegisterCreatedObjectUndo(panel,"Create Canvas");
			}

			return canvas;
		}
		#endregion UI

		#region Path Creator
		[MenuItem("GameObject/Path Creator",false,MenuOrder.Create.PATH_CREATOR)]
		private static void _OnCreatePathCreator()
		{
			Undo.IncrementCurrentGroup();

			var group = Undo.GetCurrentGroup();
			var gameObject = new GameObject("Path Creator");

			gameObject.AddComponent<LineRenderer>();
			gameObject.AddComponent<PathCreator>();

			var parent = Selection.activeTransform;

			if(parent)
			{
				Undo.SetTransformParent(gameObject.transform,parent,"Parent Path Creator");
			}

			Undo.RegisterCreatedObjectUndo(gameObject,"Create Path Creator");

			Selection.activeGameObject = gameObject;

			Undo.CollapseUndoOperations(group);
		}
		#endregion Path Creator
	}
}
#endif
