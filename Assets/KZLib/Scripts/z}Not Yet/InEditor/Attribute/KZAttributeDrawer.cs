using System;
using UnityEngine;
using Sirenix.Utilities;

#if UNITY_EDITOR

using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

#endif

namespace KZLib.KZAttribute
{
#if UNITY_EDITOR
	public abstract class KZAttributeDrawer<TAttribute,TValue> : OdinAttributeDrawer<TAttribute,TValue> where TAttribute : Attribute
	{
		protected string m_ErrorMessage = null;

		protected abstract void DoDrawPropertyLayout(GUIContent _label);

		protected override void DrawPropertyLayout(GUIContent _label)
		{
			var cashed = GUI.enabled;

			GUI.enabled = true;

			if(m_ErrorMessage.IsEmpty())
			{
				//? 에러가 없는 경우

				DoDrawPropertyLayout(_label);
			}
			else
			{
				//? 에러가 있는 경우
				SirenixEditorGUI.ErrorMessageBox(m_ErrorMessage);

				CallNextDrawer(_label);
			}

			GUI.enabled = cashed;
		}

		protected Rect DrawPrefixLabel(GUIContent _label,float? _height = null)
		{
			var rect = _height.HasValue ? EditorGUILayout.GetControlRect(true,_height.Value) : EditorGUILayout.GetControlRect();

			if(_label == null)
			{
				return rect;
			}

			EditorGUI.PrefixLabel(rect,_label);

			return rect.HorizontalPadding(EditorGUIUtility.labelWidth,0.0f);
		}

		protected TMember GetValue<TMember>(string _memberName)
		{
			return ReflectionUtility.GetValueInObject<TMember>(_memberName,Property.ParentValues[0]);
		}

		protected GUIStyle GetValidateStyle(bool _isValid,string _wrongColor = null)
		{
			var wrongColor = _wrongColor ?? Global.WRONG_HEX_COLOR;

			var style = new GUIStyle(GUI.skin.label);
			style.normal.textColor = _isValid ? style.normal.textColor : wrongColor.ToColor();

			return style;
		}

		protected void DrawColor(Rect _rect,Color _color)
		{
			var height = _rect.height;

			SirenixEditorGUI.DrawSolidRect(_rect.VerticalPadding(0.0f,height*0.3f),_color.MaskAlpha(1.0f));

			SirenixEditorGUI.DrawSolidRect(_rect.VerticalPadding(height*0.7f,0.0f),Color.white);
			SirenixEditorGUI.DrawSolidRect(_rect.VerticalPadding(height*0.7f,0.0f),new Color(0.0f,0.0f,0.0f,_color.a));
		}

		protected Rect[] GetRectArray(Rect _rect,int _count,float _space = 0.0f)
		{
			var rectArray = new Rect[_count];

			var totalWidth = _rect.width-(_count-1)*_space;
			var width = totalWidth/_count;

			rectArray[0] = new Rect(_rect.x,_rect.y,width,_rect.height);

			for(var i=1;i<_count;i++)
			{
				rectArray[i] = new Rect(_rect.x+(_space+width)*i,_rect.y,width,_rect.height);
			}

			return rectArray;
		}
	}
#endif
}