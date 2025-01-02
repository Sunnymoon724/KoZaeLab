#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace KZLib.KZWindow
{
	public class ScriptableObjectWindow : ResourceWindow<ScriptableObject>
	{
		private Editor m_editor = null;

		public override void SetResource(ScriptableObject scriptableObject)
		{
			base.SetResource(scriptableObject);

			m_editor = Editor.CreateEditor(m_resource);
		}

		protected override void _OnImGUI()
		{
			if(m_editor != null)
			{
				m_editor.OnInspectorGUI();
			}
		}
	}
}
#endif