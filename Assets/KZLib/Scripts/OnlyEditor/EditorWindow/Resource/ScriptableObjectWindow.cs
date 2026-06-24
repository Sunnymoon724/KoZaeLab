#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace KZLib.Windows
{
	/// <summary>
	/// Unity inspector viewer for a single <see cref="ScriptableObject"/> asset.
	/// Reuses a cached <see cref="Editor"/> instance until the target asset changes.
	/// </summary>
	public class ScriptableObjectWindow : ResourceWindow<ScriptableObject>
	{
		private Editor m_editor = null;

		/// <summary>
		/// Assigns the ScriptableObject and rebuilds the inspector editor when the target changes.
		/// </summary>
		public override void SetResource(ScriptableObject scriptableObject)
		{
			if(m_resource == scriptableObject)
			{
				Repaint();

				return;
			}

			_DestroyEditor();

			base.SetResource(scriptableObject);

			if(m_resource)
			{
				m_editor = Editor.CreateEditor(m_resource);
			}
		}

		protected override void _OnImGUI()
		{
			if(m_editor == null)
			{
				return;
			}

			m_editor.OnInspectorGUI();
		}

		protected override void OnDisable()
		{
			_DestroyEditor();

			base.OnDisable();
		}

		/// <summary>
		/// Destroys the cached inspector editor created by <see cref="Editor.CreateEditor"/>.
		/// </summary>
		private void _DestroyEditor()
		{
			if(m_editor == null)
			{
				return;
			}

			DestroyImmediate(m_editor);
			m_editor = null;
		}
	}
}
#endif
