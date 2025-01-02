using System;
using Sirenix.OdinInspector;
using System.Diagnostics;
using UnityEngine.Video;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZVideoPathAttribute : KZResourcePathAttribute
	{
		public KZVideoPathAttribute(bool changePathButton = false) : base(changePathButton,true) { }
	}

#if UNITY_EDITOR
	public class KZVideoPathAttributeDrawer : KZResourcePathAttributeDrawer<KZVideoPathAttribute>
	{
		protected override SdfIconType IconType => SdfIconType.CameraVideo;
	
		protected override string ResourceKind => "mp4,m4v,dv,mov,mpg,mpeg,ogv,vp8,webm";

		protected override void OnOpenResource()
		{
			var viewer = EditorWindow.GetWindow<VideoClipViewer>("Video Viewer");

			viewer.Initialize(ValueEntry.SmartValue);
			viewer.Show();
		}

		private class VideoClipViewer : ResourceViewer<VideoClip> { }
	}
#endif
}