using UnityEngine;

namespace KZLib.Auth
{
	/// <summary>
	/// The metadata for an audio file stored in Google Drive.
	/// Unity-specific data to use with <see cref="GoogleDriveFiles.DownloadAudioRequest"/>.
	/// </summary>
	public class GoogleAudioFile : GoogleFile
	{
		[Newtonsoft.Json.JsonIgnore]
		public AudioClip AudioClip { get; set; }
	}
}
