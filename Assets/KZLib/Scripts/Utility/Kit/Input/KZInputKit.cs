using KZLib;

public static class KZInputKit
{
	public static void LockInput()
	{
		_SetInput(true);
	}

	public static void UnLockInput()
	{
		_SetInput(false);
	}

	private static void _SetInput(bool isBlocked)
	{
		if(InputManager.HasInstance)
		{
			InputManager.In.BlockInput(isBlocked);
		}

		if(UIManager.HasInstance)
		{
			UIManager.In.BlockInput(isBlocked);
		}
	}
}