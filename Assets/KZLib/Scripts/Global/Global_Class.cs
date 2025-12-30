using System.ComponentModel;
using System;
using KZLib.KZUtility;
using UnityEngine;
using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class IsExternalInit { }
}

public record MessageInfo(string Header,string Body);

public record NetworkRespondInfo(bool IsUpdate,string Type,string Content);

public record NetworkPacketInfo(int Code,string Message,bool IsEncrypted);

public record UnitStateInfo(Enum PreState,Enum CurState);

#region UINameTag
public class CommonUINameTag : CustomTag
{
	public static readonly CommonUINameTag HudPanelUI					= new(nameof(HudPanelUI));

	public static readonly CommonUINameTag CommonTransitionPanelUI		= new(nameof(CommonTransitionPanelUI));

	public static readonly CommonUINameTag VideoPanelUI					= new(nameof(VideoPanelUI));
	public static readonly CommonUINameTag SubtitlePanelUI				= new(nameof(SubtitlePanelUI));
	public static readonly CommonUINameTag SkipPanelUI					= new(nameof(SkipPanelUI));

	public static readonly CommonUINameTag DialogBoxPopupUI				= new(nameof(DialogBoxPopupUI));

	public static readonly CommonUINameTag DownloadPanelUI				= new(nameof(DownloadPanelUI));

	public static readonly CommonUINameTag LoadingPanelUI				= new(nameof(LoadingPanelUI));

	public CommonUINameTag(string name) : base(name) { }
}
#endregion UINameTag

#region NoticeTag
public class CommonNoticeTag : CustomTag
{
	public static readonly CommonNoticeTag None							= new(nameof(None));
	public static readonly CommonNoticeTag DisplayLog					= new(nameof(DisplayLog));

	public static readonly CommonNoticeTag TouchDownPoint				= new(nameof(TouchDownPoint));
	public static readonly CommonNoticeTag TouchHoldingPoint			= new(nameof(TouchHoldingPoint));
	public static readonly CommonNoticeTag TouchUpPoint					= new(nameof(TouchUpPoint));

	public static readonly CommonNoticeTag ChangedApplicationFocus		= new(nameof(ChangedApplicationFocus));
	public static readonly CommonNoticeTag ChangedDeviceResolution		= new(nameof(ChangedDeviceResolution));

	// public static readonly CommonNoticeTag ChangedRedDotState			= new(nameof(ACTION_INGAME_REFRESH_UI_RED_DOT));

	public CommonNoticeTag(string name) : base(name) { }
}

public class EmptyNoticeInfo
{
	public static readonly EmptyNoticeInfo Empty = new();

	private EmptyNoticeInfo() { }
}
#endregion NoticeTag

#region UIMenuTag
public class CommonUIMenuTag : CustomTag
{
	public static readonly CommonUIMenuTag Exit		= new(nameof(Exit));
	public static readonly CommonUIMenuTag Option	= new(nameof(Option));
	public static readonly CommonUIMenuTag Tutorial	= new(nameof(Tutorial));

	public CommonUIMenuTag(string name) : base(name) { }
}
#endregion UIMenuTag

#region EntryInfo
public interface IEntryInfo
{
	string Name { get; }
	string Description { get; }
	Sprite Icon { get; }
	AudioClip Sound { get; }
	Action<IEntryInfo> OnClicked { get; }
}

public record EntryInfo : IEntryInfo
{
	public string Name { get; }
	public string Description { get; }
	public Sprite Icon { get; }
	public AudioClip Sound { get; }
	public Action<IEntryInfo> OnClicked { get; }

	public EntryInfo(string name,string description,Sprite icon,AudioClip sound,Action<IEntryInfo> onClicked)
	{
		Name = name;
		Description = description;
		Icon = icon;
		Sound = sound;
		OnClicked = onClicked;
	}

	protected EntryInfo(string name,string description,Action<IEntryInfo> onClicked) : this(name,description,null,null,onClicked) { }
	protected EntryInfo(string name,Action<IEntryInfo> onClicked) : this(name,string.Empty,null,null,onClicked) { }
	protected EntryInfo(string name) : this(name,string.Empty,null,null,null) { }
}

public record ListEntryInfo : EntryInfo
{
	public List<IEntryInfo> EntryInfoList { get; }

	public ListEntryInfo(string name,List<IEntryInfo> entryInfoList) : base(name)
	{
		EntryInfoList = new List<IEntryInfo>(entryInfoList);
	}
}

public record DialogEntryInfo(string Name,Action<IEntryInfo> OnClicked) : EntryInfo(Name,OnClicked);
#endregion EntryInfo