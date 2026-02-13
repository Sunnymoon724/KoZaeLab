using System;
using UnityEngine;
using Sirenix.Utilities;

#if UNITY_EDITOR

using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

#endif

using Attribute = System.Attribute;

namespace KZLib.Attributes
{
#if UNITY_EDITOR
	public abstract class KZAttributeDrawer<TAttribute,TValue> : OdinAttributeDrawer<TAttribute,TValue> where TAttribute : Attribute
	{
		protected string m_errorMessage = null;

		protected abstract void _DoDrawPropertyLayout(GUIContent label);

		protected override void DrawPropertyLayout(GUIContent label)
		{
			var cashed = GUI.enabled;
			var isValid = m_errorMessage.IsEmpty();

			GUI.enabled = isValid;

			if(isValid)
			{
				//? No Error
				_DoDrawPropertyLayout(label);
			}
			else
			{
				//? Include Error
				SirenixEditorGUI.ErrorMessageBox(m_errorMessage);

				CallNextDrawer(label);
			}

			GUI.enabled = cashed;
		}

		protected Rect _DrawPrefixLabel(GUIContent label,float? height = null)
		{
			var rect = height.HasValue ? EditorGUILayout.GetControlRect(true,height.Value) : EditorGUILayout.GetControlRect();

			if(label == null)
			{
				return rect;
			}

			EditorGUI.PrefixLabel(rect,label);

			return rect.HorizontalPadding(EditorGUIUtility.labelWidth,0.0f);
		}

		protected TMember _FindValue<TMember>(string memberName)
		{
			return CommonUtility.FindValueInObject<TMember>(memberName,Property.ParentValues[0]);
		}

		protected GUIStyle _GetValidationStyle(bool isValid,string wrongHexColor = null)
		{
			var style = new GUIStyle(GUI.skin.label);

			style.normal.textColor = isValid ? style.normal.textColor : wrongHexColor?.ToColor() ?? style.normal.textColor;

			return style;
		}

		protected void _DrawColor(Rect rect,Color color)
		{
			var height = rect.height;

			SirenixEditorGUI.DrawSolidRect(rect.VerticalPadding(0.0f,height*0.3f),color.MaskAlpha(1.0f));
			SirenixEditorGUI.DrawSolidRect(rect.VerticalPadding(height*0.7f,0.0f),Color.white);
			SirenixEditorGUI.DrawSolidRect(rect.VerticalPadding(height*0.7f,0.0f),new Color(0.0f,0.0f,0.0f,color.a));
		}

		protected Rect[] _GetRectArray(Rect rect,int count,float space = 0.0f)
		{
			var rectArray = new Rect[count];
			var totalWidth = rect.width-(count-1)*space;
			var width = totalWidth/count;

			rectArray[0] = new Rect(rect.x,rect.y,width,rect.height);

			for(var i=1;i<count;i++)
			{
				rectArray[i] = new Rect(rect.x+(space+width)*i,rect.y,width,rect.height);
			}

			return rectArray;
		}

		protected string _GetLabelText(GUIContent label)
		{
			return label == null ? string.Empty : label.text;
		}

		protected object _ConvertToValue(object value)
		{
			if(value == null)
			{
				return default;
			}

			var type = typeof(TValue);

			return (TValue) Convert.ChangeType(value,type);
		}
	}
#endif
}