using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Diagnostics;

#if UNITY_EDITOR

using UnityEditor;
using Sirenix.OdinInspector.Editor;

#endif

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public abstract class KZResourcePathAttribute : KZPathAttribute
	{
		protected KZResourcePathAttribute(bool _changePathBtn,bool _includeProject) : base(true,_changePathBtn,_includeProject) { }
	}

#if UNITY_EDITOR
	public abstract class KZResourcePathAttributeDrawer<TResource> : KZPathAttributeDrawer<TResource> where TResource : KZResourcePathAttribute
	{
		protected abstract SdfIconType IconType { get; }
		protected abstract string ResourceKind { get; }

		protected abstract void OnOpenResource();

		protected override string GetNewPath()
		{
			return CommonUtility.GetFilePathInPanel("위치를 수정 합니다.",ResourceKind);
		}

		protected override Rect OnClickToOpen(Rect _rect,bool _isValid)
		{
			var rect = DrawParentFolderOpenButton(_rect,_isValid);

			return DrawButton(rect,IconType,_isValid,OnOpenResource);
		}

		protected override bool IsValidPath()
		{
			return CommonUtility.IsExistFile(ValueEntry.SmartValue);
		}

		protected class ResourceViewer<UObject> : OdinEditorWindow where UObject : UnityEngine.Object
		{
			private bool m_Changed = false;
			private Editor m_Editor = null;
			protected string m_ObjectPath = null;
			private UObject m_ViewerObject = null;

			protected UObject ViewerObject
			{
				get => m_ViewerObject;
				set
				{
					m_ViewerObject = value;
					m_Changed = false;
				}
			}

			public virtual void Initialize(object _param)
			{
				m_ObjectPath = _param as string;

				if(m_ObjectPath.IsEmpty())
				{
					return;
				}

				ViewerObject = AssetDatabase.LoadAssetAtPath<UObject>(CommonUtility.GetAssetsPath(m_ObjectPath));
			}

			[OnInspectorGUI]
#pragma warning disable IDE0051
            private void OnInspector()
            {
				if(!m_Changed)
				{
					m_Editor = Editor.CreateEditor(ViewerObject);

					m_Changed = true;
				}

				ShowEditor();
			}
#pragma warning restore IDE0051

			protected override void OnDestroy()
			{
				if(m_Editor)
				{
					DestroyImmediate(m_Editor);
				}
			}

			protected virtual void ShowEditor()
			{
				m_Editor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(200,200),new GUIStyle());
			}
		}
	}
#endif
}