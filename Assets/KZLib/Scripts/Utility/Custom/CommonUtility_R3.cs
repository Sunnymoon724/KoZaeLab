using System;
using R3;

public static partial class CommonUtility
{
	public static IDisposable DelayAction(Action onAction,float second)
	{
		void _PlayAction(Unit observer)
		{
			onAction?.Invoke();
		}

		return Observable.Timer(TimeSpan.FromSeconds(second)).Subscribe(_PlayAction);
	}
}