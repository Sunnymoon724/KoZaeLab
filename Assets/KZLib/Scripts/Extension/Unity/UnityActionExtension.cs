using UnityEngine.Events;

public static class UnityActionExtension
{
	public static bool HasAction(this UnityAction onAction1,UnityAction onAction2)
	{
		if(onAction1 != null && onAction2 != null)
		{
			var actionArray = onAction1.GetInvocationList();

			for(var i=0;i<actionArray.Length;i++)
			{
				var hasMethod = actionArray[i].Method == onAction2.Method;
				var hasTarget = actionArray[i].Target == onAction2.Target;

				if(hasMethod && hasTarget)
				{
					return true;
				}
			}
		}

		return false;
	}

	public static bool HasAction<TValue>(this UnityAction<TValue> onAction1,UnityAction<TValue> onAction2)
	{
		if(onAction1 != null && onAction2 != null)
		{
			var actionArray = onAction1.GetInvocationList();

			for(var i=0;i<actionArray.Length;i++)
			{
				var hasMethod = actionArray[i].Method == onAction2.Method;
				var hasTarget = actionArray[i].Target == onAction2.Target;

				if(hasMethod && hasTarget)
				{
					return true;
				}
			}
		}

		return false;
	}

	public static bool HasAction<TValue1,TValue2>(this UnityAction<TValue1,TValue2> onAction1,UnityAction<TValue1,TValue2> onAction2)
	{
		if(onAction1 != null && onAction2 != null)
		{
			var actionArray = onAction1.GetInvocationList();

			for(var i=0;i<actionArray.Length;i++)
			{
				var hasMethod = actionArray[i].Method == onAction2.Method;
				var hasTarget = actionArray[i].Target == onAction2.Target;

				if(hasMethod && hasTarget)
				{
					return true;
				}
			}
		}

		return false;
	}
}