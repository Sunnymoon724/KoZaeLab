#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using KZLib.Utilities;
using UnityEditor;
using UnityEngine;

namespace KZLib.UI
{
	[CustomEditor(typeof(ShapeDrawing))]
	public partial class ShapeDrawingEditor : GraphicDrawingEditor
	{
		protected override Transform CurrentTransform => m_shapeDrawing.transform;

		private ShapeDrawing m_shapeDrawing = null;

		private class ShapeEditorCatalog : StrategyCatalog<ShapeDrawingEditor,ShapeDrawing.ShapePrimitiveType,ShapeEditorStrategy>
		{
			public ShapeEditorCatalog(ShapeDrawingEditor owner) : base(owner) { }

			protected override Dictionary<ShapeDrawing.ShapePrimitiveType,ShapeEditorStrategy> _BindStrategy()
			{
				return new()
				{
					[ShapeDrawing.ShapePrimitiveType.Ellipse] = new(m_owner._Draw_Ellipse,m_owner._CanShowKnot_Ellipse,m_owner._UpdateKnotList_Ellipse,m_owner._ChangeKnotPosition_Ellipse),
					[ShapeDrawing.ShapePrimitiveType.Polygon] = new(m_owner._Draw_Polygon,m_owner._CanShowKnot_Polygon,m_owner._UpdateKnotList_Polygon,m_owner._ChangeKnotPosition_Polygon),
				};
			}
		}

		private readonly struct ShapeEditorStrategy
		{
			public readonly Action Draw;
			public readonly Func<bool> CanShowKnot;
			public readonly Action<Vector3,Quaternion> UpdateKnotList;
			public readonly Action<int,Vector3> ChangeKnotPosition;

			public ShapeEditorStrategy(Action draw,Func<bool> canShowKnot,Action<Vector3,Quaternion> updateKnotList,Action<int,Vector3> changeKnotPosition)
			{
				Draw = draw;
				CanShowKnot = canShowKnot;
				UpdateKnotList = updateKnotList;
				ChangeKnotPosition = changeKnotPosition;
			}
		}

		private ShapeEditorCatalog m_shapeEditorCatalog = null;

		private bool _TryGetStrategy(ShapeDrawing.ShapePrimitiveType primitiveType,out ShapeEditorStrategy strategy)
		{
			m_shapeEditorCatalog ??= new ShapeEditorCatalog(this);

			return m_shapeEditorCatalog.TryGetStrategy(primitiveType,out strategy);
		}

		protected override void _DoEnable()
		{
			m_shapeDrawing = target as ShapeDrawing;
		}

		protected override void _DoInspectorGUI()
		{
			_DrawPrimitiveType();
			_DrawOutline();
			_DrawDefaultField();

			if(_TryGetStrategy(m_shapeDrawing.PrimitiveType,out var strategy))
			{
				strategy.Draw();
			}

			_DrawFill();
		}

		private void _DrawPrimitiveType()
		{
			EditorGUI.BeginChangeCheck();

			var newType = EditorGUILayout.EnumPopup("Primitive Type",m_shapeDrawing.PrimitiveType);

			if(EditorGUI.EndChangeCheck() && newType is ShapeDrawing.ShapePrimitiveType primitiveType)
			{
				Undo.RecordObject(m_shapeDrawing,"Change Primitive Type");

				m_shapeDrawing.PrimitiveType = primitiveType;

				m_serializedObject.Update();
			}
		}

		private void _DrawOutline()
		{
			EditorGUI.BeginChangeCheck();

			var newOutlineThickness = EditorGUILayout.FloatField("Outline Thickness",m_shapeDrawing.OutlineThickness);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Outline Thickness");

				var rect = m_shapeDrawing.rectTransform.rect;
				var thickness = Mathf.Min(rect.width,rect.height);

				m_shapeDrawing.OutlineThickness = Mathf.Clamp(newOutlineThickness,0.0f,thickness/2.0f);
			}

			EditorGUI.BeginChangeCheck();

			var newOutlineColor = EditorGUILayout.ColorField(new GUIContent("Outline Color"),m_shapeDrawing.OutlineColor);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Outline Color");

				m_shapeDrawing.OutlineColor = newOutlineColor;

