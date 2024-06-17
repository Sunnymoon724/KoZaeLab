using UnityEngine;
using System.Collections.Generic;
using System;

public class RepositoryUI3D : RepositoryUI
{
	// /// <summary>
	// /// 3dui에서 관리하는 모든 statusUI 빌보드 타겟 카메라 설정해준다.
	// /// </summary>
	// /// <param name="target"></param>
	// public void SetBillboardTargetCam(Camera _camera)
	// {
	// 	if (!_camera)
	// 	{
	// 		return;
	// 	}

	// 	for(var i=0;i<m_OpenedList.Count;i++)
	// 	{
	// 		if(m_OpenedList[i] is WindowUI3D baseUI)
	// 		{
	// 			baseUI.ChangeTargetCam(_camera);
	// 		}
	// 	}
	// }.


	
	// /// <summary>
	// /// 3dui에서 관리하는 모든 statusUI 빌보드 타겟 카메라 설정해준다.
	// /// </summary>
	// /// <param name="target"></param>
	// public void SetBillboardTargetCam(Camera _target)
	// {
	// 	if(_target == null)
	// 	{
	// 		throw new NullReferenceException("타겟이 없습니다.");
	// 	}

	// 	for (int i = 0; i < m_listOpen.Count; i++)
	// 	{
	// 		m_listOpen[i].ChangeTargetCam(target);
	// 	}
	// }

	protected override bool IsValid(WindowUI _window)
	{
		return _window != null && _window.Is3D && _window is WindowUI3D;
	}
}