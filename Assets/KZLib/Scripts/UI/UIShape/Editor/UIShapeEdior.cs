using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib
{
	[CustomEditor(typeof(UIShape))]
	public class UIShapeEditor : OdinEditor
	{
		private UIShape m_shape = null;

		private bool m_raycastPadding = false;
		private bool m_arrayFoldout = false;

		private SerializedObject m_serializedObject = null;

		private SerializedProperty m_shapeTypeProperty = null;
		private SerializedProperty m_outlineSizeProperty = null;

		private SerializedProperty m_ellipseAngleProperty = null;

		private SerializedProperty m_polygonSideCountProperty = null;
		private SerializedProperty m_polygonVertexDistanceArrayProperty = null;

		private SerializedProperty m_fillTypeProperty = null;
		private SerializedProperty m_fillColorProperty = null;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_shape = target as UIShape;

			Undo.undoRedoPerformed -= Repaint;
			Undo.undoRedoPerformed += Repaint;

			m_serializedObject = new SerializedObject(target);

			m_shapeTypeProperty = m_serializedObject.FindProperty("m_shapeType");

			m_outlineSizeProperty = m_serializedObject.FindProperty("m_outlineSize");

			m_ellipseAngleProperty = m_serializedObject.FindProperty("m_ellipseAngle");

			m_polygonSideCountProperty = m_serializedObject.FindProperty("m_polygonSideCount");
			m_polygonVertexDistanceArrayProperty = m_serializedObject.FindProperty("m_polygonVertexDistanceArray");

			m_fillTypeProperty = m_serializedObject.FindProperty("m_fillType");
			m_fillColorProperty = m_serializedObject.FindProperty("m_fillColor");
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.PropertyField(m_shapeTypeProperty,new GUIContent("Shape Type"));

			var outline = EditorGUILayout.FloatField("Outline Size",m_outlineSizeProperty.floatValue);

			m_outlineSizeProperty.floatValue = outline < 0.0f ? 0.0f : outline;

			m_shape.color = EditorGUILayout.ColorField("Outline Color",m_shape.color);
			m_shape.material = EditorGUILayout.ObjectField("Material",m_shape.material,typeof(Material),false) as Material;

			m_shape.raycastTarget = EditorGUILayout.Toggle("Raycast Target",m_shape.raycastTarget);

			m_raycastPadding = EditorGUILayout.Foldout(m_raycastPadding,"Raycast Padding");

			if(m_raycastPadding)
			{
				var x = EditorGUILayout.FloatField("Left",m_shape.raycastPadding.x);
				var y = EditorGUILayout.FloatField("Right",m_shape.raycastPadding.y);
				var z = EditorGUILayout.FloatField("Top",m_shape.raycastPadding.z);
				var w = EditorGUILayout.FloatField("Bottom",m_shape.raycastPadding.w);

				m_shape.raycastPadding = new Vector4(x,y,z,w);
			}

			m_shape.maskable = EditorGUILayout.Toggle("Maskable",m_shape.maskable);

			var shapeType = m_shapeTypeProperty.intValue;

			if(shapeType == 0)
			{
				//? Eclipse
				m_ellipseAngleProperty.floatValue = EditorGUILayout.Slider("Angle",m_ellipseAngleProperty.floatValue,0.0f,360.0f);
			}
			else if(shapeType == 1)
			{
				//? Polygon
				var sideCount = m_polygonSideCountProperty.intValue;

				m_polygonSideCountProperty.intValue = EditorGUILayout.IntSlider("Side Count",m_polygonSideCountProperty.intValue,3,36);

				if(sideCount != m_polygonSideCountProperty.intValue)
				{
					sideCount = m_polygonSideCountProperty.intValue;

					var arrayCount = m_polygonVertexDistanceArrayProperty.arraySize;

					if(arrayCount < sideCount)
					{
						for(var i=arrayCount;i<sideCount;i++)
						{
							m_polygonVertexDistanceArrayProperty.arraySize++;

							var data = m_polygonVertexDistanceArrayProperty.GetArrayElementAtIndex(i);

							data.floatValue = 1;
						}
					}
					else
					{
						var difference = arrayCount-sideCount;

						for(var i=0;i<difference;i++)
						{
							m_polygonVertexDistanceArrayProperty.arraySize--;
						}
					}
				}

				if(m_polygonVertexDistanceArrayProperty.arraySize > 0)
				{
					m_arrayFoldout = EditorGUILayout.Foldout(m_arrayFoldout,"Vertex Distance Array");

					if(m_arrayFoldout)
					{
						EditorGUILayout.BeginVertical(EditorStyles.helpBox);

						for(var i=0;i<m_polygonVertexDistanceArrayProperty.arraySize;i++)
						{
							var distance = m_polygonVertexDistanceArrayProperty.GetArrayElementAtIndex(i);

							distance.floatValue = EditorGUILayout.Slider($"Vertex {i + 1}",distance.floatValue,0.0f,1.0f);
						}

						EditorGUILayout.EndVertical();
					}
				}
			}

			EditorGUILayout.PropertyField(m_fillTypeProperty,new GUIContent("Fill Type"));

			var fillType = m_fillTypeProperty.intValue;

			if(fillType == 2)
			{
				//? Solid
				EditorGUILayout.PropertyField(m_fillColorProperty,new GUIContent("Fill Color"));
			}

			m_serializedObject.ApplyModifiedProperties();
		}
	}
}