using System;
using R3;

public static class R3Utility
{
	public static IDisposable DelayAction(Action _onAction,float _second)
	{
		return Observable.Timer(TimeSpan.FromSeconds(_second)).Subscribe(_ => _onAction?.Invoke());
	}
}