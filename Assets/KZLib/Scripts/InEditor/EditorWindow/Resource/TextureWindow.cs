#if UNITY_EDITOR
using UnityEngine;

namespace KZLib.KZWindow
{
	public class TextureWindow : ResourceWindow<Texture2D>
	{
		private Vector2 m_scrollPosition = Vector2.zero;
		private float m_scale = 1.0f;

		public override void SetResource(Texture2D texture2D)
		{
			base.SetResource(texture2D);
		}

		protected override void _OnImGUI()
		{
			var current = Event.current;

			if(current.type == EventType.ScrollWheel)
			{
				m_scale += -current.delta.y*0.1f;
				m_scale = Mathf.Clamp(m_scale,0.1f,10.0f);

				Repaint();
			}

			var width = m_resource.width*m_scale;
			var height = m_resource.height*m_scale;
			var startX = (position.width-width)*0.5f;
			var startY = (position.height-height)*0.5f;

			m_scrollPosition = GUI.BeginScrollView(new Rect(0.0f,0.0f,position.width,position.height),m_scrollPosition,new Rect(0.0f,0.0f,width,height));

			GUI.DrawTexture(new Rect(startX,startY,width,height),m_resource,ScaleMode.ScaleToFit);
			GUI.EndScrollView();
		}
	}
}

#endif