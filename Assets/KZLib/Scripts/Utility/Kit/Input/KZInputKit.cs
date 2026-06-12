using KZLib;

/// <summary>
/// Utility methods for globally blocking and unblocking player input.
/// </summary>
public static class KZInputKit
{
	/// <summary>
	/// Blocks input on both InputManager and UIManager when they are available.
	/// </summary>
	public static void LockInput()
	{
		_SetInput(true);
	}

	/// <summary>
	/// Unblocks input on both InputManager and UIManager when they are available.
	/// </summary>
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
