// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public class InputManager : SingletonMB<InputManager>
// {
//     public enum eSKILL_BUTTON
//     {
//         NormalAttack = 0,
//         Dodge,
//         SpecialAttack,
//     }

// 	#region Touch Area
// 	private class TouchArea
// 	{
// 		public bool bTouch;
// 		public Vector3 vTouchPos;
// 	}
// 	#endregion

// 	#region Constant Variable
// 	private const float BORDER_LEFT     = 0.0f;
//     private const float BORDER_TOP      = 0.0f;
//     private const float BORDER_RIGHT    = 0.0f;
//     private const float BORDER_BOTTOM   = 0.0f;
//     private const float SLEEPMODE_TIME_ONE = 60.0f;
//     private const float SLEEPMODE_TIME_FIVE  = 300.0f;
//     private const float SLEEPMODE_TIME_TEN   = 600.0f;
//     #endregion

//     private VirtualPad m_MoveStick      = null;    
// 	private bool m_bReadyToQuit			= false;

//     private Dictionary<int, TouchArea> m_dicTouchInArea	= new Dictionary<int, TouchArea>();
// 	private List<int> m_listTouchUnit					= new List<int>();

// #if UNITY_EDITOR || (UNITY_STANDALONE_WIN && !UNITY_EDITOR_OSX)
//     private ChangeWindowKeyCode m_Input					= new ChangeWindowKeyCode();
// #endif

//     private GameObject m_objBlockInput = null;
//     public bool bInputBlock { get;  private set; }

//     bool m_IsSpecialSkill = false;
//     public bool p_IsSpecialSkill
//     {
//         get { return m_IsSpecialSkill; }
//         set
//         {
//             if (ContentsOpenManager.IsOpenContentsFromUIType(epic_define.DisplayUIType_e.SpecialCounter) == false)
//             {
//                 m_IsSpecialSkill = false;
//             }
//             else
//             {
//                 m_IsSpecialSkill = value;
//             }
            
//             if(UnitManager.Instance.PC)
//             {
//                 skill_meta.skill_data skillData = SkillManager.Instance.GetExtraSkill(skill_meta.SkillType_e.SpecialDodge);
//                 if(skillData != null)
//                 {
//                     MessageSender.Instance.GameChangeDodgeSkill(m_IsSpecialSkill);
//                 }

//                 skillData = SkillManager.Instance.GetExtraSkill(skill_meta.SkillType_e.SpecialCounter);
//                 if (skillData != null)
//                 {
//                     MessageSender.Instance.GameChangeSpecialCounterSkill(m_IsSpecialSkill);
//                 }
//             }
//         }
//     }

//     bool m_IsWakeUpAttackSkill = false;
//     public bool p_IsWakeUpAttackSkill
//     {
//         get { return m_IsWakeUpAttackSkill; }
//         set
//         {
//             if(ContentsOpenManager.IsOpenContentsFromUIType(epic_define.DisplayUIType_e.WakeUpAttack) == false)
//             {
//                 m_IsWakeUpAttackSkill = false;
//             }
//             else
//             {
//                 m_IsWakeUpAttackSkill = value;
//             }
            
//             if (UnitManager.Instance.PC)
//             {
//                 skill_meta.skill_data skillData = SkillManager.Instance.GetExtraSkill(skill_meta.SkillType_e.WakeUp_Attack);
//                 if (skillData != null)
//                 {
//                     MessageSender.Instance.GameChangeWakeUpAttackSkill(m_IsWakeUpAttackSkill);
//                 }
//             }
//         }
//     }
    
//     bool m_IsAirCounterAttackSkill = false;
//     public bool p_IsAirCounterAttackSkill
//     {
//         get { return m_IsAirCounterAttackSkill; }
//         set
//         {
//             if (ContentsOpenManager.IsOpenContentsFromUIType(epic_define.DisplayUIType_e.AerialCounter) == false)
//             {
//                 m_IsAirCounterAttackSkill = false;
//             }
//             else
//             {
//                 m_IsAirCounterAttackSkill = value;
//             }
            
//             if (UnitManager.Instance.PC)
//             {
//                 skill_meta.skill_data skillData = SkillManager.Instance.GetExtraSkill(skill_meta.SkillType_e.AirealCounter);
//                 if (skillData != null)
//                 {
//                     MessageSender.Instance.GameChangeAirCounterAttackSkill(m_IsAirCounterAttackSkill);
//                 }
//             }
//         }
//     }


//     private bool m_bDashAttack      = false;
//     private Vector3 m_vInputDir     = Vector3.zero;

