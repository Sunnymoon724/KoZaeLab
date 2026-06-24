using System;
using R3;

public static partial class KZExternalKit
{
	/// <summary>
	/// Disposes the subscription and clears the reference.
	/// </summary>
	public static void KillSubscription(ref IDisposable subscription)
	{
		if(subscription == null)
		{
			return;
		}

		subscription.Dispose();
		subscription = null;
	}

	/// <summary>
	/// Schedules an action to run after the given delay using an R3 timer observable.
	/// Returns a disposable subscription that can be used to cancel the scheduled action.
	/// </summary>
	public static IDisposable DelayAction(Action onAction,float second)
	{
		void _PlayAction(Unit observer)
		{
			onAction?.Invoke();
		}

		return Observable.Timer(TimeSpan.FromSeconds(second)).Subscribe(_PlayAction);
	}
}