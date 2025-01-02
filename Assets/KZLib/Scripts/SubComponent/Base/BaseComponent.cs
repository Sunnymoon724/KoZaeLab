using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR

using UnityEditor;

#endif

public abstract class BaseComponent : SerializedMonoBehaviour
{
	protected virtual bool UseGizmos => false;
	protected virtual string GizmosText => string.Empty;

	protected void Awake()
	{
		Initialize();
	}

	protected void OnDestroy()
	{
		Release();
	}

	protected virtual void Initialize() { }
	protected virtual void Release() { }

	protected virtual void OnEnable() { }
	protected virtual void OnDisable() { }

	protected virtual void Reset() { }

	protected bool IsValid(Object value,string objectText)
	{
		if(!value)
		{
			LogTag.System.E($"{objectText} is null");

			return false;
		}

		return true;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if(!UseGizmos)
		{
			return;
		}

		DrawGizmo();
	}

	protected virtual void DrawGizmo()
	{
		DrawGizmoText(transform.position);
	}

	protected void DrawGizmoText(Vector3 position)
	{
		var style = new GUIStyle();
		style.normal.textColor = Color.white;

		Handles.Label(position,GizmosText,style);
	}

	protected Vector3 CubeSize => transform.lossyScale*10.0f;
#endif
}