//     public bool p_IsDashAttack
//     {
//         get { return m_bDashAttack; }
//         set
//         {
//             bool startCoolTime = value;
//             if (ContentsOpenManager.IsOpenContentsFromUIType(epic_define.DisplayUIType_e.DashAttack) == false)
//             {
//                 m_bDashAttack = false;
//                 startCoolTime = true;
//             }
//             else
//             {
//                 m_bDashAttack = value;
//             }
            
//             if (UnitManager.Instance.PC)
//             {
//                 MessageSender.Instance.GameChangeSkillDashAttackStart(m_IsSpecialSkill, startCoolTime);
//             }
//         }
//     }
    
//     public void InputMoveDir(Vector3 vDir, bool isBlockMove)
//     {
// 		if (UnitManager.Instance == null || UnitManager.Instance.PC == null)
//         {
//             return;
//         }
//         bool bChangeDir = m_vInputDir != vDir;
        
//         m_vInputDir = vDir;
//         if (isBlockMove)
//             return;
        
//         if (vDir != Vector3.zero)
//         {
//             UnitManager.Instance.PC.CommandMove(vDir);

//             // 방향을 바꿨을 때만 타겟팅을 지우고 다시 찾는다.
//             //if (bChangeDir == true)
//             //{
//             //    if ((CameraHandler.Instance.IsZoneLockOnState == false &&
//             //        CameraHandler.Instance.IsUnitLockOnState == false) &&
//             //        UnitManager.Instance.PC.p_UnitState.IsMoveAble())
//             //    {
//             //        UnitManager.Instance.PC.p_TargetProcessor.ClearTarget();
//             //    }
//             //}

//             UnitManager.Instance.PC.p_TargetProcessor.SetTargetAngle(true);
//             CheckClearTarget();
//             FindTarget();

//         }
//         else
//         {
//             UnitManager.Instance.PC.MoveStop();

//             if (bChangeDir == true)
//             {
//                 UnitManager.Instance.PC.p_TargetProcessor.SetTargetAngle(false);
//                 FindTarget();
//             }

//             m_vInputDir = Vector3.zero;
//         }
//     }

//     /// <summary>
//     /// 타겟 클리어가 필요한 경우에만 클리어 한다.
//     /// </summary>
//     private void CheckClearTarget()
//     {
//         //BugId#266308 [M11/live] 콜로세움 타겟팅 오류 대응.
//         //이미타겟 있는경우 클리어 한 후 타겟 찾는게 default.(다만 lockon처리 되어있는 상황에선 클리어하지않는다.(타겟설정되어있으므로).

//         if (UnitManager.Instance.PC.p_TargetProcessor.IsTarget() == false)
//             return;

//         //공격중에도 타겟변경 가능해야 함. > pd님 요청에 따라 추가.
//         if (CameraHandler.Instance.IsZoneLockOnState == true || CameraHandler.Instance.IsUnitLockOnState == true || UnitManager.Instance.PC.p_UnitState.IsTargetAble() == false) // || UnitManager.Instance.PC.p_UnitState.IsMoveAble() == false)
//             return;

//         UnitController unitCtrl = UnitManager.Instance.PC.p_TargetProcessor.TryFindTarget();

//         if (unitCtrl != null && unitCtrl.p_UnitData != null)
//         {
//             if (UnitManager.Instance.PC.p_TargetProcessor.CheckChangedTarget(unitCtrl.p_UnitData.InstantID))
//             {
//                 UnitManager.Instance.PC.p_TargetProcessor.ClearTarget();
//             }
//         }
//     }

//     private void FindTarget()
//     {  
//         if (UnitManager.Instance.PC.p_TargetProcessor.IsTarget() == false)
//         {
//             if(AutoPlayManager.Instance.IsAuto == true)
//             {
//                 AutoPlayManager.Instance.FindAutoTarget();
//             }
//             else
//             {
//                 UnitManager.Instance.PC.p_TargetProcessor.FindTarget();
//             }
//         }
//     }

//     public bool canInputMoveStop()  // 특정값이상 움직이지 않으면 정지하기위한 목적으로 사용함
//     {
//         if (m_vInputDir == Vector3.zero)
//             return true;

//         return false;
//     }

//     public void InputNormalAttack(int nIndex)
//     {
// 		AutoPlayManager.Instance.SetAutoPause(true);

