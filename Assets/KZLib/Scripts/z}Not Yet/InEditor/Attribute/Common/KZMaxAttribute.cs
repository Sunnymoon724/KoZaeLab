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
	public class KZMaxClampAttribute : Attribute
	{
		public double MaxValue { get; }
		public string MaxExpression { get; }

		public KZMaxClampAttribute(double _maxValue)
		{
			MaxValue = _maxValue;
		}

		public KZMaxClampAttribute(string _maxExpression)
		{
			MaxExpression = _maxExpression;
		}
	}

#if UNITY_EDITOR
	public abstract class KZMaxClampAttributeDrawer<TValue> : KZAttributeDrawer<KZMaxClampAttribute,TValue> where TValue : IComparable<TValue>
	{
		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			var rect = EditorGUILayout.GetControlRect();

			var labelText = _label == null ? string.Empty : _label.text;

			var value = DrawField(rect,labelText);
			var maximum = Attribute.MaxExpression.IsEmpty() ? MaxValue : GetValue<TValue>(Attribute.MaxExpression);

			ValueEntry.SmartValue = CommonUtility.MaxClamp(value,maximum);
		}

		protected abstract TValue MaxValue { get; }

		protected abstract TValue DrawField(Rect _rect,string _labelText);
	}

	public class KZMaxIntAttributeDrawer : KZMaxClampAttributeDrawer<int>
	{
		protected override int MaxValue => Convert.ToInt32(Attribute.MaxValue);

		protected override int DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.IntField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZMaxLongAttributeDrawer : KZMaxClampAttributeDrawer<long>
	{
		protected override long MaxValue => Convert.ToInt64(Attribute.MaxValue);

		protected override long DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.LongField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZMaxFloatAttributeDrawer : KZMaxClampAttributeDrawer<float>
	{
		protected override float MaxValue => Convert.ToSingle(Attribute.MaxValue);

		protected override float DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.FloatField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZMaxDoubleAttributeDrawer : KZMaxClampAttributeDrawer<double>
	{
		protected override double MaxValue => Attribute.MaxValue;

		protected override double DrawField(Rect _rect,string _labelText)
		{
			return EditorGUI.DoubleField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}
#endif
}