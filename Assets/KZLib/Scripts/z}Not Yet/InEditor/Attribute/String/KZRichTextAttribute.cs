using System;
using UnityEngine;
using System.Diagnostics;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.All,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZRichTextAttribute : Attribute { }

#if UNITY_EDITOR
	public abstract class KZRichTextAttributeDrawer<TValue> : KZAttributeDrawer<KZRichTextAttribute,TValue>
	{
		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			var rect = DrawPrefixLabel(_label);

			EditorGUI.LabelField(rect,Label,new GUIStyle(GUI.skin.label) { richText = true, });
		}

		protected abstract string Label { get; }
	}

	public class KZRichTextStringAttributeDrawer : KZRichTextAttributeDrawer<string>
	{
		protected override string Label
		{
			get
			{
				var text = ValueEntry.SmartValue.IsEmpty() ? string.Empty : ValueEntry.SmartValue;

				if(text.Contains(Environment.NewLine))
				{
					text = text.Replace(Environment.NewLine,"\\n");
				}

				return text;
			}
		}
	}

	public class KZRichTextIntAttributeDrawer : KZRichTextAttributeDrawer<int>
	{
		protected override string Label => ValueEntry.SmartValue.ToString();
	}

	public class KZRichTextFloatAttributeDrawer : KZRichTextAttributeDrawer<float>
	{
		protected override string Label => ValueEntry.SmartValue.ToString();
	}

	public class KZRichTextBoolAttributeDrawer : KZRichTextAttributeDrawer<bool>
	{
		protected override string Label => ValueEntry.SmartValue.ToString();
	}

	public class KZRichTextVector2AttributeDrawer : KZRichTextAttributeDrawer<Vector2>
	{
		protected override string Label => $"x : {ValueEntry.SmartValue.x:F3} / y : {ValueEntry.SmartValue.y:F3}";
	}

	public class KZRichTextVector3AttributeDrawer : KZRichTextAttributeDrawer<Vector3>
	{
		protected override string Label => $"{ValueEntry.SmartValue.x:F3} {ValueEntry.SmartValue.y:F3} {ValueEntry.SmartValue.z:F3}";
	}

	public class KZRichTextVector4IntAttributeDrawer : KZRichTextAttributeDrawer<Vector4>
	{
		protected override string Label => $"{ValueEntry.SmartValue.x} {ValueEntry.SmartValue.y} {ValueEntry.SmartValue.z}";
	}

	public class KZRichTextVector2IntAttributeDrawer : KZRichTextAttributeDrawer<Vector2Int>
	{
		protected override string Label => $"x : {ValueEntry.SmartValue.x} / y : {ValueEntry.SmartValue.y}";
	}

	public class KZRichTextVector3IntAttributeDrawer : KZRichTextAttributeDrawer<Vector3Int>
	{
		protected override string Label => $"{ValueEntry.SmartValue.x} {ValueEntry.SmartValue.y} {ValueEntry.SmartValue.z}";
	}
#endif
}