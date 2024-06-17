using System;
using UnityEngine;
using System.Diagnostics;
using System.Linq;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditorInternal;

#endif

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZTagAttribute : Attribute { }

#if UNITY_EDITOR
	public class KZTagAttributeDrawer : KZAttributeDrawer<KZTagAttribute,string>
	{
		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			//? label
			var rect = DrawPrefixLabel(_label);

			var tagArray = InternalEditorUtility.tags;

			EditorGUI.BeginChangeCheck();

			var index = tagArray.Contains(ValueEntry.SmartValue) ? Array.IndexOf(tagArray,ValueEntry.SmartValue) : 0;

			index = EditorGUI.Popup(rect,index,InternalEditorUtility.tags);

			if(EditorGUI.EndChangeCheck())
			{
				ValueEntry.SmartValue = InternalEditorUtility.tags[index];
			}
		}
	}
#endif
}