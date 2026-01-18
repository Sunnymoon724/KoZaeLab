#if UNITY_EDITOR
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib
{
	[CustomEditor(typeof(ShapeDrawing))]
	public partial class ShapeDrawingEditor : OdinEditor
	{
		private ShapeDrawing m_shapeDrawing = null;

		private bool m_raycastPadding = false;

		private SerializedObject m_serializedObject = null;

		private SerializedProperty m_outlineThicknessProperty = null;
		private PropertyInfo m_outlineThicknessInfo = null;
		private SerializedProperty m_outlineColorProperty = null;
		private PropertyInfo m_outlineColorInfo = null;

		private SerializedProperty m_primitiveTypeProperty = null;
		private PropertyInfo m_primitiveTypeInfo = null;

		private SerializedProperty m_fillTypeProperty = null;
		private PropertyInfo m_fillTypeInfo = null;
		private SerializedProperty m_fillColorProperty = null;
		private PropertyInfo m_fillColorInfo = null;

		private RectTransform m_rectTrans = null;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_shapeDrawing = target as ShapeDrawing;

			Undo.undoRedoPerformed -= Repaint;
			Undo.undoRedoPerformed += Repaint;

			m_serializedObject = new SerializedObject(target);

			_SetEllipse();
			_SetPolygon();
			// _SetRectangle();

			m_outlineThicknessProperty = m_serializedObject.FindProperty("m_outlineThickness");
			m_outlineThicknessInfo = target.GetType().GetProperty("OutlineThickness",BindingFlags.NonPublic | BindingFlags.Instance);
			m_outlineColorProperty = m_serializedObject.FindProperty("m_outlineColor");
			m_outlineColorInfo = target.GetType().GetProperty("OutlineColor",BindingFlags.NonPublic | BindingFlags.Instance);

			m_primitiveTypeProperty = m_serializedObject.FindProperty("m_primitiveType");
			m_primitiveTypeInfo = target.GetType().GetProperty("PrimitiveType",BindingFlags.NonPublic | BindingFlags.Instance);

			m_fillTypeProperty = m_serializedObject.FindProperty("m_fillType");
			m_fillTypeInfo = target.GetType().GetProperty("FillType",BindingFlags.NonPublic | BindingFlags.Instance);
			m_fillColorProperty = m_serializedObject.FindProperty("m_fillColor");
			m_fillColorInfo = target.GetType().GetProperty("FillColor",BindingFlags.NonPublic | BindingFlags.Instance);

			m_rectTrans = m_shapeDrawing.GetComponent<RectTransform>();
		}

		public override void OnInspectorGUI()
		{
			m_serializedObject.Update();

			_DrawPrimitiveType();
			_DrawOutline();
			_DrawDefaultField();

			var primitiveType = m_primitiveTypeProperty.enumValueIndex;

			if(primitiveType == 0)
			{
				_DrawEllipse();
			}
			else if(primitiveType == 1)
			{
				_DrawPolygon();
			}
			// else if(shapeType == 2)
			// {
			// 	_DrawRectangle();
			// }
			else
			{
				var enumNameArray = m_primitiveTypeProperty.enumNames;

				LogChannel.System.E($"{enumNameArray[primitiveType]} is not supported.");

				return;
			}

			_DrawFill();

			m_serializedObject.ApplyModifiedProperties();
		}

		private void _DrawPrimitiveType()
		{
			var oldIndex = m_primitiveTypeProperty.enumValueIndex;

			EditorGUI.BeginChangeCheck();

			var newIndex = EditorGUILayout.Popup("Primitive Type",oldIndex,m_primitiveTypeProperty.enumDisplayNames);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Primitive Type");

				m_primitiveTypeInfo.SetValue(target,newIndex);

				m_serializedObject.Update();
			}
		}

		private void _DrawOutline()
		{
			EditorGUI.BeginChangeCheck();

			var newOutlineThickness = EditorGUILayout.FloatField("Outline Thickness",m_outlineThicknessProperty.floatValue);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Outline Thickness");

				var length = Mathf.Min(m_rectTrans.rect.width,m_rectTrans.rect.height);

				m_outlineThicknessInfo.SetValue(target,Mathf.Clamp(newOutlineThickness,0.0f,length/2.0f));
			}

			EditorGUI.BeginChangeCheck();

			var newOutlineColor = EditorGUILayout.ColorField(new GUIContent("Outline Color"),m_outlineColorProperty.colorValue);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Outline Color");

				m_outlineColorInfo.SetValue(target,newOutlineColor);

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
			var oldIndex = m_fillTypeProperty.enumValueIndex;

			EditorGUI.BeginChangeCheck();

			var newIndex = EditorGUILayout.Popup("Fill Type",oldIndex,m_fillTypeProperty.enumDisplayNames);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Fill Type");

				m_fillTypeInfo.SetValue(target,newIndex);

				m_serializedObject.Update();
			}

			var index = m_fillTypeProperty.enumValueIndex;

			if(index != 2) //? Solid
			{
				return;
			}

			EditorGUI.BeginChangeCheck();

			var newFillColor = EditorGUILayout.ColorField(new GUIContent("Fill Color"),m_fillColorProperty.colorValue);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Fill Color");

				m_fillColorInfo.SetValue(target,newFillColor);

				m_serializedObject.Update();
			}
		}
	}
}
#endif