//         FindTarget();
//         if (m_bDashAttack)
// 		{
// 			UnitManager.Instance.PC.CommandDashAttack();
// 		}
//         //else if (p_IsSpecialSkill)
//         //{
//         //    Controller().CommandSpecialCounter();
//         //}
//         else
// 		{
// 			UnitManager.Instance.PC.CommandNormalAttack();
// 		}
// 	}
//     public void InputSpecialSkill()
//     {
//         FindTarget();
//         AutoPlayManager.Instance.SetAutoPause(true);
//         UnitManager.Instance.PC.CommandSpecialSkill();
//     }
//     public void InputSpecialCounter()
//     {
//         AutoPlayManager.Instance.SetAutoPause(true);
//         FindTarget();
//         if (p_IsSpecialSkill)
//         {
//             UnitManager.Instance.PC.CommandSpecialCounter();
//         }
//     }
//     public void InputWakeUpAttack()
//     {
//         AutoPlayManager.Instance.SetAutoPause(true);
//         FindTarget();
//         if (p_IsWakeUpAttackSkill)
//         {
//             UnitManager.Instance.PC.CommandWakeUpAttack();
//         }
//     }

//     public void InputAirCounterAttack()
//     {
//         AutoPlayManager.Instance.SetAutoPause(true);
//         FindTarget();
//         if (p_IsAirCounterAttackSkill)
//         {
//             UnitManager.Instance.PC.CommandAirCounterAttack();
//         }
//     }

//     public void InputDodge()
// 	{
// 		AutoPlayManager.Instance.SetAutoPause(true);
//         FindTarget();
//         if (p_IsSpecialSkill)
//         {
//             UnitManager.Instance.PC.CommandSpecailDodge(m_vInputDir);
//         }
//         else
//         {
//             UnitManager.Instance.PC.CommandDodge(m_vInputDir);
//         }
		
// 	}

//     public void InputMiniGameSkill(int nSkillMetaID)
//     {
//         FindTarget();
//         UnitManager.Instance.PC.CommandMiniGameSkill(nSkillMetaID);
//     }

//     public void InputFishing()
//     {
//         AutoPlayManager.Instance.SetAutoPause(true);
//         UnitManager.Instance.PC.CommandFishing();
//     }

//     public void InputSwap()
//     {
// 		AutoPlayManager.Instance.SetAutoPause(true);

// 		UnitManager.Instance.PC.CommandSwap();
//     }

// 	public void InputActiveAttack(short nIndex)
// 	{
// 		AutoPlayManager.Instance.SetAutoPause(true);

//         if (Main.Instance.DEV.GameMode == Developer.GAME_MODE.TEST)
//         {
//             return;
//         }

//         FindTarget();

//         UnitManager.Instance.PC.CommandActiveSkill(nIndex);
// 	}

// 	public void InputRoleSkill (int nIndex)
// 	{
//         AutoPlayManager.Instance.SetAutoPause(true);

//         FindTarget();
//         UnitManager.Instance.PC.CommandRoleSkill(nIndex);
// 	}

// 	public void InputVehicle()
// 	{
// 		AutoPlayManager.Instance.StopAuto();

// 		UnitManager.Instance.PC.CommandVehicle();
// 	}

// 	public void InputVehicleSkill()
// 	{
//         FindTarget();
//         UnitManager.Instance.PC.CommandVehicleSkill();
// 	}

// 	public void InputItemSlot(int nIndex)
// 	{
// 		UnitManager.Instance.PC.CommandUseItem(nIndex);
// 	}

// 	public void InputGestureSlot(int nMetaId)
// 	{
// 		UnitManager.Instance.PC.CommandGestureSkill(nMetaId);
// 	}

// 	public void InputTurnbackCamera()
//     {
//         CameraHandler.Instance.TurnBackTarget();
//     }

// 	public void InputSit(bool bSit)
// 	{
// 		UnitManager.Instance.PC.CommandSit(bSit);
// 	}

// 	private void OnQuickGame()
// 	{
// 		Application.Quit();
// 	}


//     public Vector3 GetInputDir()
//     {
//         if (GetMoveStick() == null)
//         {
//             return Vector3.zero;
//         }

//         return new Vector3(m_MoveStick.Direction.x, 0.0f, m_MoveStick.Direction.y);
//     }

// 	public Vector3 GetDirectionByPad()
// 	{
// 		Vector3 vCamDir		= CameraHandler.Instance.CamObj.transform.forward;
// 		vCamDir.y			= 0.0f;

// 		Vector3 vPadDir		= GetInputDir();
// 		//if (vPadDir == Vector3.zero)
// 		//{
// 		//	vPadDir			= UnitManager.Instance.PC.p_Transform.forward;
// 		//}
// 		//else
// 		if(vPadDir != Vector3.zero)
// 		{
// 			Quaternion q	= Quaternion.LookRotation(vCamDir) * Quaternion.LookRotation(vPadDir);
// 			vPadDir			= q * Vector3.forward;
// 			vPadDir.Normalize();
// 		}

