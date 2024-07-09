using System;
using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZListAttribute : Attribute { }

#if UNITY_EDITOR
	public abstract class KZListAttributeDrawer<TValue> : KZAttributeDrawer<KZListAttribute,TValue>
	{
		private const float LIST_WIDTH = 30.0f;

		protected int m_ListCount = 0;

		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			//? label
			var rect = DrawPrefixLabel(_label);
			var sizeRect = new Rect(rect.x,rect.y,LIST_WIDTH,rect.height);

			var count = EditorGUI.IntField(sizeRect,"",m_ListCount);

			m_ListCount = Mathf.Clamp(count,0,int.MaxValue);

			if(m_ListCount <= 0)
			{
				return;
			}

			DrawField(new Rect(rect.x+50,rect.y,rect.width-50,rect.height));
		}

		protected abstract void DrawField(Rect _rect);
	}

	public class KZIntListAttributeDrawer : KZListAttributeDrawer<List<int>>
	{
		protected override void Initialize()
		{
			if(ValueEntry.SmartValue == null)
			{
				ValueEntry.SmartValue = new List<int>();
			}

			m_ListCount = ValueEntry.SmartValue.Count;
		}

		protected override void DrawField(Rect _rect)
		{
			var count = ValueEntry.SmartValue.Count;

			if(count < m_ListCount)
			{
				ValueEntry.SmartValue.AddCount(0,m_ListCount-count);
			}
			else if(count > m_ListCount)
			{
				ValueEntry.SmartValue.RemoveRange(m_ListCount,count-m_ListCount);
			}

			var rectArray = GetRectArray(_rect,m_ListCount,5.0f);

			for(var i=0;i<m_ListCount;i++)
			{
				ValueEntry.SmartValue[i] = EditorGUI.IntField(rectArray[i],ValueEntry.SmartValue[i]);
			}
		}
	}

	public class KZFloatListAttributeDrawer : KZListAttributeDrawer<List<float>>
	{
		protected override void Initialize()
		{
			if(ValueEntry.SmartValue == null)
			{
				ValueEntry.SmartValue = new List<float>();
			}

			m_ListCount = ValueEntry.SmartValue.Count;
		}

		protected override void DrawField(Rect _rect)
		{
			var count = ValueEntry.SmartValue.Count;

			if(count < m_ListCount)
			{
				ValueEntry.SmartValue.AddCount(0,m_ListCount-count);
			}
			else if(count > m_ListCount)
			{
				ValueEntry.SmartValue.RemoveRange(m_ListCount,count-m_ListCount);
			}

			var rectArray = GetRectArray(_rect,m_ListCount,5.0f);

			for(var i=0;i<m_ListCount;i++)
			{
				ValueEntry.SmartValue[i] = EditorGUI.FloatField(rectArray[i],ValueEntry.SmartValue[i]);
			}
		}
	}

	public class KZStringListAttributeDrawer : KZListAttributeDrawer<List<string>>
	{
		protected override void Initialize()
		{
			if(ValueEntry.SmartValue == null)
			{
				ValueEntry.SmartValue = new List<string>();
			}

			m_ListCount = ValueEntry.SmartValue.Count;
		}

		protected override void DrawField(Rect _rect)
		{
			var count = ValueEntry.SmartValue.Count;

			if(count < m_ListCount)
			{
				ValueEntry.SmartValue.AddCount("",m_ListCount-count);
			}
			else if(count > m_ListCount)
			{
				ValueEntry.SmartValue.RemoveRange(m_ListCount,count-m_ListCount);
			}

			var rectArray = GetRectArray(_rect,m_ListCount,5.0f);

			for(var i=0;i<m_ListCount;i++)
			{
				ValueEntry.SmartValue[i] = EditorGUI.TextField(rectArray[i],ValueEntry.SmartValue[i]);
			}
		}
	}
#endif
}