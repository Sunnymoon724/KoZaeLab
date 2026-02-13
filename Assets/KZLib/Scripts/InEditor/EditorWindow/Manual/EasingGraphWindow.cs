#if UNITY_EDITOR
using System.Collections.Generic;
using KZLib.Attributes;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace KZLib.Windows
{
	public class EasingGraphWindow : OdinEditorWindow
	{
		[SerializeField,HideInInspector]
		private float m_currentPoint = 0.0f;

		[HorizontalGroup("Type",Order = 0),SerializeField]
		private EaseType m_easeType = EaseType.Linear;

		[HorizontalGroup("Value",Order = 1)]
		[HorizontalGroup("Value/0",Order = 0),ShowInInspector,PropertyRange(0.0f,1.0f)]
		private float CurrentPoint
		{
			get => m_currentPoint;
			set
			{
				m_currentPoint = value;

				m_resultPoint = CommonUtility.GetEaseCurve(m_easeType).Evaluate(value);
			}
		}

		[HorizontalGroup("Value/1",Order = 1),SerializeField,KZRichText]
		private float m_resultPoint = 0.0f;

		[HorizontalGroup("Graph",Order = 2),OnInspectorGUI]
		protected void DrawGraph()
		{
			GUILayout.Space(20);

			var rect = GUILayoutUtility.GetRect(700,700);

			_DrawGrid(rect,20.0f);

			if(Event.current.type == EventType.Repaint)
			{
				Handles.BeginGUI();

				_DrawGraphLine(rect,Color.blue);

				_DrawPoint(rect,Color.red);

				Handles.EndGUI();
			}
		}

		private void _DrawGrid(Rect rect,float size)
		{
			Handles.color = new Color(0.5f,0.5f,0.5f,0.2f);

			for(var x=rect.x;x<rect.x+rect.width;x+=size)
			{
				Handles.DrawLine(new Vector3(x,rect.y,0.0f),new Vector3(x,rect.y+rect.height,0.0f));
			}

			for(var y=rect.y;y<rect.y+rect.height;y+=size)
			{
				Handles.DrawLine(new Vector3(rect.x,y,0.0f),new Vector3(rect.x+rect.width,y,0.0f));
			}

			Handles.color = new Color(0.5f,0.5f,0.5f,1.0f);

			Handles.DrawLine(new Vector3(rect.x,rect.y,0.0f),new Vector3(rect.x,rect.y+rect.height,0.0f));
			Handles.DrawLine(new Vector3(rect.x,rect.y+rect.height,0.0f),new Vector3(rect.x+rect.width,rect.y+rect.height,0.0f));
		}

		private void _DrawGraphLine(Rect rect,Color color)
		{
			Handles.color = color;

			var pointList = new List<Vector3>();
			var curve = CommonUtility.GetEaseCurve(m_easeType);

			for(var i=0.0f;i<1.0f;i+=0.01f)
			{
				var xTime = i;
				var yTime = curve.Evaluate(i);

				var xPosition = Mathf.Lerp(rect.x,rect.x+rect.width,xTime);
				var yPosition = rect.y+rect.height-(yTime*rect.height);

				pointList.Add(new Vector3(xPosition,yPosition,0.0f));
			}

			Handles.DrawAAPolyLine(3.0f,pointList.ToArray());
		}

		private void _DrawPoint(Rect rect,Color color)
		{
			var currentX = Mathf.Lerp(rect.x,rect.x+rect.width,CurrentPoint);
			var currentY = rect.y + rect.height-(m_resultPoint*rect.height);

			Handles.color = color;
			Handles.DrawSolidDisc(new Vector3(currentX,currentY,0.0f),Vector3.forward,5.0f);
		}
	}
}
#endif