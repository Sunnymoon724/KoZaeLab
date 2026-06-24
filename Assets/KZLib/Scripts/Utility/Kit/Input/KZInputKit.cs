using KZLib;
using KZLib.Inputs;

/// <summary>
/// Global player-interaction lock for cutscenes, loading, network waits, IAP, etc.
/// Reference-counted <see cref="LockInput"/> / <see cref="UnLockInput"/> pair; blocks while any lock is active.
/// Applies <see cref="InputManager.BlockInput"/> (Input System) and <see cref="UIManager.BlockUI"/> (CanvasGroups).
/// Game code must not call <see cref="InputManager.BlockInput"/> directly.
/// </summary>
public static class KZInputKit
{
	private static int m_lockCount = 0;

	public static bool IsLocked => m_lockCount > 0;

	public static void LockInput()
	{
		m_lockCount++;

		if(m_lockCount == 1)
		{
			_SetInput(true);
		}
	}

	public static void UnLockInput()
	{
		if(m_lockCount <= 0)
		{
			LogChannel.Input.W("UnLockInput was called with no active lock.");

			return;
		}

		m_lockCount--;

		if(m_lockCount == 0)
		{
			_SetInput(false);
		}
	}

	/// <summary>Clears the lock count and unblocks input. Called from <see cref="KZGameKit.ReleaseManager"/>.</summary>
	public static void Reset()
	{
		m_lockCount = 0;

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
			UIManager.In.BlockUI(isBlocked);
		}
	}
}
