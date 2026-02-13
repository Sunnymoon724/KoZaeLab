// #if UNITY_EDITOR
// using System.Reflection;
// using Sirenix.OdinInspector.Editor;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.UI;

// namespace KZLib
// {
// 	[CustomEditor(typeof(LineDrawing))]
// 	public partial class LineDrawingEditor : OdinEditor
// 	{
// 		private LineDrawing m_lineDrawing = null;

// 		private bool m_raycastPadding = false;

// 		private SerializedObject m_serializedObject = null;

// 		private SerializedProperty m_outlineThicknessProperty = null;
// 		private PropertyInfo m_outlineThicknessInfo = null;
// 		private SerializedProperty m_outlineColorProperty = null;
// 		private PropertyInfo m_outlineColorInfo = null;

// 		private SerializedProperty m_primitiveTypeProperty = null;
// 		private PropertyInfo m_primitiveTypeInfo = null;

// 		private SerializedProperty m_fillTypeProperty = null;
// 		private PropertyInfo m_fillTypeInfo = null;
// 		private SerializedProperty m_fillColorProperty = null;
// 		private PropertyInfo m_fillColorInfo = null;

// 		private RectTransform m_rectTrans = null;

// 		protected override void OnEnable()
// 		{
// 			base.OnEnable();

// 			m_lineDrawing = target as LineDrawing;

// 			Undo.undoRedoPerformed -= Repaint;
// 			Undo.undoRedoPerformed += Repaint;

// 			m_serializedObject = new SerializedObject(target);

// 		}

// 		public override void OnInspectorGUI()
// 		{
// 			m_serializedObject.Update();
			
			

// 			m_serializedObject.ApplyModifiedProperties();
// 		}
// 	}
// }
// #endif