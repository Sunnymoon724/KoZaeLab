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
		private const int LABEL_SPACING = 5;

		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			var rect = EditorGUILayout.GetControlRect();

			var labelContent = new GUIContent(Attribute.SuffixText);
			var width = new GUIStyle(GUI.skin.label).CalcSize(labelContent).x+LABEL_SPACING;

			rect.xMax -= width;

			ValueEntry.SmartValue = DrawField(rect,GetLabelText(_label));

			rect.xMin = rect.xMax+LABEL_SPACING;
			rect.xMax += width;

			EditorGUI.LabelField(rect,labelContent,new GUIStyle(GUI.skin.label));
		}

		protected abstract TValue DrawField(Rect _rect,string _labelText);
	}

	public class KZSuffixStringAttributeDrawer : KZSuffixAttributeDrawer<string>
	{
		protected override string DrawField(Rect _rect,string _label)
		{
			return EditorGUI.TextField(_rect,_label,ValueEntry.SmartValue);
		}
	}

	public class KZSuffixIntAttributeDrawer : KZSuffixAttributeDrawer<int>
	{
		protected override int DrawField(Rect _rect,string _label)
		{
			return EditorGUI.IntField(_rect,_label,ValueEntry.SmartValue);
		}
	}

	public class KZSuffixLongAttributeDrawer : KZSuffixAttributeDrawer<long>
	{
		protected override long DrawField(Rect _rect,string _label)
		{
			return EditorGUI.LongField(_rect,_label,ValueEntry.SmartValue);
		}
	}

	public class KZSuffixFloatAttributeDrawer : KZSuffixAttributeDrawer<float>
	{
		protected override float DrawField(Rect _rect,string _label)
		{
			return EditorGUI.FloatField(_rect,_label,ValueEntry.SmartValue);
		}
	}

	public class KZSuffixDoubleAttributeDrawer : KZSuffixAttributeDrawer<double>
	{
		protected override double DrawField(Rect _rect,string _label)
		{
			return EditorGUI.DoubleField(_rect,_label,ValueEntry.SmartValue);
		}
	}
#endif
}