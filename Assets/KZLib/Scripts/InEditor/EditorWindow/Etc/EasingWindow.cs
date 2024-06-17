#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace KZLib.KZWindow
{
	public class EasingWindow : OdinEditorWindow
	{
		private EaseType m_EaseType = EaseType.Linear;

		private float m_Value = 0.0f;

		protected override void OnImGUI()
		{
			m_EaseType = (EaseType) EditorGUILayout.EnumPopup("이지 타입",m_EaseType);

			m_Value = EditorGUILayout.Slider("값 설정",m_Value,0.0f,1.0f);

			var curve = CommonUtility.GetEaseCurve(m_EaseType);

			EditorGUILayout.LabelField("결과 값",string.Format("{0}",curve.Evaluate(m_Value)));

			var rect = EditorGUILayout.GetControlRect();

			float minSize = Mathf.Min(rect.width,position.height-rect.yMin);

			rect.width = minSize;
			rect.height = minSize;

			Handles.BeginGUI();

			DrawGrid(rect,20.0f);

			if(m_EaseType != EaseType.Linear)
			{
				DrawGraphLine(rect,EaseType.Linear,Color.blue);
			}

			DrawGraphLine(rect,m_EaseType,Color.white);

			Handles.EndGUI();
		}

		void DrawGrid(Rect _rect,float _size)
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

		void DrawGraphLine(Rect _rect,EaseType _type,Color _color)
		{
			Handles.color = _color;

			var pointList = new List<Vector3>();
			var curve = CommonUtility.GetEaseCurve(_type);

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