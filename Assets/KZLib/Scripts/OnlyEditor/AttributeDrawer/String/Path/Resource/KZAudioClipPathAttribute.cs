#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

namespace KZLib.Attributes
{
	/// <summary><see cref="KZAudioClipPathAttribute"/> drawer. AudioClip preview window.</summary>
	public class KZAudioClipPathAttributeDrawer : KZResourcePathAttributeDrawer<KZAudioClipPathAttribute>
	{
		protected override SdfIconType IconType => SdfIconType.MusicNoteBeamed;

		protected override string ResourceKind => "wav,mp3,ogg,aiff,aif,flac";

		protected override void OnOpenResource()
		{
			var viewer = EditorWindow.GetWindow<AudioClipViewer>("Audio Clip Viewer");

			viewer.Initialize(ValueEntry.SmartValue);
			viewer.Show();
		}

		private class AudioClipViewer : ResourceViewer<AudioClip> { }
	}
}
#endif