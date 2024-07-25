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
		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			var rect = EditorGUILayout.GetControlRect();

			var labelText = _label == null ? string.Empty : _label.text;

			var value = DrawField(rect,labelText);
			var minimum = Attribute.MinExpression.IsEmpty() ? MinValue : GetValue<TValue>(Attribute.MinExpression);

			ValueEntry.SmartValue = CommonUtility.MinClamp(value,minimum);
		}

		protected abstract TValue MinValue { get; }

		protected abstract TValue DrawField(Rect _rect, string _labelText);
	}

	public class KZMinIntAttributeDrawer : KZMinClampAttributeDrawer<int>
	{
		protected override int MinValue => Convert.ToInt32(Attribute.MinValue);

		protected override int DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.IntField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZMinLongAttributeDrawer : KZMinClampAttributeDrawer<long>
	{
		protected override long MinValue => Convert.ToInt64(Attribute.MinValue);

		protected override long DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.LongField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZMinFloatAttributeDrawer : KZMinClampAttributeDrawer<float>
	{
		protected override float MinValue => Convert.ToSingle(Attribute.MinValue);

		protected override float DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.FloatField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZMinDoubleAttributeDrawer : KZMinClampAttributeDrawer<double>
	{
		protected override double MinValue => Attribute.MinValue;

		protected override double DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.DoubleField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}
#endif
}