// 		return vPadDir;
// 	}

//     public bool IsOverUI(Vector3 vPos)
//     {
//         //return UICamera.isOverUI;

//         Camera uiCamera = UIManager.Instance.p_UICamera;
//         Vector3 vViewPort = CameraHandler.Instance.MainCamera.ScreenToViewportPoint(vPos);
//         Ray uiRay = uiCamera.ViewportPointToRay(vViewPort);
//         if (Physics2D.Raycast(uiRay.origin, uiRay.direction, uiCamera.farClipPlane, LayerMask.GetMask("UI")) == true)
//         {
//             return true;
//         }
//         return false;
//     }

//     public bool IsInTouchArea(Vector3 vPosition)
//     {
//         if ((BORDER_LEFT <= Input.mousePosition.x && Input.mousePosition.x <= Screen.width - BORDER_RIGHT) &&
//             BORDER_BOTTOM <= Input.mousePosition.y && Input.mousePosition.y <= Screen.height - BORDER_TOP)
//         {
//             return true;
//         }
//         return false;
//     }

// 	public bool TouchScreenUI(Vector3 vPosition)
// 	{
// 		Camera mainCamera	= CameraHandler.Instance.MainCamera;
// 		Vector3 vViewPort   = mainCamera.ScreenToViewportPoint(vPosition);
//         Ray uiRay           = mainCamera.ViewportPointToRay(vViewPort);

// 		float dist			= mainCamera.farClipPlane - mainCamera.nearClipPlane;
// 		RaycastHit raycastHit;
//         if (Physics.Raycast(uiRay, out raycastHit, dist, LayerMask.GetMask("ScreenUI")) == true)
//         {
// 			raycastHit.collider.gameObject.SendMessage("OnClick", null, SendMessageOptions.DontRequireReceiver);
// 			return true;
//         }
//         return false;
// 	}

// 	public bool TouchStartUnit(Vector3 vPosition, int index)//NPC나 기믹을 클릭하면 자동으로 위치로 이동하게 처리
//     {
// 		m_listTouchUnit.Clear();
// 		if(m_dicTouchInArea.ContainsKey(index) == true)
//         {
//             if(m_dicTouchInArea[index].bTouch == true)
//             {
//                 return false;
//             }
//         }

// 		if(IsSelectUnit() == false)
// 		{
// 			return false;
// 		}
		

// 		Vector3 vViewPort			= CameraHandler.Instance.MainCamera.ScreenToViewportPoint(vPosition);
//         Ray ray						= CameraHandler.Instance.MainCamera.ViewportPointToRay(vViewPort);
// 		RaycastHit[] hitList		= Physics.RaycastAll(ray);
// 		for (int i = 0; i < hitList.Length; ++i)
//         {
//             // 탈것도 root 에는 UnitController 와 연결되어 있어서 탈것을 클릭해도 포함된다
//             RaycastHit hitInfo		= hitList[i];
//             UnitController unitController = hitInfo.collider.transform.root.GetComponent<UnitController>();            
//             if (unitController == null)
// 			{
// 				continue;
// 			}

//             // 내 PC 는 제외함
//             if (m_listTouchUnit.Contains(unitController.p_UnitData.InstantID) == false && UnitManager.Instance.PC != unitController)
//                 m_listTouchUnit.Add(unitController.p_UnitData.InstantID);
// 		}

//         return false;
//     }

// 	private bool IsSelectUnit()
// 	{
//         if(null == PvpManager.Instance)
//             return false;

// 		if (PvpManager.Instance.IsPvpPC() == true)
// 		{
// 			return false;
// 		}

//         // 사진 찍기 모드에서는 이동불가
//         if(UIManager.Instance.IsOpenUI(eUI_TYPE.Picture))
//         {
//             return false;
//         }

// 		return true;
// 	}

// 	public void TouchEndUnit(Vector3 vPosition, int index)
// 	{
// 		if(m_dicTouchInArea.ContainsKey(index) == true)
//         {
//             if(m_dicTouchInArea[index].bTouch == false)
//             {
//                 return;
//             }
//         }

//         bool bSelectOtherUnit = false;
//         Vector3 vViewPort			= CameraHandler.Instance.MainCamera.ScreenToViewportPoint(vPosition);
// 		Ray ray						= CameraHandler.Instance.MainCamera.ViewportPointToRay(vViewPort);
//         RaycastHit[] hitList = Physics.RaycastAll(ray);

