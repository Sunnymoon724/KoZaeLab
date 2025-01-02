using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Diagnostics;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZAudioClipPathAttribute : KZResourcePathAttribute
	{
		public KZAudioClipPathAttribute(bool changePathButton = false) : base(changePathButton,true) { }
	}

#if UNITY_EDITOR
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
#endif
}