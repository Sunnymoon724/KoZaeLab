#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine.Video;
using UnityEditor;

namespace KZLib.Attributes
{
	/// <summary><see cref="KZVideoPathAttribute"/> drawer. VideoClip preview window.</summary>
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
}
#endif