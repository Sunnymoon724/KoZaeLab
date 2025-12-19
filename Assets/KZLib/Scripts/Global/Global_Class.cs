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

#region UITag
public class CommonUITag : CustomTag
{
	public static readonly CommonUITag HudPanelUI					= new(nameof(HudPanelUI));

	public static readonly CommonUITag CommonTransitionPanelUI		= new(nameof(CommonTransitionPanelUI));

	public static readonly CommonUITag VideoPanelUI					= new(nameof(VideoPanelUI));
	public static readonly CommonUITag SubtitlePanelUI				= new(nameof(SubtitlePanelUI));
	public static readonly CommonUITag SkipPanelUI					= new(nameof(SkipPanelUI));

	public static readonly CommonUITag DialogBoxPopupUI				= new(nameof(DialogBoxPopupUI));

	public static readonly CommonUITag DownloadPanelUI				= new(nameof(DownloadPanelUI));

	public static readonly CommonUITag LoadingPanelUI				= new(nameof(LoadingPanelUI));

	public CommonUITag(string name) : base(name) { }
}
#endregion UITag

#region NoticeTag
public class CommonNoticeTag : CustomTag
{
	public static readonly CommonNoticeTag None							= new(nameof(None));
	public static readonly CommonNoticeTag DisplayLog					= new(nameof(DisplayLog));

	public static readonly CommonNoticeTag TouchDownPoint				= new(nameof(TouchDownPoint));
	public static readonly CommonNoticeTag TouchHoldingPoint			= new(nameof(TouchHoldingPoint));
	public static readonly CommonNoticeTag TouchUpPoint					= new(nameof(TouchUpPoint));

	public CommonNoticeTag(string name) : base(name) { }
}

public class EmptyNoticeInfo
{
	public static readonly EmptyNoticeInfo Empty = new();

	private EmptyNoticeInfo() { }
}
#endregion NoticeTag

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