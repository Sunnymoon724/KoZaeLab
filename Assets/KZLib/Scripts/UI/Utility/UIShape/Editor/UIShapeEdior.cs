using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.KZEditor
{
	[CustomEditor(typeof(UIShape))]
	public class UIShapeEditor : OdinEditor
	{
		private UIShape m_Shape = null;

		private bool m_RaycastPadding = false;
		private bool m_ArrayFoldout = false;

		private SerializedObject m_SerializedObject = null;

		private SerializedProperty m_ShapeTypeProperty = null;
		private SerializedProperty m_OutlineSizeProperty = null;

		private SerializedProperty m_EllipseReverseProperty = null;
		private SerializedProperty m_EllipseAngleProperty = null;
		private SerializedProperty m_EllipseUseCapProperty = null;

		private SerializedProperty m_PolygonSideCountProperty = null;
		private SerializedProperty m_PolygonVertexDistanceArrayProperty = null;

		private SerializedProperty m_RectangleRoundProperty = null;

		private SerializedProperty m_TriangleOffsetProperty = null;

		private SerializedProperty m_FillTypeProperty = null;
		private SerializedProperty m_FillColorProperty = null;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_Shape = target as UIShape;

			Undo.undoRedoPerformed -= Repaint;
			Undo.undoRedoPerformed += Repaint;

			m_SerializedObject = new SerializedObject(target);

			m_ShapeTypeProperty = m_SerializedObject.FindProperty("m_ShapeType");

			m_OutlineSizeProperty = m_SerializedObject.FindProperty("m_OutlineSize");

			m_EllipseReverseProperty = m_SerializedObject.FindProperty("m_EllipseReverse");
			m_EllipseAngleProperty = m_SerializedObject.FindProperty("m_EllipseAngle");
			m_EllipseUseCapProperty = m_SerializedObject.FindProperty("m_EllipseUseCap");

			m_PolygonSideCountProperty = m_SerializedObject.FindProperty("m_PolygonSideCount");
			m_PolygonVertexDistanceArrayProperty = m_SerializedObject.FindProperty("m_PolygonVertexDistanceArray");

			m_RectangleRoundProperty = m_SerializedObject.FindProperty("m_RectangleRound");

			m_TriangleOffsetProperty = m_SerializedObject.FindProperty("m_TriangleOffset");

			m_FillTypeProperty = m_SerializedObject.FindProperty("m_FillType");
			m_FillColorProperty = m_SerializedObject.FindProperty("m_FillColor");
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.PropertyField(m_ShapeTypeProperty,new GUIContent("Shape Type"));

			var outline = EditorGUILayout.FloatField("Outline Size",m_OutlineSizeProperty.floatValue);

			m_OutlineSizeProperty.floatValue = outline < 0.0f ? 0.0f : outline;

			m_Shape.color = EditorGUILayout.ColorField("Outline Color",m_Shape.color);
			m_Shape.material = EditorGUILayout.ObjectField("Material",m_Shape.material,typeof(Material),false) as Material;

			// m_Shape.material = EditorGUILayout.ObjectField("Material",m_Shape.material,typeof(Material),false) as Material;
			m_Shape.raycastTarget = EditorGUILayout.Toggle("Raycast Target",m_Shape.raycastTarget);

			m_RaycastPadding = EditorGUILayout.Foldout(m_RaycastPadding,"Raycast Padding");

			if(m_RaycastPadding)
			{
				var x = EditorGUILayout.FloatField("Left",m_Shape.raycastPadding.x);
				var y = EditorGUILayout.FloatField("Right",m_Shape.raycastPadding.y);
				var z = EditorGUILayout.FloatField("Top",m_Shape.raycastPadding.z);
				var w = EditorGUILayout.FloatField("Bottom",m_Shape.raycastPadding.w);

				m_Shape.raycastPadding = new Vector4(x,y,z,w);
			}

			m_Shape.maskable = EditorGUILayout.Toggle("Maskable",m_Shape.maskable);

			var ShapeType = m_ShapeTypeProperty.intValue;

			if(ShapeType == 0)
			{
				//? Eclipse
				EditorGUILayout.PropertyField(m_EllipseReverseProperty,new GUIContent("Reverse"));
				m_EllipseAngleProperty.floatValue = EditorGUILayout.Slider("Angle",m_EllipseAngleProperty.floatValue,0.0f,360.0f);
				EditorGUILayout.PropertyField(m_EllipseUseCapProperty,new GUIContent("Use Cap"));
			}
			else if(ShapeType == 1)
			{
				//? Polygon
				var sideCount = m_PolygonSideCountProperty.intValue;

				m_PolygonSideCountProperty.intValue = EditorGUILayout.IntSlider("Side Count",m_PolygonSideCountProperty.intValue,3,36);

				if(sideCount != m_PolygonSideCountProperty.intValue)
				{
					sideCount = m_PolygonSideCountProperty.intValue;

					var arrayCount = m_PolygonVertexDistanceArrayProperty.arraySize;

					if(arrayCount < sideCount)
					{
						for(var i=arrayCount;i<sideCount;i++)
						{
							m_PolygonVertexDistanceArrayProperty.arraySize++;

							var data = m_PolygonVertexDistanceArrayProperty.GetArrayElementAtIndex(i);

							data.floatValue = 1;
						}
					}
					else
					{
						var difference = arrayCount-sideCount;

						for(var i=0;i<difference;i++)
						{
							m_PolygonVertexDistanceArrayProperty.arraySize--;
						}
					}
				}

				if(m_PolygonVertexDistanceArrayProperty.arraySize > 0)
				{
					m_ArrayFoldout = EditorGUILayout.Foldout(m_ArrayFoldout,"Vertex Distance Array");

					if(m_ArrayFoldout)
					{
						EditorGUILayout.BeginVertical(EditorStyles.helpBox);

						for(var i=0;i<m_PolygonVertexDistanceArrayProperty.arraySize;i++)
						{
							var distance = m_PolygonVertexDistanceArrayProperty.GetArrayElementAtIndex(i);

							distance.floatValue = EditorGUILayout.Slider(string.Format("Vertex {0}",i+1),distance.floatValue,0.0f,1.0f);
						}

						EditorGUILayout.EndVertical();
					}
				}
			}
			// else if(ShapeType == 2)
			// {
			// 	//? Rectangle
			// 	EditorGUILayout.PropertyField(m_RectangleRoundProperty,new GUIContent("Round"));
			// }
			else if(ShapeType == 2)
			{
				//? Triangle
				m_TriangleOffsetProperty.floatValue = EditorGUILayout.Slider("Offset",m_TriangleOffsetProperty.floatValue,-1.0f,+1.0f);
			}

			EditorGUILayout.PropertyField(m_FillTypeProperty,new GUIContent("Fill Type"));

			var fillType = m_FillTypeProperty.intValue;

			if(fillType == 2)
			{
				//? Solid
				EditorGUILayout.PropertyField(m_FillColorProperty,new GUIContent("Fill Color"));
			}

			m_SerializedObject.ApplyModifiedProperties();
		}
	}
}