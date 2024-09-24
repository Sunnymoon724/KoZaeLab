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
			var rect = DrawPrefixLabel(_label);
			var countRect = new Rect(rect.x,rect.y,LIST_WIDTH,rect.height);

			m_ListCount = MathUtility.MinClamp(EditorGUI.IntField(countRect,"",m_ListCount),0);

			if(m_ListCount <= 0)
			{
				return;
			}

			DrawField(new Rect(rect.x+50,rect.y,rect.width-50,rect.height));
		}

		protected void AdjustList<TData>(List<TData> _dataList,TData _data,int _count)
		{
			if(_count < m_ListCount)
			{
				_dataList.AddCount(_data,m_ListCount-_count);
			}
			else if(_count > m_ListCount)
			{
				_dataList.RemoveRange(m_ListCount,_count-m_ListCount);
			}
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
			AdjustList(ValueEntry.SmartValue,0,ValueEntry.SmartValue.Count);

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
			AdjustList(ValueEntry.SmartValue,0.0f,ValueEntry.SmartValue.Count);

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
			AdjustList(ValueEntry.SmartValue,"",ValueEntry.SmartValue.Count);

			var rectArray = GetRectArray(_rect,m_ListCount,5.0f);

			for(var i=0;i<m_ListCount;i++)
			{
				ValueEntry.SmartValue[i] = EditorGUI.TextField(rectArray[i],ValueEntry.SmartValue[i]);
			}
		}
	}
#endif
}