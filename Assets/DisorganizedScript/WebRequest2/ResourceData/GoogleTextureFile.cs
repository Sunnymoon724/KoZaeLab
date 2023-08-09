using UnityEngine;

namespace KZLib.Auth
{
	/// <summary>
	/// The metadata for a texture file stored in Google Drive.
	/// Unity-specific data to use with <see cref="GoogleDriveFiles.DownloadTextureRequest"/>.
	/// </summary>
	public class GoogleTextureFile : GoogleFile
	{
		[Newtonsoft.Json.JsonIgnore]
		public Texture2D Texture { get; set; }
	}
}