using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Diagnostics;

using Object = UnityEngine.Object;

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
		protected KZResourcePathAttribute(bool changePathBtn,bool isIncludeAssets) : base(true,changePathBtn,isIncludeAssets) { }
	}

#if UNITY_EDITOR
	public abstract class KZResourcePathAttributeDrawer<TResource> : KZPathAttributeDrawer<TResource> where TResource : KZResourcePathAttribute
	{
		protected abstract SdfIconType IconType { get; }
		protected abstract string ResourceKind { get; }

		protected abstract void OnOpenResource();

		protected override string GetNewPath()
		{
			return CommonUtility.GetFilePathInPanel("Change new path.",ResourceKind);
		}

		protected override Rect OnClickToOpen(Rect rect,bool isValid)
		{
			var newRect = DrawParentFolderOpenButton(rect,isValid);

			return DrawButton(newRect,IconType,isValid,OnOpenResource);
		}

		protected override bool IsValidPath()
		{
			return CommonUtility.IsFileExist(CommonUtility.GetAbsolutePath(ValueEntry.SmartValue,Attribute.IsIncludeAssets));
		}

		protected UResource GetResource<UResource>() where UResource : Object
		{
			var assetsPath = CommonUtility.GetAssetsPath(ValueEntry.SmartValue);

			if(!CommonUtility.IsStartWithAssetsHeader(assetsPath))
			{
				CommonUtility.DisplayError(new Exception($"{ValueEntry.SmartValue} is not in the Assets folder."));

				return null;
			}

			var resource = AssetDatabase.LoadAssetAtPath<UResource>(assetsPath);

			if(!resource)
			{
				CommonUtility.DisplayError(new Exception($"{assetsPath} is not a resource."));

				return null;
			}

			return resource;
		}

		protected class ResourceViewer<UObject> : OdinEditorWindow where UObject : Object
		{
			private bool m_changed = false;
			private Editor m_editor = null;
			protected string m_objectPath = null;
			private UObject m_viewerObject = null;

			protected UObject ViewerObject
			{
				get => m_viewerObject;
				set
				{
					m_viewerObject = value;
					m_changed = false;
				}
			}

			public virtual void Initialize(object pathParam)
			{
				m_objectPath = pathParam as string;

				if(m_objectPath.IsEmpty())
				{
					return;
				}

				ViewerObject = AssetDatabase.LoadAssetAtPath<UObject>(CommonUtility.GetAssetsPath(m_objectPath));
			}

			[OnInspectorGUI]
            protected void OnInspector()
            {
				if(!m_changed)
				{
					m_editor = Editor.CreateEditor(ViewerObject);

					m_changed = true;
				}

				ShowEditor();
			}

			protected override void OnDestroy()
			{
				if(m_editor)
				{
					DestroyImmediate(m_editor);
				}
			}

			protected virtual void ShowEditor()
			{
				m_editor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(200,200),new GUIStyle());
			}
		}
	}
#endif
}