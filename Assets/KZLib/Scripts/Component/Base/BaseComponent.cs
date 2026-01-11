using UnityEngine;
using Sirenix.OdinInspector;
using System;

using Object = UnityEngine.Object;

#if UNITY_EDITOR

using UnityEditor;

#endif

public abstract class BaseComponent : SerializedMonoBehaviour
{
	protected virtual bool UseGizmos => false;
	protected virtual string GizmosText => string.Empty;

	protected void Awake()
	{
		_Initialize();
	}

	protected void OnDestroy()
	{
		_Release();
	}

	protected virtual void _Initialize() { }
	protected virtual void _Release() { }

	protected virtual void OnEnable() { }
	protected virtual void OnDisable() { }

	protected virtual void Start() { }

	protected virtual void Reset() { }

	protected bool _IsValidObject(Object value,string objectText)
	{
		if(!value)
		{
			throw new ArgumentNullException(nameof(value),$"{objectText} is null. Object must be assigned."); 
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

		_DrawGizmo();
	}

	protected virtual void _DrawGizmo()
	{
		_DrawGizmoText(transform.position);
	}

	protected void _DrawGizmoText(Vector3 position)
	{
		var style = new GUIStyle();
		style.normal.textColor = Color.white;

		Handles.Label(position,GizmosText,style);
	}

	protected Vector3 CubeSize => transform.lossyScale*10.0f;
#endif
}