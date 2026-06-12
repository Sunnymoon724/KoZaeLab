#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace KZLib.Attributes
{
	public abstract class KZSuffixAttributeDrawer<TValue> : KZAttributeDrawer<KZSuffixAttribute,TValue>
	{
		private const int c_labelSpace = 5;

		protected override void _DoDrawPropertyLayout(GUIContent label)
		{
			var rect = EditorGUILayout.GetControlRect();

			var labelContent = new GUIContent(Attribute.SuffixText);
			var width = new GUIStyle(GUI.skin.label).CalcSize(labelContent).x+c_labelSpace;

			rect.xMax -= width;

			ValueEntry.SmartValue = DrawField(rect,_GetLabelText(label));

			rect.xMin = rect.xMax+c_labelSpace;
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
}
#endif