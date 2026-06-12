#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace KZLib.Attributes
{
	public class KZTagAttributeDrawer : KZAttributeDrawer<KZTagAttribute,string>
	{
		protected override void _DoDrawPropertyLayout(GUIContent label)
		{
			var rect = _DrawPrefixLabel(label);

			var tagArray = InternalEditorUtility.tags;

			EditorGUI.BeginChangeCheck();

			var index = Array.IndexOf(tagArray,ValueEntry.SmartValue);

			index = index >= 0 ? index : 0;

			index = EditorGUI.Popup(rect,index,InternalEditorUtility.tags);

			if(EditorGUI.EndChangeCheck())
			{
				ValueEntry.SmartValue = InternalEditorUtility.tags[index];
			}
		}
	}
}
#endif