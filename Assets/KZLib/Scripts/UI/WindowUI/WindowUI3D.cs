
public abstract class WindowUI3D : WindowUI
{
	private const string c_screenUI = "ScreenUI";

	public override bool IsBlocked => true; // UI3D does not have input

	public override bool Is3D => true;

	public override bool IsPooling => true;
	public override WindowUIType WindowType => WindowUIType.Panel;
	public override UIPriorityType PriorityType => UIPriorityType.Middle;

	public override void Open(object param)
	{
		base.Open(param);

		gameObject.SetAllLayer(c_screenUI.FindLayerByName(true));
	}

	public override void BlockInput(bool isBlocked) { }
}