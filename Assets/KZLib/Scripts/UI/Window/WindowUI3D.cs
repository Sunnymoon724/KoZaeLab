using UnityEngine;

public abstract class WindowUI3D : WindowUI
{
	private const string HIDE_UI = "HideUI";
	private const string SCREEN_UI = "ScreenUI";

	public override bool Is3D => true;

	public override bool IsPooling => true;
	public override UILayerType Layer => UILayerType.Panel;
	public override UIPriorityType Priority => UIPriorityType.Normal;

	public override void Open(object _param)
	{
		base.Open(_param);

		gameObject.SetAllLayer(CommonUtility.GetLayerByName(SCREEN_UI,true));
	}

	public override void Close()
	{
		base.Close();
	}

	public override void Hide(bool _hide)
	{
		base.Hide(_hide);

		gameObject.SetAllLayer(CommonUtility.GetLayerByName(_hide ? HIDE_UI : SCREEN_UI,true));
	}

	// /// <summary>
	// /// 빌보드 적용 시 사용.
	// /// </summary>
	// public virtual void ChangeTargetCam(Camera _cam) { }
}

public abstract class UIBaseItem3D : BaseComponentUI
{
	// [SerializeField,Range(0.1f, 2.0f)]
	// private float m_UIScale = 0.55f;
	// private Transform m_FollowTarget = null;

	protected virtual void OnUpdate(Transform _cam)
	{
		if(_cam)
		{
			transform.forward = _cam.forward;
		}

		// TODO 카메라
		// if(CameraMgr.In.CurFollowTargetCam == null)
		//     return;

		// TODO 카메라
		// transform.localScale = Vector3.one * Mathf.Lerp(0.5f, 5.0f,CameraMgr.In.CurFollowTargetCam.GetRealBackDistance() / 10.0f)*m_UIScale;
	}

	// public void ChangeTargetCam(Transform _target)
	// {
	// 	m_FollowTarget = _target;
	// }

	// public void Action()
	// {
	// 	if (!m_FollowTarget)
	// 	{   
	// 		if(CameraMgr.In.MainCamera)
	// 			m_FollowTarget = CameraMgr.In.MainCamera.transform;
	// 	}

	// 	OnUpdate(m_FollowTarget);
	// }
}