﻿using System.IO;
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

	public const string ASSETS_TEXT = "Assets";
	public const string RESOURCES_TEXT = "Resources";

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
	public readonly static string[] EXCEL_EXTENSION_ARRAY = new string[] { ".xls", ".xlsx", ".xlsm" };


	public const string GLOBAL_TEXTURE_MIPMAP_LIMIT = "GlobalTextureMipmapLimit";
	public const string ANISOTROPIC_FILTERING = "AnisotropicFiltering";
	public const string VERTICAL_SYNC_COUNT = "VerticalSyncCount";
	public const string DISABLE_CAMERA_FAR_HALF = "DisableCameraFarHalf";

	public const string DISPLAY_LOG = "DisplayLog";
	
	public const string TITLE_SCENE = "TitleScene";
}

#region UI Name
public partial struct Global
{
	public const string HUD_PANEL_UI = "HudPanelUI";

	public const string TRANSITION_PANEL_UI = "TransitionPanelUI";

	public const string VIDEO_PANEL_UI = "VideoPanelUI";
	public const string SUBTITLE_PANEL_UI = "SubtitlePanelUI";
	public const string SKIP_PANEL_UI = "SkipPanelUI";

	public const string DIALOG_BOX_PANEL_UI = "DialogBoxPopupUI";

	public const string DOWNLOAD_PANEL_UI = "DownloadPanelUI";

	public const string LOADING_PANEL_UI = "LoadingPanelUI";
}
#endregion UI Name