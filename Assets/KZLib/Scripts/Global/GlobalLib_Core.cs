
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
	public const int INVALID_NUMBER = 0;
	public const float INVALID_TIME = Mathf.Infinity;
	public const float INVALID_DURATION = Mathf.Infinity;


	public const string DEFAULT_BUNDLE = "default";
	public const string RESOURCE_BUNDLE = "resource";
	public const string COMMON_BUNDLE = "common";

	public const float FADE_TIME = 1.0f;

	public const string DISABLE_HEX_COLOR = "#808080FF";
	public const string WRONG_HEX_COLOR = "#FFC300FF";

	public const float FRAME_UPDATE_PERIOD = 0.25f;

	public const string CATEGORY_TAG = "Category";
	public const string CATEGORY_LINE = "----------------------------";

	public const string ASSETS_HEADER = "Assets";

	public const int MENU_LINE = 20;
}