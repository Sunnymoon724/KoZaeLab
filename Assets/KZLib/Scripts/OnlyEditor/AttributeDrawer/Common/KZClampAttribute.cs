#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace KZLib.Attributes
{
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

			ValueEntry.SmartValue = KZMathKit.Clamp(current,m_minValue,m_maxValue);
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
}
#endif
