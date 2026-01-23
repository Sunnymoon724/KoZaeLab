#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib
{
	[CustomEditor(typeof(ShapeDrawing))]
	public partial class ShapeDrawingEditor : GraphicDrawingEditor
	{
		protected override SpaceType CurrentSpaceType => SpaceType.xy;
		protected override Transform CurrentTransform => m_shapeDrawing.transform;

		private ShapeDrawing m_shapeDrawing = null;
		private RectTransform m_rectTrans = null;

		protected override void _DoEnable()
		{
			m_shapeDrawing = target as ShapeDrawing;

			m_rectTrans = m_shapeDrawing.GetComponent<RectTransform>();
		}

		protected override void _DoInspectorGUI()
		{
			_DrawPrimitiveType();
			_DrawOutline();
			_DrawDefaultField();

			switch(m_shapeDrawing.PrimitiveType)
			{
				case ShapeDrawing.ShapePrimitiveType.Ellipse:
					{
						_DrawEllipse();
						break;
					}
				case ShapeDrawing.ShapePrimitiveType.Polygon:
					{
						_DrawPolygon();
						break;
					}
				// case ShapeType.Rectangle:
				default:
					{
						LogChannel.System.E($"{m_shapeDrawing.PrimitiveType} is not supported.");

						return;
					}
			}

			_DrawFill();
		}

		private void OnSceneGUI()
		{
			_SceneGUI();
		}

		private void _DrawPrimitiveType()
		{
			EditorGUI.BeginChangeCheck();

			var newType = (ShapeDrawing.ShapePrimitiveType) EditorGUILayout.EnumPopup("Primitive Type",m_shapeDrawing.PrimitiveType);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Primitive Type");

				m_shapeDrawing.PrimitiveType = newType;

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

				var thickness = Mathf.Min(m_rectTrans.rect.width,m_rectTrans.rect.height);

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

			m_raycastPadding = EditorGUILayout.Foldout(m_raycastPadding,"Raycast Padding");

			if(m_raycastPadding)
			{
				EditorGUI.BeginChangeCheck();

				var x = EditorGUILayout.FloatField("Left",m_shapeDrawing.raycastPadding.x);
				var y = EditorGUILayout.FloatField("Right",m_shapeDrawing.raycastPadding.y);
				var z = EditorGUILayout.FloatField("Top",m_shapeDrawing.raycastPadding.z);
				var w = EditorGUILayout.FloatField("Bottom",m_shapeDrawing.raycastPadding.w);

				if(EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(m_shapeDrawing,"Change Raycast Padding");

					m_shapeDrawing.raycastPadding = new Vector4(x,y,z,w);
				}
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

			var newType = (ShapeDrawing.ShapeFillType) EditorGUILayout.EnumPopup("Fill Type",m_shapeDrawing.FillType);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Fill Type");

				m_shapeDrawing.FillType = newType;

				m_serializedObject.Update();
			}

			var fillType = m_shapeDrawing.FillType;

			if(fillType != ShapeDrawing.ShapeFillType.Solid) //? solid is use fill color only
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

		protected override bool _IsShowHandle()
		{
			switch(m_shapeDrawing.PrimitiveType)
			{
				case ShapeDrawing.ShapePrimitiveType.Polygon:
					{
						return m_shapeDrawing.PolygonVertexDistanceCount >= Global.MIN_POLYGON_COUNT;
					}
				// case ShapeType.Rectangle:
				case ShapeDrawing.ShapePrimitiveType.Ellipse:
					{
						return false;
					}
				default:
					{
						LogChannel.System.E($"{m_shapeDrawing.PrimitiveType} is not supported.");

						return false;
					}
			}
		}
	}
}
#endif