#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace KZLib.Windows
{
	/// <summary>
	/// Base Odin editor window that displays a single Unity resource through IMGUI.
	/// Derived windows implement <see cref="_OnImGUI"/> and assign the asset via <see cref="SetResource"/>.
	/// </summary>
	public abstract class ResourceWindow<TResource> : OdinEditorWindow where TResource : Object
	{
		protected TResource m_resource = null;

		/// <summary>
		/// Assigns the resource shown by this window and requests a repaint.
		/// </summary>
		public virtual void SetResource(TResource resource)
		{
			m_resource = resource;

			Repaint();
		}

		protected override void OnImGUI()
		{
			if(!m_resource)
			{
				_DrawEmptyState();

				return;
			}

			_OnImGUI();
		}

		/// <summary>
		/// Draws the resource-specific viewer content when a resource is assigned.
		/// </summary>
		protected abstract void _OnImGUI();

		/// <summary>
		/// Draws a placeholder message before any resource is assigned.
		/// </summary>
		protected virtual void _DrawEmptyState()
		{
			GUILayout.Label("No resource assigned.");
		}
	}
}
#endif
