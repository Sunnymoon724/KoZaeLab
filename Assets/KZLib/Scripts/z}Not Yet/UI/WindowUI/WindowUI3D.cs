
public abstract class WindowUI3D : WindowUI
{
	private const string HIDE_UI = "HideUI";
	private const string SCREEN_UI = "ScreenUI";

	public override bool Is3D => true;

	public override bool IsPooling => true;
	public override UILayerType Layer => UILayerType.Panel;
	public override UIPriorityType Priority => UIPriorityType.Normal;

	public override void Open(object _param)
	{
		base.Open(_param);

		gameObject.SetAllLayer(SCREEN_UI.FindLayerByName(true));
	}

	public override void Close()
	{
		base.Close();
	}

	public override void Hide(bool _hide)
	{
		base.Hide(_hide);

		var layerName = _hide ? HIDE_UI : SCREEN_UI;

		gameObject.SetAllLayer(layerName.FindLayerByName(true));
	}
}