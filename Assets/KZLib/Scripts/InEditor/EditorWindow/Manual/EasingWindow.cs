#if UNITY_EDITOR
using System.Collections.Generic;
using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace KZLib.KZWindow
{
	public class EasingWindow : OdinEditorWindow
	{
		[SerializeField,HideInInspector]
		private float m_CurrentValue = 0.0f;

		
		[HorizontalGroup("Type",Order = 0),SerializeField,LabelText("Ease Type")]
		private EaseType m_EaseType = EaseType.Linear;

		[HorizontalGroup("Value",Order = 1)]
		[HorizontalGroup("Value/0",Order = 0),ShowInInspector,LabelText("Current Value"),PropertyRange(0.0f,1.0f)]
		protected float CurrentValue
		{
			get => m_CurrentValue;
			set
			{
				m_CurrentValue = value;

				m_ResultValue = CommonUtility.GetEaseCurve(m_EaseType).Evaluate(value);
			}
		}

		[HorizontalGroup("Value/1",Order = 1),SerializeField,LabelText("Result Value"),KZRichText]
		protected float m_ResultValue = 0.0f;

		[HorizontalGroup("Graph",Order = 2),OnInspectorGUI]
		protected void DrawGraph()
		{
			GUILayout.Space(20);

			var rect = GUILayoutUtility.GetRect(500,500);

			DrawGrid(rect,20.0f);

			if(Event.current.type == EventType.Repaint)
			{
				Handles.BeginGUI();

				DrawGraphLine(rect,Color.blue);

				Handles.EndGUI();
			}
		}

		private void DrawGrid(Rect _rect,float _size)
		{
			Handles.color = new Color(0.5f,0.5f,0.5f,0.2f);

			for(var x=_rect.x;x<_rect.x+_rect.width;x+=_size)
			{
				Handles.DrawLine(new Vector3(x,_rect.y,0.0f),new Vector3(x,_rect.y+_rect.height,0.0f));
			}

			for(var y=_rect.y;y<_rect.y+_rect.height;y+=_size)
			{
				Handles.DrawLine(new Vector3(_rect.x,y,0.0f),new Vector3(_rect.x+_rect.width,y,0.0f));
			}

			Handles.color = new Color(0.5f,0.5f,0.5f,1.0f);

			Handles.DrawLine(new Vector3(_rect.x+_rect.width/2.0f,_rect.y,0.0f),new Vector3(_rect.x+_rect.width/2.0f,_rect.y+_rect.height,0.0f));
			Handles.DrawLine(new Vector3(_rect.x,_rect.y+_rect.height/2.0f,0.0f),new Vector3(_rect.x+_rect.width,_rect.y+_rect.height/2.0f,0.0f));
		}

		private void DrawGraphLine(Rect _rect,Color _color)
		{
			Handles.color = _color;

			var pointList = new List<Vector3>();
			var curve = CommonUtility.GetEaseCurve(m_EaseType);

			for(var i=0.0f;i<1.0f;i+=0.01f)
			{
				var xTime = i;
				var yTime = curve.Evaluate(i);

				pointList.Add(new Vector3(Mathf.Lerp(_rect.x,_rect.x+_rect.width,xTime),Mathf.Lerp(_rect.y+_rect.height,_rect.y,yTime),0.0f));
			}

			Handles.DrawAAPolyLine(3.0f,pointList.ToArray());
		}
	}
}
#endif