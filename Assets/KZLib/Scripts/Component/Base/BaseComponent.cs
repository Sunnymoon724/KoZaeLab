using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR

using UnityEditor;

#endif

public abstract class BaseComponent : SerializedMonoBehaviour
{
	protected virtual bool UseGizmos => false;
	private bool IsExistText => UseGizmos && !GizmosText.IsEmpty();
	protected virtual string GizmosText => string.Empty;

	protected virtual void Awake() { }

	protected virtual void OnDestroy() { }

	protected virtual void Reset() { }

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
		DrawGizmoText(transform.position,GizmosText);
	}

	protected void DrawGizmoText(Vector3 _position,string _text)
	{
		var style = new GUIStyle();
		style.normal.textColor = Color.white;

		Handles.Label(_position,GizmosText,style);
	}

	protected Vector3 CubeSize => transform.lossyScale*10.0f;
#endif
}