using System;
using UnityEngine;
using System.Diagnostics;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZHexColorAttribute : Attribute { }

#if UNITY_EDITOR
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
#endif
}