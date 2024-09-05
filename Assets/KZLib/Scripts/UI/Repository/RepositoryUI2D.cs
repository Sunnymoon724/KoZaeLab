using KZLib;

public class RepositoryUI2D : RepositoryUI
{
	protected override bool IsValid(WindowUI _window)
	{
		return _window != null && !_window.Is3D && _window is WindowUI2D;
	}
}