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
	public class KZCheckVersionAttribute : Attribute { }

#if UNITY_EDITOR
	public class KZCheckVersionAttributeDrawer : KZAttributeDrawer<KZCheckVersionAttribute,string>
	{
		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			var rect = DrawPrefixLabel(_label);

			EditorGUI.LabelField(rect,ValueEntry.SmartValue,GetValidateStyle(CommonUtility.CheckVersion(ValueEntry.SmartValue),Global.DISABLE_HEX_COLOR));
		}
	}
#endif
}