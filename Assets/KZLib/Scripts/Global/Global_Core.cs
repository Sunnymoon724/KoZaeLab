using System.IO;
using UnityEngine;

#region General
public static partial class Global
{
	public const float ZeroAngle = 0.0f;
	public const float QuarterAngle = 90.0f;
	public const float HalfAngle = 180.0f;
	public const float FullAngle = 360.0f;


	public const int BaseWidth = 1920;
	public const int BaseHeight = 1080;


	public const int FrameRate120 = 120;
	public const int FrameRate60 = 60;
	public const int FrameRate30 = 30;


	public static readonly string[] ByteUnitArray = new string[] { "B","KB","MB","GB","TB" };


	public static readonly Vector2 CenterAnchorPoint = new(0.5f,0.5f);
	public static readonly Vector3 CenterViewportPoint = new(0.5f,0.5f,1.0f);


	public const int InvalidIndex = -1;


	public const float OnePercent = 0.01f;
	public const float OnePermille = 0.001f;
	public const float OnePerTenThousand = 0.0001f;


	public const float SqrMagnitudeThreshold = 0.001f;


	public const float ColorMaxValue = 255.0f;


	public const int BytesPerKilobyte = 1024;
	public const int MillisecondsPerSecond = 1000;


	public const int HexLetterOffset = 10;


	public const float AnimationFinishThreshold = 0.99f;


	public const float DefaultPixelsPerUnit = 100.0f;


	public const int RectCornerCount = 4;


	public const string HexPrefix = "0x";
	public const string HexColorPrefix = "#";
	public const string DefaultAlphaHex = "FF";
	public const string DefaultDateTimeFormat = "yyyy-MM-dd HH:mm";
}
#endregion General

#region HexColor
public static partial class Global
{
	public const string DisableHexColor = "#808080FF";
	public const string WrongHexColor = "#FFC300FF";
}
#endregion HexColor

#region LocalPush
public static partial class Global
{
	public const string LocalPushEnable = "LOCAL_PUSH_ENABLE";
	public const string LocalPushBlockEnable = "LOCAL_PUSH_BLOCK_ENABLE";
	public const string LocalPushBlockStartHour = "LOCAL_PUSH_BLOCK_START";
	public const string LocalPushBlockEndHour = "LOCAL_PUSH_BLOCK_END";
}
#endregion LocalPush

#region Path
public static partial class Global
{
	public readonly static string ProjectPath = Directory.GetCurrentDirectory();
	public readonly static string ProjectParentPath = Path.GetFullPath(Path.Combine(ProjectPath,".."));

	public readonly static string DocumentFolderPath = Path.Combine(ProjectParentPath,"Documents");
	public readonly static string ToolFolderPath = Path.Combine(ProjectParentPath,"Tools");


	#region Config
	public readonly static string ConfigFolderPath = Path.Combine(DocumentFolderPath,"Config");
	public readonly static string CustomConfigFolderPath = Path.Combine(ConfigFolderPath,"Custom");
	#endregion Config

	#region Proto
	public readonly static string ProtoFolderPath = Path.Combine(DocumentFolderPath,"Proto");
	#endregion Proto

	#region Lingo
	public readonly static string LingoFolderPath = Path.Combine(DocumentFolderPath,"Lingo");
	#endregion Lingo
}
#endregion Path

#region Graphic Option
public static partial class Global
{
	public const string GlobalTextureMipmapLimit = "GlobalTextureMipmapLimit";
	public const string AnisotropicFiltering = "AnisotropicFiltering";
	public const string VerticalSyncCount = "VerticalSyncCount";
	public const string DisableCameraFarHalf = "DisableCameraFarHalf";
	public const string LodBias = "LodBias";
	public const string MaximumLODLevel = "MaximumLODLevel";
	public const string AntiAliasing = "AntiAliasing";
	public const string SkinWeights = "SkinWeights";
	public const string ShadowDistance = "ShadowDistance";
	public const string RealtimeReflectionProbes = "RealtimeReflectionProbes";
}
#endregion Graphic Option

#region Menu Item
public static partial class Global
{
	public const int MenuOrderMainSpace = 1000000;
	public const int MenuOrderSubSpace = 50;
}
#endregion Menu Item