//         for (int i = 0; i < hitList.Length; ++i)
// 		{
//             // 탈것도 root 에는 UnitController 와 연결되어 있어서 탈것을 클릭해도 포함된다
//             RaycastHit hitInfo		= hitList[i];
//             UnitController unitController = hitInfo.collider.transform.root.GetComponent<UnitController>();            
//             if (unitController == null)
// 			{
//                 UnitStatusItemTouch pItemTouch = hitInfo.collider.gameObject.GetComponent<UnitStatusItemTouch>();
//                 if(pItemTouch != null)
//                 {
//                     pItemTouch.Touch();
//                     return;
//                 }
//                 continue;
// 			}
           
//             // 터치 업시 터치 시작되었을때 유닛들에서만 터치업이 되게 처리함
//             UnitData unitData = unitController.p_UnitData;
//             if (m_listTouchUnit.Contains(unitData.InstantID) == false)
// 			{
// 				continue;
// 			}

//             if (UnitManager.Instance.IsOtherPC(unitData.InstantID, false))
//             {
//                 bSelectOtherUnit = true;
//                 if (unitController.p_UnitState.IsDead() == false && unitData.UnitMetaData.Not_selectable == false &&
//                     UnitManager.Instance.PC.p_UnitState.canAttackTarget(unitController, true) == true)
//                 {
//                     UnitManager.Instance.PC.p_TargetProcessor.SetTarget(unitController);
//                 }
//             }
//             else if (unitData.UnitMetaData.UnitType == unit_meta.UnitType_e.Npc
//                 || unitData.UnitMetaData.UnitType == unit_meta.UnitType_e.Gimmick
//                 || unitData.UnitMetaData.UnitType == unit_meta.UnitType_e.Crops)
//             {
//                 // 낚시 기믹은 작동금지
//                 if( unitData.UnitMetaData.UnitType == unit_meta.UnitType_e.Gimmick )
//                 {
//                     // 낚시 기믹은 작동금지
//                     gimmick_meta.gimmick_data gimmickMetaData = MetaTable.Instance.Get<gimmick_meta.gimmick_data>(unitData.UnitMetaData.Unit_ex_meta_id);
//                     if( gimmickMetaData.Type == gimmick_meta.GimmickType_e.Fishing || gimmickMetaData.Type == gimmick_meta.GimmickType_e.SeaFishing )
//                     {
//                         continue;
//                     }
//                 }
//                 else if( unitData.UnitMetaData.UnitType == unit_meta.UnitType_e.Npc && unitData.UnitMetaData.Not_selectable )
//                     continue;

//                 //AutoPlayManager.Instance.SetApproachUnit(unitController);
//                 AutoPlayManager.Instance.PlayAuto(new Auto.ApproachContollerData(unitController));
//                 //UnitManager.Instance.PC.OnApproachUnit(unitController);
//                 return;
//             }
//             else if (unitData.UnitMetaData.UnitType == unit_meta.UnitType_e.Monster || unitData.UnitMetaData.UnitType == unit_meta.UnitType_e.Boss)
//             {
//                 if ( !unitController.p_UnitState.IsDead() && !unitData.UnitMetaData.Not_selectable )
//                 {
//                     UnitManager.Instance.PC.p_TargetProcessor.SetTarget(unitController);
//                 }
//             }
//             else if (unitData.UnitMetaData.UnitType == unit_meta.UnitType_e.Prop)
//             {
//                 AutoPlayManager.Instance.SetPropTarget(unitController);
//             }
//         }

// 		if(bSelectOtherUnit == true)
// 		{
//             Data.MiniGame miniGameData = DataManager.Instance.Access<Data.MiniGame>("MiniGame");
//             var miniGameMeta = miniGameData.MiniGameMeta;
//             if (miniGameMeta != null && miniGameMeta.Type == minigame_meta.minigame_type.pet)
//             {
//                 //기절한 유저 살려주기 배달왕
//                 int nOtherPc = m_listTouchUnit.Find(item => UnitManager.Instance.IsOtherPC(item, false));
//                 AutoPlayManager.Instance.PlayAuto(new Auto.ApproachContollerData(UnitManager.Instance.GetUnit(nOtherPc)));
//                 return;
//             }

//             MessageSender.Instance.CharChoiceEnable(m_listTouchUnit.ToArray(), true);
// 		}

// 		m_listTouchUnit.Clear();
// 	}

