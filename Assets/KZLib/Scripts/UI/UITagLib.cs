using KZLib.KZData;

public class UITag : CustomTag
{
	public static readonly UITag TransitionPanelUI	= new(nameof(TransitionPanelUI));
	public static readonly UITag VideoPanelUI		= new(nameof(VideoPanelUI));
	public static readonly UITag SubtitlePanelUI	= new(nameof(SubtitlePanelUI));
	public static readonly UITag SkipPanelUI		= new(nameof(SkipPanelUI));
	public static readonly UITag LoadingPanelUI		= new(nameof(LoadingPanelUI));

	public static readonly UITag HudPanelUI			= new(nameof(HudPanelUI));

	public static readonly UITag DialogBoxPopupUI	= new(nameof(DialogBoxPopupUI));

	public static readonly UITag DownloadPanelUI	= new(nameof(DownloadPanelUI));

	public UITag(string name) : base(name) { }
}