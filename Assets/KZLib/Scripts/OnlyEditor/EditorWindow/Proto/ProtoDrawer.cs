#if UNITY_EDITOR
using System.Reflection;
using System.Text;
using KZLib.Data;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace KZLib.Windows
{
	/// <summary>
	/// Custom Odin drawer for <see cref="IProto"/> list entries in <see cref="ProtoWindow"/>.
	/// Shows the proto number and copies all public properties to the clipboard.
	/// </summary>
	public class ProtoDrawer : OdinValueDrawer<IProto>
	{
		private const float c_copyButtonWidth = 50.0f;

		protected override void DrawPropertyLayout(GUIContent label)
		{
			var rect = EditorGUILayout.GetControlRect();
			var proto = ValueEntry.SmartValue;

			if(proto == null)
			{
				EditorGUI.LabelField(rect,"Proto is null");

				return;
			}

			if(SirenixEditorGUI.SDFIconButton(new Rect(rect.x+rect.width-c_copyButtonWidth,rect.y,c_copyButtonWidth,rect.height),SdfIconType.Clipboard,new GUIStyle(GUI.skin.button)))
			{
				_CopyAllPropertiesToClipboard(proto);
			}

			EditorGUI.LabelField(new Rect(rect.x,rect.y,rect.width-c_copyButtonWidth,rect.height),$"Num - {proto.Num}");
		}

		/// <summary>
		/// Serializes readable public properties as name:value lines for debugging and diffing.
		/// </summary>
		private static void _CopyAllPropertiesToClipboard(IProto proto)
		{
			var propertyArray = proto.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var stringBuilder = new StringBuilder();

			for(var i=0;i<propertyArray.Length;i++)
			{
				var property = propertyArray[i];

				if(!property.CanRead || property.GetMethod == null || !property.GetMethod.IsPublic)
				{
					continue;
				}

				var value = property.GetValue(proto,null);

				stringBuilder.AppendLine($"{property.Name}:{value}");
			}

			GUIUtility.systemCopyBuffer = stringBuilder.ToString();

			KZEditorKit.DisplayInfo("The properties have been copied to the clipboard.");
		}
	}
}
#endif