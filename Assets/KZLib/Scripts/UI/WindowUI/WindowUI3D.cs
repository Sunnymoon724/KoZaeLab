
public abstract class WindowUI3D : WindowUI
{
	private const string c_screen_ui = "ScreenUI";

	public override bool IsHidden => m_canvasGroup.alpha == 0;
	public override bool IsInputBlocked => true; // UI3D does not have input

	public override bool Is3D => true;

	public override bool IsPooling => true;
	public override UILayerType LayerType => UILayerType.Panel;
	public override UIPriorityType PriorityType => UIPriorityType.Middle;

	public override void Open(object param)
	{
		base.Open(param);

		gameObject.SetAllLayer(c_screen_ui.FindLayerByName(true));
	}
}