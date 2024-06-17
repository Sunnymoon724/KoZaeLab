using System;
using UnityEngine;
using System.Diagnostics;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZClampAttribute : Attribute
	{
		public double MinValue { get; }
		public double MaxValue { get; }
		public string MinExpression { get; }
		public string MaxExpression { get; }

		public KZClampAttribute(double _minValue,double _maxValue)
		{
			MinValue = _minValue;
			MaxValue = _maxValue;
		}

		public KZClampAttribute(string _minExpression,string _maxExpression)
		{
			MinExpression = _minExpression;
			MaxExpression = _maxExpression;
		}
	}

#if UNITY_EDITOR
	public abstract class KZClampAttributeDrawer<TValue> : KZAttributeDrawer<KZClampAttribute,TValue> where TValue : IComparable<TValue>
	{
		protected TValue m_MinValue = default;
		protected TValue m_MaxValue = default;

		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			var rect = EditorGUILayout.GetControlRect();

			var labelText = _label == null ? string.Empty : _label.text;

			var value = DrawField(rect,labelText);

			ValueEntry.SmartValue = CommonUtility.Clamp(value,m_MinValue,m_MaxValue);
		}

		protected abstract TValue DrawField(Rect _rect,string _labelText);
	}

	public class KZClampIntAttributeDrawer : KZClampAttributeDrawer<int>
	{
		protected override void Initialize()
		{
			m_MinValue = Attribute.MinExpression.IsEmpty() ? (int) Attribute.MinValue : GetValue<int>(Attribute.MinExpression);

			m_MaxValue = Attribute.MaxExpression.IsEmpty() ? (int) Attribute.MaxValue : GetValue<int>(Attribute.MaxExpression);
		}

		protected override int DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.IntField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZClampLongAttributeDrawer : KZClampAttributeDrawer<long>
	{
		protected override void Initialize()
		{
			m_MinValue = Attribute.MinExpression.IsEmpty() ? (long) Attribute.MinValue : GetValue<long>(Attribute.MinExpression);

			m_MaxValue = Attribute.MaxExpression.IsEmpty() ? (long) Attribute.MaxValue : GetValue<long>(Attribute.MaxExpression);
		}

		protected override long DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.LongField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZClampFloatAttributeDrawer : KZClampAttributeDrawer<float>
	{
		protected override void Initialize()
		{
			m_MinValue = Attribute.MinExpression.IsEmpty() ? (float) Attribute.MinValue : GetValue<float>(Attribute.MinExpression);

			m_MaxValue = Attribute.MaxExpression.IsEmpty() ? (float) Attribute.MaxValue : GetValue<float>(Attribute.MaxExpression);
		}

		protected override float DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.FloatField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZClampDoubleAttributeDrawer : KZClampAttributeDrawer<double>
	{
		protected override void Initialize()
		{
			m_MinValue = Attribute.MinExpression.IsEmpty() ? Attribute.MinValue : GetValue<double>(Attribute.MinExpression);

			m_MaxValue = Attribute.MaxExpression.IsEmpty() ? Attribute.MaxValue : GetValue<double>(Attribute.MaxExpression);
		}

		protected override double DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.DoubleField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}
#endif
}