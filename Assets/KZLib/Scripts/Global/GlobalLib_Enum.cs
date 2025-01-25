using System;

public enum UILayerType { Panel, PopUp };

public enum UIPriorityType
{
	Backmost,		//
	Back,			//
	Middle,			// default UI
	Fore, 			// fade UI
	Foremost		// info UI
}

public enum SpaceType { xyz, xy, xz };

public enum GraphicQualityPresetType { QualityLowest, QualityLow, QualityMiddle, QualityHigh, QualityHighest, }

[Flags]
public enum BuildPlatformType
{
	None = 0,
	StandaloneOSX = 1<<0, StandaloneWindows = 1<<1, StandaloneWindows64 = 1<<2, StandaloneLinux64 = 1<<3,

	iOS = 1<<4, Android = 1<<5,

	PS4 = 1<<6, PS5 = 1<<7, XboxOne = 1<<8, Switch = 1<<9, GameCoreXboxSeries = 1<<10, GameCoreXboxOne = 1<<11, tvOS = 1<<12, Stadia = 1<<13,
	WebGL = 1<<14,WSAPlayer = 1<<15,
	LinuxHeadlessSimulation = 1<<16, EmbeddedLinux = 1<<17, VisionOS = 1<<18, QNX = 1<<19,

	All = -1,
}