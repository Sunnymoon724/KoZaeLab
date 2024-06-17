#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace KZLib.KZWindow
{
	public class ScriptableObjectWindow : OdinEditorWindow
	{
		[SerializeField]
		private ScriptableObject m_Asset = null;

		private Editor m_Editor = null;

		public void SetScriptableObject(ScriptableObject _asset)
		{
			m_Asset = _asset;
			m_Editor = Editor.CreateEditor(m_Asset);
		}

		protected override void OnImGUI()
		{
			if(m_Editor != null)
			{
				m_Editor.OnInspectorGUI();
			}
		}
	}
}
#endif