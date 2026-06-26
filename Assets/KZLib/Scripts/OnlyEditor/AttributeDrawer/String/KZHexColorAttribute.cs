#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace KZLib.Attributes
{
	/// <summary><see cref="KZHexColorAttribute"/> drawer. Converts between ColorField and hex string.</summary>
	public class KZHexColorAttributeDrawer : KZAttributeDrawer<KZHexColorAttribute,string>
	{
		protected override void _DoDrawPropertyLayout(GUIContent label)
		{
			var rect = _DrawPrefixLabel(label);

			var hexCode = ValueEntry.SmartValue;

			var color = hexCode.IsEmpty() ? Color.white : hexCode.ToColor();
			var result = EditorGUI.ColorField(rect,color);

			if(color != result)
			{
				ValueEntry.SmartValue = result.ToHexCode();
			}
		}
	}
}
#endif