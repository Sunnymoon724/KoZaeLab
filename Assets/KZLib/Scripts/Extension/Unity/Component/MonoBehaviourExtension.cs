#if UNITY_EDITOR
using UnityEngine;

public static class MonoBehaviourExtension
{
	public static void StartRunInEditMode(this MonoBehaviour _behaviour)
	{
		_behaviour.runInEditMode = true;
	}
	
	public static void StopRunInEditMode(this MonoBehaviour _behaviour)
	{
		var enabled = _behaviour.enabled;

		if(enabled)
		{
			_behaviour.enabled = false;
		}

		_behaviour.runInEditMode = false;

		if(enabled)
		{
			_behaviour.enabled = true;
		}
	}
}
#endif