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
	public class KZRichTextStringAttributeDrawer : KZAttributeDrawer<KZRichTextAttribute,string>
	{
		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			//? label
			var rect = DrawPrefixLabel(_label);

			//? 텍스트
			EditorGUI.LabelField(rect,ValueEntry.SmartValue,new GUIStyle(GUI.skin.label)
			{
				richText = true,
			});
		}
	}

	public class KZRichTextIntAttributeDrawer : KZAttributeDrawer<KZRichTextAttribute,int>
	{
		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			//? label
			var rect = DrawPrefixLabel(_label);

			//? 텍스트
			EditorGUI.LabelField(rect,ValueEntry.SmartValue.ToString(),new GUIStyle(GUI.skin.label)
			{
				richText = true,
			});
		}
	}

	public class KZRichTextFloatAttributeDrawer : KZAttributeDrawer<KZRichTextAttribute,float>
	{
		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			//? label
			var rect = DrawPrefixLabel(_label);

			//? 텍스트
			EditorGUI.LabelField(rect,ValueEntry.SmartValue.ToString(),new GUIStyle(GUI.skin.label)
			{
				richText = true,
			});
		}
	}

	public class KZRichTextBoolAttributeDrawer : KZAttributeDrawer<KZRichTextAttribute,bool>
	{
		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			//? label
			var rect = DrawPrefixLabel(_label);

			//? 텍스트
			EditorGUI.LabelField(rect,ValueEntry.SmartValue.ToString(),new GUIStyle(GUI.skin.label)
			{
				richText = true,
			});
		}
	}
#endif
}