				m_serializedObject.Update();
			}
		}

		private void _DrawDefaultField()
		{
			EditorGUI.BeginChangeCheck();

			var newMaterial = EditorGUILayout.ObjectField("Material",m_shapeDrawing.material,typeof(Material),false) as Material;

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Material");

				m_shapeDrawing.material = newMaterial;
			}

			EditorGUI.BeginChangeCheck();

			var newRaycast = EditorGUILayout.Toggle("Raycast Target",m_shapeDrawing.raycastTarget);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Raycast Target");

				m_shapeDrawing.raycastTarget = newRaycast;
			}

			m_isRaycastPaddingExpanded = EditorGUILayout.Foldout(m_isRaycastPaddingExpanded,"Raycast Padding");

			if(m_isRaycastPaddingExpanded)
			{
				EditorGUI.indentLevel++;

				EditorGUI.BeginChangeCheck();

				var padding = new Vector4(
					EditorGUILayout.FloatField("Left",m_shapeDrawing.raycastPadding.x),
					EditorGUILayout.FloatField("Right",m_shapeDrawing.raycastPadding.y),
					EditorGUILayout.FloatField("Top",m_shapeDrawing.raycastPadding.z),
					EditorGUILayout.FloatField("Bottom",m_shapeDrawing.raycastPadding.w)
				);

				if(EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(m_shapeDrawing,"Change Raycast Padding");

					m_shapeDrawing.raycastPadding = padding;
				}

				EditorGUI.indentLevel--;
			}

			EditorGUI.BeginChangeCheck();

			var newMaskable = EditorGUILayout.Toggle("Maskable",m_shapeDrawing.maskable);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Maskable");

				m_shapeDrawing.maskable = newMaskable;
			}
		}

		private void _DrawFill()
		{
			EditorGUI.BeginChangeCheck();

			var newType = EditorGUILayout.EnumPopup("Fill Type",m_shapeDrawing.FillType);

			if(EditorGUI.EndChangeCheck() && newType is ShapeDrawing.ShapeFillType shapeFillType)
			{
				Undo.RecordObject(m_shapeDrawing,"Change Fill Type");

				m_shapeDrawing.FillType = shapeFillType;

				m_serializedObject.Update();
			}

			if(m_shapeDrawing.FillType != ShapeDrawing.ShapeFillType.Solid) //? solid is use fill color only
			{
				return;
			}

			EditorGUI.BeginChangeCheck();

			var newFillColor = EditorGUILayout.ColorField(new GUIContent("Fill Color"),m_shapeDrawing.FillColor);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Fill Color");

				m_shapeDrawing.FillColor = newFillColor;

				m_serializedObject.Update();
			}
		}

		private void OnSceneGUI()
		{
			_SceneGUI();
		}

		protected override bool _CanShowKnot()
		{
			var radius = m_shapeDrawing.Radius;

			if(radius.x <= 0.0f || radius.y <= 0.0f)
			{
				return false;
			}

			return _TryGetStrategy(m_shapeDrawing.PrimitiveType,out var strategy) && strategy.CanShowKnot();
		}

		protected override Vector3[] _GetWorldCornerArray()
		{
			var cornerArray = new Vector3[5];

			m_shapeDrawing.rectTransform.GetWorldCorners(cornerArray);

			return cornerArray;
		}

		protected override void _DoUpdateKnotList()
		{
			CurrentTransform.GetPositionAndRotation(out var position,out var rotation);

			_AddOrUpdateKnotInfo(0,position,KnotType.Fixed);

			if(_TryGetStrategy(m_shapeDrawing.PrimitiveType,out var strategy))
			{
				strategy.UpdateKnotList(position,rotation);
			}
		}

		protected override void _ChangeKnotPosition(int index,Vector3 newPosition)
		{
			var localPos = CurrentTransform.InverseTransformPoint(newPosition);

			if(_TryGetStrategy(m_shapeDrawing.PrimitiveType,out var strategy))
			{
				strategy.ChangeKnotPosition(index,localPos);
			}
		}

		protected override Vector3 _ConvertToMousePosition(Vector3 mousePosition)
		{
			var plane = new Plane(Vector3.forward,CurrentTransform.position);
			var ray = HandleUtility.GUIPointToWorldRay(mousePosition);

			return plane.Raycast(ray,out float enter) ? ray.GetPoint(enter) : CurrentTransform.position;
		}

		private Vector3 _GetEdgePosition(float angle)
		{
			var radius = m_shapeDrawing.Radius;

			var cos = Mathf.Cos(angle);
			var sin = Mathf.Sin(angle);

			return new Vector3(cos*radius.x,sin*radius.y);
		}
	}
}
#endif