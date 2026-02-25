#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using KZLib.Data;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Reflection;
using Sirenix.OdinInspector;
using System.Text;

public class ProtoDrawer : OdinValueDrawer<IProto>
{
	protected override void DrawPropertyLayout(GUIContent label)
	{
		var rect = EditorGUILayout.GetControlRect();

		var proto = ValueEntry.SmartValue;
		var numProperty = proto.GetType().GetProperty("Num");
		var numValue = numProperty.GetValue(proto);

		EditorGUILayout.BeginHorizontal();
		EditorGUI.LabelField(new Rect(rect.x,rect.y,rect.width-50.0f,rect.height),$"Num - {numValue}");

		if(SirenixEditorGUI.SDFIconButton(new Rect(rect.x+rect.width-50.0f,rect.y,50.0f,rect.height),SdfIconType.Clipboard,new GUIStyle(GUI.skin.button)))
		{
			_CopyAllPropertiesToClipboard(proto);
		}

		EditorGUILayout.EndHorizontal();
	}

	private void _CopyAllPropertiesToClipboard(IProto proto)
	{
		var propertyArray = proto.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
		var stringBuilder = new StringBuilder();

		for(var i=0;i<propertyArray.Length;i++)
		{
			var property = propertyArray[i];

			if(!property.CanRead || !property.GetMethod.IsPublic)
			{
				continue;
			}

			var value = property.GetValue(proto, null);

			stringBuilder.AppendLine($"{property.Name}:{value}");
		}

		GUIUtility.systemCopyBuffer = stringBuilder.ToString();

		KZEditorKit.DisplayInfo("The properties have been copied to the clipboard.");
	}
}
#endif