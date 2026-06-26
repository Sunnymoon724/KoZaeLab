#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace KZLib.Attributes
{
	/// <summary><see cref="KZIsValidAttribute"/> drawer. Displays bool as O/X text labels.</summary>
	public class KZIsValidAttributeDrawer : KZAttributeDrawer<KZIsValidAttribute,bool>
	{
		protected override void _DoDrawPropertyLayout(GUIContent label)
		{
			var rect = _DrawPrefixLabel(label);

			var text = ValueEntry.SmartValue ? Attribute.CorrectText : Attribute.WrongText;
			var style = _GetValidationStyle(ValueEntry.SmartValue,Attribute.WrongHexColor ?? Global.WrongHexColor);

			EditorGUI.LabelField(rect,text,style);
		}
	}
}
#endif