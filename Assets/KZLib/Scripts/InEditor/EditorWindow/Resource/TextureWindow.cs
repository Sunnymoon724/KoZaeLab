#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace KZLib.KZWindow
{
	public class TextureWindow : OdinEditorWindow
	{
		private Texture2D m_Texture = null;
		private Vector2 m_ScrollPosition = Vector2.zero;
		private float m_Scale = 1.0f;

		public void SetTexture(Texture2D _texture)
		{
			m_Texture = _texture;
		}

		protected override void OnImGUI()
		{
			if(!m_Texture)
			{
				GUILayout.Label("Texture is null");

				return;
			}

			var current = Event.current;

			if(current.type == EventType.ScrollWheel)
			{
				m_Scale += -current.delta.y*0.1f;
				m_Scale = Mathf.Clamp(m_Scale,0.1f,10.0f);

				Repaint();
			}

			var width = m_Texture.width*m_Scale;
			var height = m_Texture.height*m_Scale;
			var startX = (position.width-width)*0.5f;
			var startY = (position.height-height)*0.5f;

			m_ScrollPosition = GUI.BeginScrollView(new Rect(0.0f,0.0f,position.width,position.height),m_ScrollPosition,new Rect(0.0f,0.0f,width,height));

			GUI.DrawTexture(new Rect(startX,startY,width,height),m_Texture,ScaleMode.ScaleToFit);
			GUI.EndScrollView();
		}
	}
}

#endif