// 	public void TouchUnit(Vector3 vPosition)
// 	{
// 		Vector3 vViewPort		= CameraHandler.Instance.MainCamera.ScreenToViewportPoint(vPosition);
//         Ray ray					= CameraHandler.Instance.MainCamera.ViewportPointToRay(vViewPort);

// 		RaycastHit[] hits		= Physics.RaycastAll(ray);
// 		for(int i = 0; i < hits.Length; i++)
// 		{
// 			UnitData unitData = hits[i].collider.GetComponentInParent<UnitData>();
//             if (unitData != null)
// 			{
//                 //epic_msg_cheat.req_userinfo 
//                 //	packet		= new epic_msg_cheat.req_userinfo();
//                 //packet.UnitId	= data.InstantID;
//                 //packet.PosX = hits[i].collider.gameObject.transform.position.x;
//                 //packet.PosY = hits[i].collider.gameObject.transform.position.y;
//                 //packet.PosZ = hits[i].collider.gameObject.transform.position.z;
//                 //NetworkManager.Instance.Send(epic_msg.Id_e.req_userinfo, packet);

//                 NetworkManager.Instance.cq_chatting_1("requserinfo",
//                     unitData.InstantID,
//                     unitData.gameObject.transform.position.x,
//                     unitData.gameObject.transform.position.y,
//                     unitData.gameObject.transform.position.z,
//                     unitData.gameObject.transform.rotation.eulerAngles.y);
//                 break;
// 			}
// 		}
// 	}

//     public void TouchLobbyCharacter(Vector3 vPosition)
//     {
// 		//Vector3 vScreenPos = new Vector3(vPosition.x, vPosition.y, -1f);

// 		//Ray RayToCharacter = LobbyManager.Instance.LobbyCamera.ScreenPointToRay(vScreenPos);
// 		//RaycastHit HitInfo;

// 		//if (Physics.Raycast(RayToCharacter, out HitInfo) == false)
// 		//{
// 		//	return;
// 		//}


// 		//UnitLobby unit = HitInfo.collider.GetComponent<UnitLobby>();

// 		//if (unit == null)
// 		//{
// 		//	return;
// 		//}

// 		//unit.Select();
// 	}




//     #region Process Touch State

//     public bool TouchStart(Vector3 vTouch, int index)
//     {
//         MessageSender.Instance.MenuHideAssetTooltip();
//         MessageSender.Instance.CommonTooltipHide();
        
//         if (m_dicTouchInArea.ContainsKey(index) == false)
//         {
// 			TouchArea touchArea	= new TouchArea();
// 			touchArea.bTouch	= false;
// 			touchArea.vTouchPos	= vTouch;

// 			m_dicTouchInArea.Add(index, touchArea);
// 		}

//         if (IsOverUI(vTouch) == true)
//         {
//             m_dicTouchInArea[index].bTouch	= false;
//             return false;
//         }
// 		eSCENE_TYPE eSceneType  = SceneManager.Instance.GetSceneType();

// 		m_dicTouchInArea[index].vTouchPos	= vTouch;

// 		if (eSceneType == eSCENE_TYPE.Lobby)
//         {
// 			TouchLobbyCharacter(vTouch);
//         }
//         else if (eSceneType == eSCENE_TYPE.TestInGame ||
//                  eSceneType == eSCENE_TYPE.InGame)
//         {
// 			//if(TouchScreenUI(vTouch) == true)
// 			//{
// 			//	m_dicTouchInArea[index].bTouch	= false;
// 			//             return false;
// 			//}
// 			if (TouchStartUnit(vTouch, index) == true)
//             {
//                 m_dicTouchInArea[index].bTouch	= false;
//                 return false;
//             }
// 			if (CameraHandler.Instance != null && IsInTouchArea(vTouch) == true)
//             {
//                 CameraHandler.Instance.Lock();
//                 m_dicTouchInArea[index].bTouch	= true;
//                 //Message.MenuToggle message;
//                 //MessageManager.Instance.ExcuteMessage(message);
//                 MessageSender.Instance.MenuToggle();
//             }
//         }
//         return true;
//     }

//     public void TouchStay(Vector3 vTouch, int index)
//     {
// 		if (m_dicTouchInArea.ContainsKey(index) == true)
//         {
//             if (m_dicTouchInArea[index].bTouch == true)
//             {
//                 if (UnitManager.Instance.PC != null)
//                 {
//                     float fRotate_Y = vTouch.x - m_dicTouchInArea[index].vTouchPos.x;
//                     float fRotate_X = vTouch.y - m_dicTouchInArea[index].vTouchPos.y;

