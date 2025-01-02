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

		public KZSuffixAttribute(string suffixText)
		{
			SuffixText = suffixText;
		}
	}

#if UNITY_EDITOR
	public abstract class KZSuffixAttributeDrawer<TValue> : KZAttributeDrawer<KZSuffixAttribute,TValue>
	{
		private const int c_label_space = 5;

		protected override void _DrawPropertyLayout(GUIContent label)
		{
			var rect = EditorGUILayout.GetControlRect();

			var labelContent = new GUIContent(Attribute.SuffixText);
			var width = new GUIStyle(GUI.skin.label).CalcSize(labelContent).x+c_label_space;

			rect.xMax -= width;

			ValueEntry.SmartValue = DrawField(rect,GetLabelText(label));

			rect.xMin = rect.xMax+c_label_space;
			rect.xMax += width;

			EditorGUI.LabelField(rect,labelContent,new GUIStyle(GUI.skin.label));
		}

		protected abstract TValue DrawField(Rect rect,string labelText);
	}

	public class KZSuffixStringAttributeDrawer : KZSuffixAttributeDrawer<string>
	{
		protected override string DrawField(Rect rect,string label)
		{
			return EditorGUI.TextField(rect,label,ValueEntry.SmartValue);
		}
	}

	public class KZSuffixIntAttributeDrawer : KZSuffixAttributeDrawer<int>
	{
		protected override int DrawField(Rect rect,string label)
		{
			return EditorGUI.IntField(rect,label,ValueEntry.SmartValue);
		}
	}

	public class KZSuffixLongAttributeDrawer : KZSuffixAttributeDrawer<long>
	{
		protected override long DrawField(Rect rect,string label)
		{
			return EditorGUI.LongField(rect,label,ValueEntry.SmartValue);
		}
	}

	public class KZSuffixFloatAttributeDrawer : KZSuffixAttributeDrawer<float>
	{
		protected override float DrawField(Rect rect,string label)
		{
			return EditorGUI.FloatField(rect,label,ValueEntry.SmartValue);
		}
	}

	public class KZSuffixDoubleAttributeDrawer : KZSuffixAttributeDrawer<double>
	{
		protected override double DrawField(Rect rect,string label)
		{
			return EditorGUI.DoubleField(rect,label,ValueEntry.SmartValue);
		}
	}
#endif
}