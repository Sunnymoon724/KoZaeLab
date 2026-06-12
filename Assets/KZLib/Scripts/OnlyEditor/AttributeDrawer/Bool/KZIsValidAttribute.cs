#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace KZLib.Attributes
{
	public class KZIsValidAttributeDrawer : KZAttributeDrawer<KZIsValidAttribute,bool>
	{
		protected override void _DoDrawPropertyLayout(GUIContent label)
		{
			var rect = _DrawPrefixLabel(label);

			var text = ValueEntry.SmartValue ? Attribute.CorrectText : Attribute.WrongText;
			var style = _GetValidationStyle(ValueEntry.SmartValue,Attribute.WrongHexColor);

			EditorGUI.LabelField(rect,text,style);
		}
	}
}
#endif