//                     CameraHandler.Instance.UpdateRotateY(fRotate_Y * 0.1f);
//                     CameraHandler.Instance.UpdateRotateX(fRotate_X * 0.1f);
// 				}

// 				m_dicTouchInArea[index].vTouchPos	= vTouch;
// 			}
//         }
//     }

//     public void TouchLeave(Vector3 vTouch, int index)
//     {
// 		eSCENE_TYPE eSceneType		= SceneManager.Instance.GetSceneType();
// 		if(eSceneType == eSCENE_TYPE.TestInGame || eSceneType == eSCENE_TYPE.InGame)
// 		{
// 			TouchEndUnit(vTouch, index);
// 		}

//         if (m_dicTouchInArea.ContainsKey(index) == true)
//         {
//             if (m_dicTouchInArea[index].bTouch == true)
//             {
//                 CameraHandler.Instance.EndRotate();
// 				m_dicTouchInArea[index].bTouch	= false;
//             }
//         }
// 	}

// 	public void TouchWheel(float fScrollWheel)
// 	{
// 		eSCENE_TYPE eSceneType		= SceneManager.Instance.GetSceneType();
// 		if(eSceneType == eSCENE_TYPE.TestInGame || eSceneType == eSCENE_TYPE.InGame)
// 		{
// 			CameraHandler.Instance.UpdateZoom(fScrollWheel * 1f);
// 		}
// 	}

// 	public void TouchMultiStay(Touch[] arrTouch)
// 	{
// 		if(arrTouch == null || arrTouch.Length < 2)
// 		{
// 			return;
// 		}

// 		int[] arrTouchIndex			= new int[2];
// 		int nIndex					= 0;
// 		for(int i = 0; i < arrTouch.Length; ++i)
// 		{
// 			if(nIndex >= arrTouchIndex.Length)
// 			{
// 				break;
// 			}

// 			if(m_dicTouchInArea.ContainsKey(arrTouch[i].fingerId) == false)
// 			{
// 				continue;
// 			}
//             if (m_dicTouchInArea.ContainsKey(nIndex) == false)
//             {
//                 continue;
//             }
//             if (arrTouch[i].phase != TouchPhase.Moved && arrTouch[i].phase != TouchPhase.Stationary)
// 			{
// 				continue;
// 			}

//             arrTouchIndex[nIndex] = arrTouch[i].fingerId;
//             m_dicTouchInArea[nIndex].vTouchPos	= arrTouch[i].position;
// 			nIndex++;
// 		}


//         if (nIndex >= 2)
//         {
//             int nTouchIndex1 = arrTouchIndex[0];
//             int nTouchIndex2 = arrTouchIndex[1];
//             int index1 = -1;
//             int index2 = -1;
//             for(int i = 0; i < arrTouch.Length; ++i)
//             {
//                 if( arrTouch[i].fingerId == nTouchIndex1 )
//                 {
//                     index1 = i;
//                 }
//                 else if (arrTouch[i].fingerId == nTouchIndex2)
//                 {
//                     index2 = i;
//                 }
//             }
//             if(index1 > -1 && index2 > -1)
//             {
//                 float fDistance = Vector2.Distance(arrTouch[index1].position, arrTouch[index2].position);
//                 float fPrevDistance = Vector3.Distance((arrTouch[index1].position - arrTouch[index1].deltaPosition), (arrTouch[index2].position - arrTouch[index2].deltaPosition));

//                 float fDelta = fDistance - fPrevDistance;

//                 CameraHandler.Instance.UpdateZoom(fDelta * 0.005f);
//             }
//         }
//     }
//     #endregion

// 	public bool IsMultiTouchInArea()
// 	{
// 		int nCount				= 0;
// 		Dictionary<int, TouchArea>.Enumerator
// 			iter				= m_dicTouchInArea.GetEnumerator();
// 		while(iter.MoveNext() == true)
// 		{
// 			if(iter.Current.Value.bTouch == true)
// 			{
// 				nCount++;
// 			}
// 		}
// 		return nCount >= 2;
// 	}

// 	private VirtualPad GetMoveStick()
//     {
//         if (m_MoveStick == null)
//         {
// 			InGameInfoUI inGameInfo = UIManager.Instance.GetUI(eUI_TYPE.InGameInfo) as InGameInfoUI;
//             if (inGameInfo != null)
//             {
//                 m_MoveStick = inGameInfo.MoveStick;
//             }
//             else
//             {
//                 MiniGameInfoUI miniUI = UIManager.Instance.GetUI(eUI_TYPE.MiniGameInfo) as MiniGameInfoUI;

