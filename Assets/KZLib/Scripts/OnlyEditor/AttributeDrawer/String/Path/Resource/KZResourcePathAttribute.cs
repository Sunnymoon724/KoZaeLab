#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

using Object = UnityEngine.Object;

namespace KZLib.Attributes
{
	/// <summary>
	/// <see cref="KZResourcePathAttribute"/> drawer base.
	/// File picker, parent-folder open, and resource preview window.
	/// </summary>
	public abstract class KZResourcePathAttributeDrawer<TResource> : KZPathAttributeDrawer<TResource> where TResource : KZResourcePathAttribute
	{
		protected abstract SdfIconType IconType { get; }
		/// <summary>Filter string for <see cref="KZEditorKit.OpenFilePanel"/>.</summary>
		protected abstract string ResourceKind { get; }

		protected abstract void OnOpenResource();

		protected override string FindNewPath() => KZEditorKit.OpenFilePanel("Change new path.",ResourceKind);

		protected override Rect OnClickToOpen(Rect rect,bool isValid)
		{
			var newRect = DrawParentFolderOpenButton(rect,isValid);

			return DrawButton(newRect,IconType,isValid,OnOpenResource);
		}

		protected override bool IsValidPath() => KZFileKit.IsFileExist(AbsolutePath);

		/// <summary>Loads a resource from an Assets path. Shows an error dialog on failure.</summary>
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

		/// <summary>Base Odin EditorWindow for resource preview.</summary>
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