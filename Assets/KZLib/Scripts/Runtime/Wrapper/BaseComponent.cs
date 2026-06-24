using System;
using UnityEngine;

using Object = UnityEngine.Object;

#if UNITY_EDITOR

using UnityEditor;

#endif

/// <summary>
/// Common base for Wrapper components that extend Unity built-ins (<see cref="UnityEngine.UI.Button"/>, <see cref="Camera"/>, etc.).
/// </summary>
/// <remarks>
/// <c>Awake</c> and <c>OnDestroy</c> are private and only forward to <see cref="_Initialize"/> and <see cref="_Release"/>.
/// Override those hooks (and <see cref="OnEnable"/> / <see cref="OnDisable"/> / <see cref="Start"/> / <see cref="Reset"/>) in derived classes.
/// </remarks>
public abstract class BaseComponent : MonoBehaviour
{
	/// <summary>When true, draws editor gizmos via <see cref="_DrawGizmo"/>.</summary>
	protected virtual bool UseGizmos => false;

	/// <summary>Label text drawn in the Scene view when <see cref="UseGizmos"/> is true.</summary>
	protected virtual string GizmosText => string.Empty;

	// Unity entry points are sealed here so derived types use _Initialize / _Release instead.
	private void Awake()
	{
		_Initialize();
	}

	private void OnDestroy()
	{
		_Release();
	}

	/// <summary>Override for enable-time setup (event subscribe, refresh, etc.). Call <c>base.OnEnable()</c> first.</summary>
	protected virtual void OnEnable() { }

	/// <summary>Override for disable-time cleanup. Call <c>base.OnDisable()</c> last.</summary>
	protected virtual void OnDisable() { }

	/// <summary>Override for first-frame logic after all <c>Awake</c> calls. Call <c>base.Start()</c> when overriding.</summary>
	protected virtual void Start() { }

	/// <summary>Override for inspector Reset / component add. Assign serialized references here. Call <c>base.Reset()</c> first.</summary>
	protected virtual void Reset() { }

	/// <summary>One-time init from <c>Awake</c>. Override instead of <c>Awake</c>.</summary>
	protected virtual void _Initialize() { }

	/// <summary>Teardown from <c>OnDestroy</c>. Override instead of <c>OnDestroy</c>.</summary>
	protected virtual void _Release() { }

	/// <summary>Throws <see cref="ArgumentNullException"/> when <paramref name="value"/> is null.</summary>
	protected bool _IsValidObject(Object value,string objectText)
	{
		if(!value)
		{
			throw new ArgumentNullException($"{objectText} is null. Object must be assigned.");
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

	/// <summary>Override to customize editor gizmo drawing. Default draws <see cref="GizmosText"/> at this transform.</summary>
	protected virtual void _DrawGizmo()
	{
		_DrawGizmoText(transform.position);
	}

	/// <summary>Draws <see cref="GizmosText"/> at <paramref name="position"/> in the Scene view.</summary>
	protected void _DrawGizmoText(Vector3 position)
	{
		var style = new GUIStyle();
		style.normal.textColor = Color.white;

		Handles.Label(position,GizmosText,style);
	}

	/// <summary>Default gizmo cube size derived from <see cref="Transform.lossyScale"/>.</summary>
	protected Vector3 CubeSize => transform.lossyScale*10.0f;
#endif
}