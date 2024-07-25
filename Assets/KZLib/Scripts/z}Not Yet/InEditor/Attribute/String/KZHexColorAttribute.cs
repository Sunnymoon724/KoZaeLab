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
	public class KZHexColorAttribute : Attribute { }

#if UNITY_EDITOR
	public class KZHexColorAttributeDrawer : KZAttributeDrawer<KZHexColorAttribute,string>
	{
		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			//? label
			var rect = DrawPrefixLabel(_label);

			var color = ValueEntry.SmartValue.ToColor();
			var result = EditorGUI.ColorField(rect,color);

			if(color != result)
			{
				ValueEntry.SmartValue = result.ToHexCode();
			}
		}
	}
#endif
}