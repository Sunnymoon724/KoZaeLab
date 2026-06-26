#if UNITY_EDITOR
using System;
using System.Globalization;
using UnityEngine;
using UnityEditor;

namespace KZLib.Attributes
{
	/// <summary><see cref="KZClampAttribute"/> drawer base. Clamps the value after editing.</summary>
	public abstract class KZBaseClampAttributeDrawer<TAttribute,TValue> : KZAttributeDrawer<TAttribute,TValue> where TAttribute : KZClampAttribute where TValue : IComparable<TValue>
	{
		protected TValue m_minValue;
		protected TValue m_maxValue;

		protected override void Initialize()
		{
			base.Initialize();

			m_minValue = _ResolveBound(Attribute.MinText,true);
			m_maxValue = _ResolveBound(Attribute.MaxText,false);
		}

		/// <summary>
		/// Resolves bounds via literal parse, then member lookup.
		/// Null bounds on <see cref="KZMinClampAttribute"/>/<see cref="KZMaxClampAttribute"/> string overloads fall back to field-type min/max.
		/// </summary>
		private TValue _ResolveBound(string text,bool isMin)
		{
			if(string.IsNullOrEmpty(text))
			{
				return isMin ? GetTypeMin() : GetTypeMax();
			}

			if(TryConvertTo(text,out var value))
			{
				return value;
			}

			return _FindValue<TValue>(text);
		}

		protected override void _DoDrawPropertyLayout(GUIContent label)
		{
			var rect = EditorGUILayout.GetControlRect();
			var current = DrawField(rect,_GetLabelText(label));

			ValueEntry.SmartValue = KZMathKit.Clamp(current,m_minValue,m_maxValue);
		}

		protected abstract TValue DrawField(Rect rect,string label);
		protected abstract bool TryConvertTo(string text,out TValue result);
		protected abstract TValue GetTypeMin();
		protected abstract TValue GetTypeMax();
	}

	/// <summary><see cref="KZClampAttribute"/> / Min / Max — int fields.</summary>
	public abstract class KZBaseClampIntAttributeDrawer<TAttribute> : KZBaseClampAttributeDrawer<TAttribute,int> where TAttribute : KZClampAttribute
	{
		protected override int DrawField(Rect rect,string label) => EditorGUI.IntField(rect,label,ValueEntry.SmartValue);

		protected override bool TryConvertTo(string text,out int result) => int.TryParse(text,NumberStyles.Integer,CultureInfo.InvariantCulture,out result);

		protected override int GetTypeMin() => int.MinValue;
		protected override int GetTypeMax() => int.MaxValue;
	}

	/// <summary><see cref="KZClampAttribute"/> / Min / Max — long fields.</summary>
	public abstract class KZBaseClampLongAttributeDrawer<TAttribute> : KZBaseClampAttributeDrawer<TAttribute,long> where TAttribute : KZClampAttribute
	{
		protected override long DrawField(Rect rect,string label) => EditorGUI.LongField(rect,label,ValueEntry.SmartValue);

		protected override bool TryConvertTo(string text,out long result) => long.TryParse(text,NumberStyles.Integer,CultureInfo.InvariantCulture,out result);

		protected override long GetTypeMin() => long.MinValue;
		protected override long GetTypeMax() => long.MaxValue;
	}

	/// <summary><see cref="KZClampAttribute"/> / Min / Max — float fields.</summary>
	public abstract class KZBaseClampFloatAttributeDrawer<TAttribute> : KZBaseClampAttributeDrawer<TAttribute,float> where TAttribute : KZClampAttribute
	{
		protected override float DrawField(Rect rect,string label) => EditorGUI.FloatField(rect,label,ValueEntry.SmartValue);

		protected override bool TryConvertTo(string text,out float result) => float.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out result);

		protected override float GetTypeMin() => float.MinValue;
		protected override float GetTypeMax() => float.MaxValue;
	}

	/// <summary><see cref="KZClampAttribute"/> / Min / Max — double fields.</summary>
	public abstract class KZBaseClampDoubleAttributeDrawer<TAttribute> : KZBaseClampAttributeDrawer<TAttribute,double> where TAttribute : KZClampAttribute
	{
		protected override double DrawField(Rect rect,string label) => EditorGUI.DoubleField(rect,label,ValueEntry.SmartValue);

		protected override bool TryConvertTo(string text,out double result) => double.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out result);

		protected override double GetTypeMin() => double.MinValue;
		protected override double GetTypeMax() => double.MaxValue;
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
}
#endif