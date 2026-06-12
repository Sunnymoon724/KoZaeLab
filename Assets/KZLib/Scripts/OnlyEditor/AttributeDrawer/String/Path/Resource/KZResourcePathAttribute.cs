#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

using Object = UnityEngine.Object;

namespace KZLib.Attributes
{
	public abstract class KZResourcePathAttributeDrawer<TResource> : KZPathAttributeDrawer<TResource> where TResource : KZResourcePathAttribute
	{
		protected abstract SdfIconType IconType { get; }
		protected abstract string ResourceKind { get; }

		protected abstract void OnOpenResource();

		protected override string FindNewPath()
		{
			return KZEditorKit.FindFilePathInPanel("Change new path.",ResourceKind);
		}

		protected override Rect OnClickToOpen(Rect rect,bool isValid)
		{
			var newRect = DrawParentFolderOpenButton(rect,isValid);

			return DrawButton(newRect,IconType,isValid,OnOpenResource);
		}

		protected override bool IsValidPath()
		{
			return KZFileKit.IsFileExist(AbsolutePath);
		}

		protected UResource GetResource<UResource>() where UResource : Object
		{
			var assetsPath = KZFileKit.GetAssetPath(ValueEntry.SmartValue);

			if(!KZFileKit.IsStartWithAssetHeader(assetsPath))
			{
				KZEditorKit.DisplayError(new Exception($"{ValueEntry.SmartValue} is not in the Assets folder. Path must be assigned."));

				return null;
			}

			var resource = AssetDatabase.LoadAssetAtPath<UResource>(assetsPath);

			if(!resource)
			{
				KZEditorKit.DisplayError(new Exception($"{assetsPath} is not a resource. Path must be assigned."));

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

			public virtual void Initialize(object param)
			{
				m_objectPath = param as string;

				if(m_objectPath.IsEmpty())
				{
					return;
				}

				ViewerObject = AssetDatabase.LoadAssetAtPath<UObject>(KZFileKit.GetAssetPath(m_objectPath));
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
}
#endif
