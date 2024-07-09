using UnityEngine;
using KZLib;

[RequireComponent(typeof(Camera))]
public abstract class BaseCamera : BaseComponent
{
	[SerializeField]
	protected Camera m_Camera = null;

	protected override void Initialize()
	{
		base.Initialize();

		if(CameraMgr.HasInstance)
		{
			CameraMgr.In.AddSubCamera(m_Camera);
		}
	}

	protected override void Release()
	{
		base.Release();

		if(CameraMgr.HasInstance)
		{
			CameraMgr.In.RemoveSubCamera(m_Camera);
		}
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_Camera)
		{
			m_Camera = GetComponent<Camera>();
		}
	}
}