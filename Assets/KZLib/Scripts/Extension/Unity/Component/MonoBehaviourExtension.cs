#if UNITY_EDITOR
using UnityEngine;

public static class MonoBehaviourExtension
{
	public static void StartRunInEditMode(this MonoBehaviour behaviour)
	{
		if(!_IsValid(behaviour))
		{
			return;
		}

		behaviour.runInEditMode = true;
	}
	
	public static void StopRunInEditMode(this MonoBehaviour behaviour)
	{
		if(!_IsValid(behaviour))
		{
			return;
		}

		var enabled = behaviour.enabled;

		if(enabled)
		{
			behaviour.enabled = false;
		}

		behaviour.runInEditMode = false;

		if(enabled)
		{
			behaviour.enabled = true;
		}
	}

	private static bool _IsValid(MonoBehaviour behaviour)
	{
		if(!behaviour)
		{
			Logger.System.E("MonoBehaviour is null");

			return false;
		}

		return true;
	}
}
#endif