//                 if (miniUI != null)
//                 {
//                     m_MoveStick = miniUI.MoveStick;
//                 }
//                 else
//                 {
//                     Prologue_InGameInfoUI prologueInGameInfo = UIManager.Instance.GetUI(eUI_TYPE.PrologueInGmaInfoUI) as Prologue_InGameInfoUI;
//                     if (prologueInGameInfo != null)
//                     {
//                         m_MoveStick = prologueInGameInfo.MoveStick;
//                     }
//                 }
//             }
//         }

//         return m_MoveStick;
//     }

//     public void SetInputIME(bool bWrite)
//     {
//         TheGlobal.bWriting = bWrite;
// #if UNITY_EDITOR || (UNITY_STANDALONE_WIN && !UNITY_EDITOR_OSX)
//         if(m_Input != null)
//         {
//             m_Input.SetInputIME(bWrite);
//         }
// #endif
//     }

//     public void ForcedDirection(Vector3 vDir)
//     {
//         m_vInputDir = vDir;
//     }

//     Data.SystemOption m_sysOptionData;
//     float m_fSleepModeSeconds = 0f;
//     public void SetSleepMode()
//     {
// #if UNITY_EDITOR
//         if (Main.Instance.DEV.DoNotUseSleepMode == true)
//         {
//             return;
//         }
// #endif

//         if (UnitManager.Instance == null || UnitManager.Instance.PC == null)
//         {
//             return;
//         }

//         if (m_sysOptionData.SleepMode == false)
//         {
//             return;
//         }

//         if (DataManager.Instance.Access<Data.World>("World").ZoneMetaData == null)
//         {
//             return;
//         }

//         if (DataManager.Instance.Access<Data.World>("World").ZoneMetaData.Auto_play == false)
//         {
//             return;
//         }

//         m_fSleepModeSeconds -= Time.deltaTime;
//         if (m_fSleepModeSeconds < 0f)
//         {
//             if (UIManager.Instance.IsOpenUI(eUI_TYPE.SleepMode) == false)
//             {
//                 UIManager.Instance.Open<SleepModeUI, UIEmptyParam>(eUI_TYPE.SleepMode, new UIEmptyParam());
//             }
//         }
//     }

//     public void ResetSleepModeTime()
//     {
//         if (m_sysOptionData == null)
//         {
//             return;
//         }

//         m_fSleepModeSeconds = (float)m_sysOptionData.SleepModeTime;
//     }

//     public void SetSleepModeOption()
//     {
//         m_sysOptionData = DataManager.Instance.Access<Data.SystemOption>("SystemOption");
//         ResetSleepModeTime();
//     }

//     public void BlockInput(bool bBlock)
//     {
//         if (null != m_MoveStick)
//             m_MoveStick.SetBlockMove(bBlock);

//         UIManager.Instance.BlockInput( bBlock );

//         if (m_objBlockInput == null)
//         {
//             m_objBlockInput = new GameObject();
//             m_objBlockInput.transform.SetParent(UIManager.Instance.UIRepository2D.transform);
//             m_objBlockInput.name = "ui_block_input";
//             m_objBlockInput.transform.localScale = Vector3.one;
//             m_objBlockInput.transform.localPosition = Vector3.zero;
//             m_objBlockInput.transform.localRotation = Quaternion.Euler(Vector3.zero);
//             m_objBlockInput.layer = LayerMask.NameToLayer("UI");

//             var panel = m_objBlockInput.AddComponent<UIPanel>();
//             panel.depth = UIManager.CUSTOM_PANEL_DEPTH_BLOCK_UI;

//             GameObject objWidget = new GameObject();
//             objWidget.name = "box";
//             objWidget.layer = LayerMask.NameToLayer("UI");
//             objWidget.transform.SetParent(m_objBlockInput.transform);
//             objWidget.transform.localScale = Vector3.one;
//             objWidget.transform.localPosition = Vector3.zero;
//             objWidget.transform.localRotation = Quaternion.Euler(Vector3.zero);

//             var widget = objWidget.AddComponent<UIWidget>();
//             widget.depth = 1;
//             widget.width = 5000;
//             widget.height = 5000;
//             widget.autoResizeBoxCollider = true;

//             var box2D = objWidget.AddComponent<BoxCollider2D>();
//             box2D.size = new Vector2(widget.width, widget.height);
//         }

//         m_objBlockInput.SetActive(bBlock);
//         UIManager.Instance.SetBackButtonActive(!bBlock);

//         bInputBlock = bBlock;
//     }
// }