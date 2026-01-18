using System.IO;
using UnityEngine;

#region General
public static partial class Global
{
	//? General
	public const float ZERO_ANGLE = 0.0f;
	public const float HALF_ANGLE = 180.0f;
	public const float FULL_ANGLE = 360.0f;

	public const int BASE_WIDTH = 1920;
	public const int BASE_HEIGHT = 1080;

	public const int FRAME_RATE_60 = 60;
	public const int FRAME_RATE_30 = 30;

	public static readonly string[] BYTE_UNIT_ARRAY = new string[] { "B","KB","MB","GB","TB" };

	public static readonly Vector3 CENTER_VIEWPORT_POINT = new(0.5f,0.5f,1.0f);

	public const int INVALID_INDEX = -1;

	public const string DISABLE_HEX_COLOR = "#808080FF";
	public const string WRONG_HEX_COLOR = "#FFC300FF";

	public const float PERCENT_HUNDRED = 0.01f;
	public const float PERCENT_THOUSAND = 0.001f;
	public const float PERCENT_10THOUSAND = 0.0001f;

	public const int MIN_POLYGON_COUNT = 3;
	public const int MAX_POLYGON_COUNT = 36;
}
#endregion General

// #region Time
// public static partial class Global
// {
	
// }
// #endregion Time

#region Path
public static partial class Global
{
	public readonly static string PROJECT_PATH = Directory.GetCurrentDirectory();
	public readonly static string PROJECT_PARENT_PATH = Path.GetFullPath(Path.Combine(PROJECT_PATH,".."));

	public readonly static string DOCUMENT_FOLDER_PATH = Path.Combine(PROJECT_PARENT_PATH,"Documents");
	public readonly static string TOOL_FOLDER_PATH = Path.Combine(PROJECT_PARENT_PATH,"Tools");


	#region Config
	public readonly static string CONFIG_FOLDER_PATH = Path.Combine(DOCUMENT_FOLDER_PATH,"Config");
	public readonly static string CUSTOM_CONFIG_FOLDER_PATH = Path.Combine(CONFIG_FOLDER_PATH,"Custom");
	#endregion Config

	#region Proto
	public readonly static string PROTO_FOLDER_PATH = Path.Combine(DOCUMENT_FOLDER_PATH,"Proto");
	#endregion Proto

	#region Lingo
	public readonly static string LINGO_FOLDER_PATH = Path.Combine(DOCUMENT_FOLDER_PATH,"Lingo");
	#endregion Lingo
}
#endregion Path

#region Graphic Option
public static partial class Global
{
	
	public const string GLOBAL_TEXTURE_MIPMAP_LIMIT = "GlobalTextureMipmapLimit";
	public const string ANISOTROPIC_FILTERING = "AnisotropicFiltering";
	public const string VERTICAL_SYNC_COUNT = "VerticalSyncCount";
	public const string DISABLE_CAMERA_FAR_HALF = "DisableCameraFarHalf";
}
#endregion Graphic Option

#region Menu Item
public static partial class Global
{
	public const int MENU_ORDER_MAIN_SPACE = 1000000;
	public const int MENU_ORDER_SUB_SPACE = 50;
}
#endregion Menu Item