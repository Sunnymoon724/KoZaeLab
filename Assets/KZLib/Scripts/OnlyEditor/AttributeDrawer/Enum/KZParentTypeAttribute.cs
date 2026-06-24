#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace KZLib.Attributes
{
	/// <summary>
	/// <see cref="KZParentTypeAttribute"/> drawer.
	/// Shows only enum members whose <see cref="KZParentTypeAttribute.ParentType"/> matches the field attribute.
	/// </summary>
	public class KZParentTypeAttributeDrawer : KZAttributeDrawer<KZParentTypeAttribute,Enum>
	{
		protected override void _DoDrawPropertyLayout(GUIContent label)
		{
			var enumType = ValueEntry.TypeOfValue;
			var valueArray = Enum.GetValues(enumType).Cast<Enum>().Where(value => _IsVisible(enumType,value)).ToArray();

			if(valueArray.Length == 0)
			{
				CallNextDrawer(label);

				return;
			}

			var current = (Enum)ValueEntry.SmartValue;
			var index = Array.IndexOf(valueArray,current);

			if(index < 0)
			{
				index = 0;
			}

			var rect = EditorGUILayout.GetControlRect();
			var nameArray = valueArray.Select(value => value.ToString()).ToArray();
			var newIndex = EditorGUI.Popup(rect,_GetLabelText(label),index,nameArray);

			ValueEntry.SmartValue = valueArray[newIndex];
		}

		/// <summary>Visible when the enum member has no attribute or a matching ParentType.</summary>
		private bool _IsVisible(Type enumType,Enum value)
		{
			var field = enumType.GetField(value.ToString(),BindingFlags.Public | BindingFlags.Static);

			if(field == null)
			{
				return true;
			}

			var parentTypeAttribute = field.GetCustomAttribute<KZParentTypeAttribute>();

			return parentTypeAttribute == null || parentTypeAttribute.ParentType == Attribute.ParentType;
		}
	}
}
#endif
