using System.IO;
using KZLib.KZUtility;
using UnityEngine;

public partial struct Global
{
	//? General
	public const float HALF_ANGLE = 180.0f;
	public const float FULL_ANGLE = 360.0f;

	public const int BASE_WIDTH = 1920;
	public const int BASE_HEIGHT = 1080;

	public const int FRAME_RATE_60 = 60;
	public const int FRAME_RATE_30 = 30;

	public static readonly string[] BYTE_UNIT_ARRAY = new string[] { "B","KB","MB","GB","TB" };

	public static readonly Vector3 CENTER_VIEWPORT_POINT = new(0.5f,0.5f,1.0f);

	public const int INVALID_INDEX = -1;
	public const int INVALID_NUM = 0;
	public const float INVALID_TIME = Mathf.Infinity;
	public const float INVALID_DURATION = Mathf.Infinity;

	public const float FADE_TIME = 1.0f;

	public const string DISABLE_HEX_COLOR = "#808080FF";
	public const string WRONG_HEX_COLOR = "#FFC300FF";

	public const string ASSETS_HEADER = "Assets";

	public readonly static string PROJECT_PATH = Directory.GetCurrentDirectory();
	public readonly static string PROJECT_PARENT_PATH = Path.GetFullPath(Path.Combine(PROJECT_PATH,".."));

	#region Config
	public readonly static string CONFIG_FOLDER_PATH = Path.Combine(PROJECT_PARENT_PATH,"Documents","Config");
	public readonly static string CUSTOM_CONFIG_FOLDER_PATH = Path.Combine(CONFIG_FOLDER_PATH,"Custom");
	#endregion Config

	#region Proto
	public readonly static string PROTO_FOLDER_PATH = Path.Combine(PROJECT_PARENT_PATH,"Documents","Proto");
	#endregion Proto

	#region Lingo
	public readonly static string LINGO_FOLDER_PATH = Path.Combine(PROJECT_PARENT_PATH,"Documents","Lingo");
	#endregion Lingo
	public readonly static string[] EXCEL_EXTENSION_ARRAY = new string[] { ".xls", ".xlsx", ".xlsm" };


	public const string GLOBAL_TEXTURE_MIPMAP_LIMIT = "GlobalTextureMipmapLimit";
	public const string ANISOTROPIC_FILTERING = "AnisotropicFiltering";
	public const string VERTICAL_SYNC_COUNT = "VerticalSyncCount";
	public const string DISABLE_CAMERA_FAR_HALF = "DisableCameraFarHalf";
}