using System;
using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using KZLib.KZUtility;


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

		protected KZPathAttribute(bool addOpenBtn,bool changePathBtn,bool isIncludeAssets) : base()
		{
			AddOpenButton = addOpenBtn;
			AddChangeButton = changePathBtn;

			IsIncludeAssets = isIncludeAssets;
		}
	}

#if UNITY_EDITOR
	public abstract class KZPathAttributeDrawer<TAttribute> : KZAttributeDrawer<TAttribute,string> where TAttribute : KZPathAttribute
	{
		private const int c_buttonWidth = 30;

		protected readonly List<Func<Rect,bool,Rect>> m_onClickedList = new();

		protected string AbsolutePath => FileUtility.GetAbsolutePath(ValueEntry.SmartValue,Attribute.IsIncludeAssets);

		protected override void Initialize()
		{
			if(Attribute.AddChangeButton)
			{
				m_onClickedList.Add(_OnClickToChangePath);
			}

			if(Attribute.AddOpenButton)
			{
				m_onClickedList.Add(OnClickToOpen);
			}
		}

		protected abstract string FindNewPath();

		protected abstract bool IsValidPath();

		private Rect _OnClickToChangePath(Rect rect,bool _)
		{
			return DrawButton(rect,SdfIconType.ArrowRepeat,true,()=>
			{
				var dataPath = FindNewPath();

				if(dataPath.IsEmpty())
				{
					return;
				}

				if(Attribute.IsIncludeAssets)
				{
					//? Path in assets folder
					if(!FileUtility.IsIncludeAssetHeader(dataPath))
					{
						CommonUtility.DisplayError(new NullReferenceException($"{dataPath} is not in assets folder."));
					}

					ValueEntry.SmartValue = FileUtility.RemoveAssetHeader(dataPath);
				}
				else
				{
					//? Path in project folder
					ValueEntry.SmartValue = dataPath;
				}
			});
		}

		protected abstract Rect OnClickToOpen(Rect rect,bool isValid);

		protected override void _DrawPropertyLayout(GUIContent label)
		{
			var isValid = IsValidPath();
			var rect = DrawPrefixLabel(label);

			//? Add other buttons
			foreach(var onClicked in m_onClickedList)
			{
				rect = onClicked(rect,isValid);
			}

			EditorGUI.LabelField(rect,ValueEntry.SmartValue,GetValidationStyle(isValid,Global.WRONG_HEX_COLOR));
		}

		protected Rect DrawButton(Rect rect,SdfIconType iconType,bool active,Action onClicked)
		{
			var cached = GUI.enabled;
			GUI.enabled = active;

			var newRect = new Rect(rect.xMax-c_buttonWidth,rect.y,c_buttonWidth,rect.height);

			if(SirenixEditorGUI.SDFIconButton(newRect,iconType,new GUIStyle(GUI.skin.button)))
			{
				onClicked?.Invoke();
			}

			GUI.enabled = cached;

			return new Rect(rect.x,rect.y,rect.width-c_buttonWidth,rect.height);
		}

		protected Rect DrawParentFolderOpenButton(Rect rect,bool isValid)
		{
			return DrawButton(rect,SdfIconType.Folder2,isValid,()=>
			{
				CommonUtility.Open(FileUtility.GetParentPath(AbsolutePath));
			});
		}
	}
#endif
}