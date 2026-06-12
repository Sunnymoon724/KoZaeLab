using System;

namespace KZLib.Actors
{
	public abstract class Structure<TState,TStat> : Actor<TState,TStat> where TState : struct,Enum where TStat : struct,Enum
	{
		
	}
}