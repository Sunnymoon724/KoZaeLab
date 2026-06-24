namespace KZLib.UI
{
	/// <summary>
	/// Base class for full-screen or embedded 2D panel windows.
	/// </summary>
	public abstract class BasePanel : Window2D
	{
		public override WindowPrefabType WindowType => WindowPrefabType.Panel;
	}
}