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
		private const float c_width = 30.0f;
		protected const float c_space = 5.0f;

		protected int m_listCount = 0;

		protected override void _DrawPropertyLayout(GUIContent label)
		{
			var rect = DrawPrefixLabel(label);
			var countRect = new Rect(rect.x,rect.y,c_width,rect.height);

			m_listCount = CommonUtility.MinClamp(EditorGUI.IntField(countRect,"",m_listCount),0);

			if(m_listCount <= 0)
			{
				return;
			}

			DrawField(new Rect(rect.x+50,rect.y,rect.width-50,rect.height));
		}

		protected void AdjustList<TData>(List<TData> dataList,TData data,int count)
		{
			if(count < m_listCount)
			{
				dataList.AddCount(data,m_listCount-count);
			}
			else if(count > m_listCount)
			{
				dataList.RemoveRange(m_listCount,count-m_listCount);
			}
		}

		protected abstract void DrawField(Rect rect);
	}

	public class KZIntListAttributeDrawer : KZListAttributeDrawer<List<int>>
	{
		protected override void Initialize()
		{
			if(ValueEntry.SmartValue == null)
			{
				ValueEntry.SmartValue = new List<int>();
			}

			m_listCount = ValueEntry.SmartValue.Count;
		}

		protected override void DrawField(Rect rect)
		{
			AdjustList(ValueEntry.SmartValue,0,ValueEntry.SmartValue.Count);

			var rectArray = GetRectArray(rect,m_listCount,c_space);

			for(var i=0;i<m_listCount;i++)
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

			m_listCount = ValueEntry.SmartValue.Count;
		}

		protected override void DrawField(Rect rect)
		{
			AdjustList(ValueEntry.SmartValue,0.0f,ValueEntry.SmartValue.Count);

			var rectArray = GetRectArray(rect,m_listCount,c_space);

			for(var i=0;i<m_listCount;i++)
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

			m_listCount = ValueEntry.SmartValue.Count;
		}

		protected override void DrawField(Rect rect)
		{
			AdjustList(ValueEntry.SmartValue,string.Empty,ValueEntry.SmartValue.Count);

			var rectArray = GetRectArray(rect,m_listCount,c_space);

			for(var i=0;i<m_listCount;i++)
			{
				ValueEntry.SmartValue[i] = EditorGUI.TextField(rectArray[i],ValueEntry.SmartValue[i]);
			}
		}
	}
#endif
}