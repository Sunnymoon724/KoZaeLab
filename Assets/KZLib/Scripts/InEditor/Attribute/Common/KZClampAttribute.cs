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

		public KZClampAttribute(double minValue,double maxValue) : this(minValue,null,maxValue,null) { }
		public KZClampAttribute(string minExpression,string maxExpression) : this(double.MinValue,minExpression,double.MaxValue,maxExpression) { }

		protected KZClampAttribute(double minValue,string minExpression,double maxValue,string maxExpression)
		{
			MinValue = minValue;
			MinExpression = minExpression;

			MaxValue = maxValue;
			MaxExpression = maxExpression;
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZMaxClampAttribute : KZClampAttribute
	{
		public KZMaxClampAttribute(double maxValue) : base(double.MinValue,null,maxValue,null) { }
		public KZMaxClampAttribute(string maxExpression) : base(double.MinValue,null,double.MaxValue,maxExpression) { }
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZMinClampAttribute : KZClampAttribute
	{
		public KZMinClampAttribute(double minValue) : base(minValue,null,double.MaxValue,null) { }
		public KZMinClampAttribute(string minExpression) : base(double.MinValue,minExpression,double.MaxValue,null) { }
	}

#if UNITY_EDITOR
	public abstract class KZBaseClampAttributeDrawer<TAttribute,TValue> : KZAttributeDrawer<TAttribute,TValue> where TAttribute : KZClampAttribute where TValue : IComparable<TValue>
	{
		protected TValue m_minValue;
		protected TValue m_maxValue;

		protected override void Initialize()
		{
			base.Initialize();

			m_minValue = Attribute.MinExpression.IsEmpty() ? (TValue) ConvertToValue(Attribute.MinValue) : FindValue<TValue>(Attribute.MinExpression);
			m_maxValue = Attribute.MaxExpression.IsEmpty() ? (TValue) ConvertToValue(Attribute.MaxValue) : FindValue<TValue>(Attribute.MaxExpression);
		}

		protected override void _DrawPropertyLayout(GUIContent label)
		{
			var rect = EditorGUILayout.GetControlRect();
			var current = DrawField(rect,GetLabelText(label));

			ValueEntry.SmartValue = CommonUtility.Clamp(current,m_minValue,m_maxValue);
		}

		protected abstract TValue DrawField(Rect rect,string label);
	}

	public abstract class KZBaseClampIntAttributeDrawer<TAttribute> : KZBaseClampAttributeDrawer<TAttribute,int> where TAttribute : KZClampAttribute
	{
		protected override int DrawField(Rect rect,string label)
		{
			return EditorGUI.IntField(rect,label,ValueEntry.SmartValue);
		}
	}

	public abstract class KZBaseClampLongAttributeDrawer<TAttribute> : KZBaseClampAttributeDrawer<TAttribute,long> where TAttribute : KZClampAttribute
	{
		protected override long DrawField(Rect rect,string label)
		{
			return EditorGUI.LongField(rect,label,ValueEntry.SmartValue);
		}
	}

	public abstract class KZBaseClampFloatAttributeDrawer<TAttribute> : KZBaseClampAttributeDrawer<TAttribute,float> where TAttribute : KZClampAttribute
	{
		protected override float DrawField(Rect rect,string label)
		{
			return EditorGUI.FloatField(rect,label,ValueEntry.SmartValue);
		}
	}

	public abstract class KZBaseClampDoubleAttributeDrawer<TAttribute> : KZBaseClampAttributeDrawer<TAttribute,double> where TAttribute : KZClampAttribute
	{
		protected override double DrawField(Rect rect,string label)
		{
			return EditorGUI.DoubleField(rect,label,ValueEntry.SmartValue);
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