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
		public string MinText { get; }
		public string MaxText { get; }

		public KZClampAttribute(int minValue,int maxValue)				: this(minValue.ToString(),maxValue.ToString()) { }
		public KZClampAttribute(long minValue,long maxValue)			: this(minValue.ToString(),maxValue.ToString()) { }
		public KZClampAttribute(float minValue,float maxValue)			: this(minValue.ToString(),maxValue.ToString()) { }
		public KZClampAttribute(double minValue,double maxValue)		: this(minValue.ToString(),maxValue.ToString()) { }

		public KZClampAttribute(int minValue,string maxExpression)		: this(minValue.ToString(),maxExpression) 		{ }
		public KZClampAttribute(long minValue,string maxExpression)		: this(minValue.ToString(),maxExpression) 		{ }
		public KZClampAttribute(float minValue,string maxExpression)	: this(minValue.ToString(),maxExpression) 		{ }
		public KZClampAttribute(double minValue,string maxExpression)	: this(minValue.ToString(),maxExpression) 		{ }

		public KZClampAttribute(string minExpression,int maxValue)		: this(minExpression,maxValue.ToString()) 		{ }
		public KZClampAttribute(string minExpression,long maxValue)		: this(minExpression,maxValue.ToString()) 		{ }
		public KZClampAttribute(string minExpression,float maxValue)	: this(minExpression,maxValue.ToString()) 		{ }
		public KZClampAttribute(string minExpression,double maxValue)	: this(minExpression,maxValue.ToString()) 		{ }

		protected KZClampAttribute(string minText,string maxText)
		{
			MinText = minText;
			MaxText = maxText;
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZMaxClampAttribute : KZClampAttribute
	{
		public KZMaxClampAttribute(int maxValue)			: base(int.MinValue.ToString(),maxValue.ToString())		{ }
		public KZMaxClampAttribute(long maxValue)			: base(long.MinValue.ToString(),maxValue.ToString())	{ }
		public KZMaxClampAttribute(float maxValue)			: base(float.MinValue.ToString(),maxValue.ToString())	{ }
		public KZMaxClampAttribute(double maxValue)			: base(double.MinValue.ToString(),maxValue.ToString())	{ }
		public KZMaxClampAttribute(string maxExpression)	: base(double.MinValue.ToString(),maxExpression)		{ }
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZMinClampAttribute : KZClampAttribute
	{
		public KZMinClampAttribute(int minValue)			: base(minValue.ToString(),int.MaxValue.ToString())		{ }
		public KZMinClampAttribute(long minValue)			: base(minValue.ToString(),long.MaxValue.ToString())	{ }
		public KZMinClampAttribute(float minValue)			: base(minValue.ToString(),float.MaxValue.ToString())	{ }
		public KZMinClampAttribute(double minValue)			: base(minValue.ToString(),double.MaxValue.ToString())	{ }
		public KZMinClampAttribute(string minExpression)	: base(minExpression,double.MaxValue.ToString())		{ }
	}

#if UNITY_EDITOR
	public abstract class KZBaseClampAttributeDrawer<TAttribute,TValue> : KZAttributeDrawer<TAttribute,TValue> where TAttribute : KZClampAttribute where TValue : IComparable<TValue>
	{
		protected TValue m_minValue;
		protected TValue m_maxValue;

		protected override void Initialize()
		{
			base.Initialize();

			m_minValue = TryConvertTo(Attribute.MinText,out var minValue) ? minValue : _FindValue<TValue>(Attribute.MinText);
			m_maxValue = TryConvertTo(Attribute.MaxText,out var maxValue) ? maxValue : _FindValue<TValue>(Attribute.MaxText);
		}

		protected override void _DoDrawPropertyLayout(GUIContent label)
		{
			var rect = EditorGUILayout.GetControlRect();
			var current = DrawField(rect,_GetLabelText(label));

			ValueEntry.SmartValue = CommonUtility.Clamp(current,m_minValue,m_maxValue);
		}

		protected abstract TValue DrawField(Rect rect,string label);
		protected abstract bool TryConvertTo(string text,out TValue result);
	}

	public abstract class KZBaseClampIntAttributeDrawer<TAttribute> : KZBaseClampAttributeDrawer<TAttribute,int> where TAttribute : KZClampAttribute
	{
		protected override int DrawField(Rect rect,string label)
		{
			return EditorGUI.IntField(rect,label,ValueEntry.SmartValue);
		}

		protected override bool TryConvertTo(string text,out int result)
		{
			return int.TryParse(text,out result);
		}
	}

	public abstract class KZBaseClampLongAttributeDrawer<TAttribute> : KZBaseClampAttributeDrawer<TAttribute,long> where TAttribute : KZClampAttribute
	{
		protected override long DrawField(Rect rect,string label)
		{
			return EditorGUI.LongField(rect,label,ValueEntry.SmartValue);
		}

		protected override bool TryConvertTo(string text,out long result)
		{
			return long.TryParse(text,out result);
		}
	}

	public abstract class KZBaseClampFloatAttributeDrawer<TAttribute> : KZBaseClampAttributeDrawer<TAttribute,float> where TAttribute : KZClampAttribute
	{
		protected override float DrawField(Rect rect,string label)
		{
			return EditorGUI.FloatField(rect,label,ValueEntry.SmartValue);
		}

		protected override bool TryConvertTo(string text,out float result)
		{
			return float.TryParse(text,out result);
		}
	}

	public abstract class KZBaseClampDoubleAttributeDrawer<TAttribute> : KZBaseClampAttributeDrawer<TAttribute,double> where TAttribute : KZClampAttribute
	{
		protected override double DrawField(Rect rect,string label)
		{
			return EditorGUI.DoubleField(rect,label,ValueEntry.SmartValue);
		}

		protected override bool TryConvertTo(string text,out double result)
		{
			return double.TryParse(text,out result);
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