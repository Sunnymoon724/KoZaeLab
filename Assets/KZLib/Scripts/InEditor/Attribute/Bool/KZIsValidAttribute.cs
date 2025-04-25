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
	public class KZIsValidAttribute : Attribute
	{
		public string CorrectText { get; }
		public string WrongText { get; }

		public string WrongHexColor { get; }

		public KZIsValidAttribute(string correctText = "O",string wrongText = "X",string wrongHexColor = null)
		{
			CorrectText = correctText;
			WrongText = wrongText;
			WrongHexColor = wrongHexColor;
		}
	}

#if UNITY_EDITOR
	public class KZIsValidAttributeDrawer : KZAttributeDrawer<KZIsValidAttribute,bool>
	{
		protected override void _DrawPropertyLayout(GUIContent label)
		{
			var rect = DrawPrefixLabel(label);

			var text = ValueEntry.SmartValue ? Attribute.CorrectText : Attribute.WrongText;
			var style = GetValidationStyle(ValueEntry.SmartValue,Attribute.WrongHexColor);

			EditorGUI.LabelField(rect,text,style);
		}
	}
#endif
}