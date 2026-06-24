#if UNITY_EDITOR
using UnityEngine;

namespace KZLib.Windows
{
	/// <summary>
	/// Scrollable texture preview window with mouse-wheel zoom.
	/// Opened from texture path drawers and the asset context menu.
	/// </summary>
	public class TextureWindow : ResourceWindow<Texture2D>
	{
		private Vector2 m_scrollPosition = Vector2.zero;
		private float m_scale = 1.0f;

		/// <summary>
		/// Resets pan/zoom state when a different texture is opened.
		/// </summary>
		public override void SetResource(Texture2D texture2D)
		{
			if(m_resource != texture2D)
			{
				m_scrollPosition = Vector2.zero;
				m_scale = 1.0f;
			}

			base.SetResource(texture2D);
		}

		protected override void _OnImGUI()
		{
			var current = Event.current;

			if(current.type == EventType.ScrollWheel)
			{
				m_scale += -current.delta.y*0.1f;
				m_scale = Mathf.Clamp(m_scale,0.1f,10.0f);

				current.Use();
				Repaint();
			}

			var width = m_resource.width*m_scale;
			var height = m_resource.height*m_scale;
			var contentWidth = Mathf.Max(width,position.width);
			var contentHeight = Mathf.Max(height,position.height);
			var startX = (contentWidth-width)*0.5f;
			var startY = (contentHeight-height)*0.5f;

			m_scrollPosition = GUI.BeginScrollView(new Rect(0.0f,0.0f,position.width,position.height),m_scrollPosition,new Rect(0.0f,0.0f,contentWidth,contentHeight));

			GUI.DrawTexture(new Rect(startX,startY,width,height),m_resource,ScaleMode.ScaleToFit);

			GUI.EndScrollView();
		}
	}
}
#endif
