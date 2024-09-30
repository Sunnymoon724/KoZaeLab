using System;
using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;
using Sirenix.OdinInspector;

#if UNITY_EDITOR

using UnityEditor;
using Sirenix.Utilities.Editor;

#endif

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public abstract class KZPathAttribute : Attribute
	{
		public bool AddOpenButton { get; }
		public bool AddChangeButton { get; }

		public bool IsIncludeAssets { get; }

		protected KZPathAttribute(bool _addOpenBtn,bool _changePathBtn,bool _isIncludeAssets) : base()
		{
			AddOpenButton = _addOpenBtn;
			AddChangeButton = _changePathBtn;

			IsIncludeAssets = _isIncludeAssets;
		}
	}

#if UNITY_EDITOR
	public abstract class KZPathAttributeDrawer<TAttribute> : KZAttributeDrawer<TAttribute,string> where TAttribute : KZPathAttribute
	{
		private const int BUTTON_WIDTH = 30;

		protected readonly List<Func<Rect,bool,Rect>> m_OnClickedList = new();

		protected override void Initialize()
		{
			if(Attribute.AddChangeButton)
			{
				m_OnClickedList.Add(OnClickToChangePath);
			}

			if(Attribute.AddOpenButton)
			{
				m_OnClickedList.Add(OnClickToOpen);
			}
		}

		protected abstract string GetNewPath();

		protected abstract bool IsValidPath();

		private Rect OnClickToChangePath(Rect _rect,bool _)
		{
			return DrawButton(_rect,SdfIconType.ArrowRepeat,true,()=>
			{
				var dataPath = GetNewPath();

				if(dataPath.IsEmpty())
				{
					return;
				}

				if(Attribute.IsIncludeAssets)
				{
					//? Path in assets folder
					if(!FileUtility.IsIncludeAssetsHeader(dataPath))
					{
						UnityUtility.DisplayError($"{dataPath} is not in assets folder.");
					}

					ValueEntry.SmartValue = FileUtility.RemoveAssetsHeader(dataPath);
				}
				else
				{
					//? Path in project folder
					ValueEntry.SmartValue = dataPath;
				}
			});
		}

		protected abstract Rect OnClickToOpen(Rect _rect,bool _isValid);

		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			var isValid = IsValidPath();
			var rect = DrawPrefixLabel(_label);

			//? Add other buttons
			foreach(var onClicked in m_OnClickedList)
			{
				rect = onClicked(rect,isValid);

				// if(Event.current.type == EventType.Repaint && rect.Contains(Event.current.mousePosition))
				// {
				// 	GUI.Label(rect, new GUIContent("","툴팁"));
				// }
			}

			EditorGUI.LabelField(rect,ValueEntry.SmartValue,GetValidateStyle(isValid,Global.WRONG_HEX_COLOR));
		}

		protected Rect DrawButton(Rect _rect,SdfIconType _icon,bool _active,Action _onClicked)
		{
			var cached = GUI.enabled;
			GUI.enabled = _active;

			var rect = new Rect(_rect.xMax-BUTTON_WIDTH,_rect.y,BUTTON_WIDTH,_rect.height);

			if(SirenixEditorGUI.SDFIconButton(rect,_icon,new GUIStyle(GUI.skin.button)))
			{
				_onClicked?.Invoke();
			}

			GUI.enabled = cached;

			return new Rect(_rect.x,_rect.y,_rect.width-BUTTON_WIDTH,_rect.height);
		}

		protected Rect DrawParentFolderOpenButton(Rect _rect,bool _isValid)
		{
			return DrawButton(_rect,SdfIconType.Folder2,_isValid,()=>
			{
				FileUtility.Open(FileUtility.GetParentPath(FileUtility.GetAbsolutePath(ValueEntry.SmartValue,Attribute.IsIncludeAssets)));
			});
		}
	}
#endif
}