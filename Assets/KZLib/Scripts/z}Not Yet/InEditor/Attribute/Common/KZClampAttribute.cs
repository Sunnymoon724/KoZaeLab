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
		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			var rect = EditorGUILayout.GetControlRect();

			var labelText = _label == null ? string.Empty : _label.text;

			var value = DrawField(rect,labelText);
			var minimum = Attribute.MinExpression.IsEmpty() ? MinValue : GetValue<TValue>(Attribute.MinExpression);
			var maximum = Attribute.MaxExpression.IsEmpty() ? MaxValue : GetValue<TValue>(Attribute.MaxExpression);

			ValueEntry.SmartValue = CommonUtility.Clamp(value,minimum,maximum);
		}

		protected abstract TValue MinValue { get; }
		protected abstract TValue MaxValue { get; }

		protected abstract TValue DrawField(Rect _rect,string _labelText);
	}

	public class KZClampIntAttributeDrawer : KZClampAttributeDrawer<int>
	{
		protected override int MinValue => Convert.ToInt32(Attribute.MinValue);
		protected override int MaxValue => Convert.ToInt32(Attribute.MaxValue);

		protected override int DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.IntField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZClampLongAttributeDrawer : KZClampAttributeDrawer<long>
	{
		protected override long MinValue => Convert.ToInt64(Attribute.MinValue);
		protected override long MaxValue => Convert.ToInt64(Attribute.MaxValue);

		protected override long DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.LongField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZClampFloatAttributeDrawer : KZClampAttributeDrawer<float>
	{
		protected override float MinValue => Convert.ToSingle(Attribute.MinValue);
		protected override float MaxValue => Convert.ToSingle(Attribute.MaxValue);

		protected override float DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.FloatField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZClampDoubleAttributeDrawer : KZClampAttributeDrawer<double>
	{
		protected override double MinValue => Attribute.MinValue;
		protected override double MaxValue => Attribute.MaxValue;

		protected override double DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.DoubleField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}
#endif
}