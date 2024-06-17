using System;
using UnityEngine;
using System.Diagnostics;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZMinClampAttribute : Attribute
	{
		public double MinValue { get; }
		public string MinExpression { get; }

		public KZMinClampAttribute(double _minValue)
		{
			MinValue = _minValue;
		}

		public KZMinClampAttribute(string _minExpression)
		{
			MinExpression = _minExpression;
		}
	}

#if UNITY_EDITOR
	public abstract class KZMinClampAttributeDrawer<TValue> : KZAttributeDrawer<KZMinClampAttribute, TValue> where TValue : IComparable<TValue>
	{
		protected TValue m_MinValue = default;

		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			var rect = EditorGUILayout.GetControlRect();

			var labelText = _label == null ? string.Empty : _label.text;

			var value = DrawField(rect,labelText);

			ValueEntry.SmartValue = CommonUtility.MinClamp(value,m_MinValue);
		}

		protected abstract TValue DrawField(Rect _rect, string _labelText);
	}

	public class KZMinIntAttributeDrawer : KZMinClampAttributeDrawer<int>
	{
		protected override void Initialize()
		{
			m_MinValue = Attribute.MinExpression.IsEmpty() ? (int) Attribute.MinValue : GetValue<int>(Attribute.MinExpression);
		}

		protected override int DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.IntField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZMinLongAttributeDrawer : KZMinClampAttributeDrawer<long>
	{
		protected override void Initialize()
		{
			m_MinValue = Attribute.MinExpression.IsEmpty() ? (long) Attribute.MinValue : GetValue<long>(Attribute.MinExpression);
		}

		protected override long DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.LongField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZMinFloatAttributeDrawer : KZMinClampAttributeDrawer<float>
	{
		protected override void Initialize()
		{
			m_MinValue = Attribute.MinExpression.IsEmpty() ? (float) Attribute.MinValue : GetValue<float>(Attribute.MinExpression);
		}

		protected override float DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.FloatField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZMinDoubleAttributeDrawer : KZMinClampAttributeDrawer<double>
	{
		protected override void Initialize()
		{
			m_MinValue = Attribute.MinExpression.IsEmpty() ? Attribute.MinValue : GetValue<double>(Attribute.MinExpression);
		}

		protected override double DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.DoubleField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}
#endif
}