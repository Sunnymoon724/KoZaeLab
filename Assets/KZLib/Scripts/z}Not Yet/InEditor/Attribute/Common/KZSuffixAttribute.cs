using System;
using UnityEngine;
using System.Diagnostics;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib.KZAttribute
{
	/// <summary>
	/// 접미사를 표현하는 용도
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZSuffixAttribute : Attribute
	{
		public string SuffixText { get; }

		public KZSuffixAttribute(string _suffix)
		{
			SuffixText = _suffix;
		}
	}

#if UNITY_EDITOR
	public abstract class KZSuffixAttributeDrawer<TValue> : KZAttributeDrawer<KZSuffixAttribute,TValue>
	{
		private const int SPACE = 5;

		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			var rect = EditorGUILayout.GetControlRect();

			var label = new GUIContent(Attribute.SuffixText);
			var style = new GUIStyle(GUI.skin.label);
			var width = style.CalcSize(label).x+SPACE;

			rect.xMax -= width;

			var labelText = _label == null ? string.Empty : _label.text;

			DrawField(rect,labelText);

			rect.xMin = rect.xMax+SPACE;
			rect.xMax += width;

			EditorGUI.LabelField(rect,label,style);
		}

		protected abstract void DrawField(Rect _rect,string _labelText);
	}

	public class KZSuffixStringAttributeDrawer : KZSuffixAttributeDrawer<string>
	{
		protected override void DrawField(Rect _rect,string _labelText)
		{
			ValueEntry.SmartValue = EditorGUI.TextField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZSuffixIntAttributeDrawer : KZSuffixAttributeDrawer<int>
	{
		protected override void DrawField(Rect _rect,string _labelText)
		{
			ValueEntry.SmartValue = EditorGUI.IntField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZSuffixLongAttributeDrawer : KZSuffixAttributeDrawer<long>
	{
		protected override void DrawField(Rect _rect,string _labelText)
		{
			ValueEntry.SmartValue = EditorGUI.LongField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZSuffixFloatAttributeDrawer : KZSuffixAttributeDrawer<float>
	{
		protected override void DrawField(Rect _rect,string _labelText)
		{
			ValueEntry.SmartValue = EditorGUI.FloatField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}

	public class KZSuffixDoubleAttributeDrawer : KZSuffixAttributeDrawer<double>
	{
		protected override void DrawField(Rect _rect,string _labelText)
		{
			ValueEntry.SmartValue = EditorGUI.DoubleField(_rect,_labelText,ValueEntry.SmartValue);
		}
	}
#endif
}