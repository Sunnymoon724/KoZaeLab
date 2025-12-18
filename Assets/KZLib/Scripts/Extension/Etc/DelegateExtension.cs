using System;

public static class DelegateExtension
{
	public static bool HasDelegate(this Delegate onAction1,Delegate onAction2)
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