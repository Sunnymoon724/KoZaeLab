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

		public KZClampAttribute(double _minValue,double _maxValue) : this(_minValue,null,_maxValue,null) { }
		public KZClampAttribute(string _minExpression,string _maxExpression) : this(double.MinValue,_minExpression,double.MaxValue,_maxExpression) { }

		protected KZClampAttribute(double _minValue,string _minExpression,double _maxValue,string _maxExpression)
		{
			MinValue = _minValue;
			MinExpression = _minExpression;

			MaxValue = _maxValue;
			MaxExpression = _maxExpression;
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZMaxClampAttribute : KZClampAttribute
	{
		public KZMaxClampAttribute(double _maxValue) : base(double.MinValue,null,_maxValue,null) { }
		public KZMaxClampAttribute(string _maxExpression) : base(double.MinValue,null,double.MaxValue,_maxExpression) { }
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZMinClampAttribute : KZClampAttribute
	{
		public KZMinClampAttribute(double _minValue) : base(_minValue,null,double.MaxValue,null) { }
		public KZMinClampAttribute(string _minExpression) : base(double.MinValue,_minExpression,double.MaxValue,null) { }
	}

#if UNITY_EDITOR
	public abstract class KZBaseClampAttributeDrawer<TAttribute,TValue> : KZAttributeDrawer<TAttribute,TValue> where TAttribute : KZClampAttribute where TValue : IComparable<TValue>
	{
		protected TValue m_MinValue;
		protected TValue m_MaxValue;

		protected override void Initialize()
		{
			base.Initialize();

			m_MinValue = Attribute.MinExpression.IsEmpty() ? (TValue) ConvertToValue(Attribute.MinValue) : GetValue<TValue>(Attribute.MinExpression);
			m_MaxValue = Attribute.MaxExpression.IsEmpty() ? (TValue) ConvertToValue(Attribute.MaxValue) : GetValue<TValue>(Attribute.MaxExpression);
		}

		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			var rect = EditorGUILayout.GetControlRect();
			var current = DrawField(rect,GetLabelText(_label));

			ValueEntry.SmartValue = MathUtility.Clamp(current,m_MinValue,m_MaxValue);
		}

		protected abstract TValue DrawField(Rect _rect,string _label);
	}

	public abstract class KZBaseClampIntAttributeDrawer<TAttribute> : KZBaseClampAttributeDrawer<TAttribute,int> where TAttribute : KZClampAttribute
	{
		protected override int DrawField(Rect _rect,string _label)
		{
			return EditorGUI.IntField(_rect,_label,ValueEntry.SmartValue);
		}
	}

	public abstract class KZBaseClampLongAttributeDrawer<TAttribute> : KZBaseClampAttributeDrawer<TAttribute,long> where TAttribute : KZClampAttribute
	{
		protected override long DrawField(Rect _rect,string _label)
		{
			return EditorGUI.LongField(_rect,_label,ValueEntry.SmartValue);
		}
	}

	public abstract class KZBaseClampFloatAttributeDrawer<TAttribute> : KZBaseClampAttributeDrawer<TAttribute,float> where TAttribute : KZClampAttribute
	{
		protected override float DrawField(Rect _rect,string _label)
		{
			return EditorGUI.FloatField(_rect,_label,ValueEntry.SmartValue);
		}
	}

	public abstract class KZBaseClampDoubleAttributeDrawer<TAttribute> : KZBaseClampAttributeDrawer<TAttribute,double> where TAttribute : KZClampAttribute
	{
		protected override double DrawField(Rect _rect,string _label)
		{
			return EditorGUI.DoubleField(_rect,_label,ValueEntry.SmartValue);
		}
	}

	public class KZClampIntAttributeDrawer : KZBaseClampIntAttributeDrawer<KZClampAttribute> { }
	public class KZMinClampIntAttributeDrawer : KZBaseClampIntAttributeDrawer<KZMinClampAttribute> { }
	public class KZMaxClampIntAttributeDrawer : KZBaseClampIntAttributeDrawer<KZMaxClampAttribute> { }

	public class KZClampLongAttributeDrawer : KZBaseClampLongAttributeDrawer<KZClampAttribute> { }
	public class KZMinClampLongAttributeDrawer : KZBaseClampLongAttributeDrawer<KZMinClampAttribute> { }
	public class KZMaxClampLongAttributeDrawer : KZBaseClampLongAttributeDrawer<KZMaxClampAttribute> { }

	public class KZClampFloatAttributeDrawer : KZBaseClampFloatAttributeDrawer<KZClampAttribute> { }
	public class KZMinClampFloatAttributeDrawer : KZBaseClampFloatAttributeDrawer<KZMinClampAttribute> { }
	public class KZMaxClampFloatAttributeDrawer : KZBaseClampFloatAttributeDrawer<KZMaxClampAttribute> { }

	public class KZClampDoubleAttributeDrawer : KZBaseClampDoubleAttributeDrawer<KZClampAttribute> { }
	public class KZMinClampDoubleAttributeDrawer : KZBaseClampDoubleAttributeDrawer<KZMinClampAttribute> { }
	public class KZMaxClampDoubleAttributeDrawer : KZBaseClampDoubleAttributeDrawer<KZMaxClampAttribute> { }
#endif
}