using System;
using R3;

public static partial class CommonUtility
{
	public static IDisposable DelayAction(Action onAction,float second)
	{
		return Observable.Timer(TimeSpan.FromSeconds(second)).Subscribe(_ => onAction?.Invoke());
	}
}