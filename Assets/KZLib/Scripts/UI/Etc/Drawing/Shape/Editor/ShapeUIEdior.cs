#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib
{
	[CustomEditor(typeof(ShapeUI))]
	public partial class ShapeUIEditor : OdinEditor
	{
		private ShapeUI m_shape = null;

		private bool m_raycastPadding = false;

		private SerializedObject m_serializedObject = null;

		private SerializedProperty m_shapeTypeProperty = null;
		private SerializedProperty m_outlineSizeProperty = null;

		private SerializedProperty m_fillTypeProperty = null;
		private SerializedProperty m_fillColorProperty = null;

		private RectTransform m_rectTrans = null;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_shape = target as ShapeUI;

			Undo.undoRedoPerformed -= Repaint;
			Undo.undoRedoPerformed += Repaint;

			m_serializedObject = new SerializedObject(target);

			_SetEllipse();
			_SetPolygon();
			// _SetRectangle();

			m_shapeTypeProperty = m_serializedObject.FindProperty("m_shapeType");
			m_outlineSizeProperty = m_serializedObject.FindProperty("m_outlineSize");

			m_fillTypeProperty = m_serializedObject.FindProperty("m_fillType");
			m_fillColorProperty = m_serializedObject.FindProperty("m_fillColor");

			m_rectTrans = m_shape.GetComponent<RectTransform>();
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
				_DrawEllipse();
			}
			else if(shapeType == 1)
			{
				_DrawPolygon();
			}
			// else if(shapeType == 2)
			// {
			// 	_DrawRectangle();
			// }

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
#endif