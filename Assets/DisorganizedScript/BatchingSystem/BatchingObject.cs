using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatchingObject : MonoBehaviour
{
	public enum BATCHING_LEVEL_TYPE { LEVEL1, LEVEL2, LEVEL3, LEVEL4, LEVEL5, }
	public enum BATCHING_STATE_TYPE { Active, Opaque, ProcessOpaque, FadeIn, BatchingIn, Idle, BatchingOut, FadeOut, Transparent, ProcessTransparent, InActive, }

	public BATCHING_STATE_TYPE m_TargetState = BATCHING_STATE_TYPE.Active;
	public BATCHING_STATE_TYPE m_CurrentState = BATCHING_STATE_TYPE.Active;
	public BATCHING_LEVEL_TYPE m_BatchingLevel = BATCHING_LEVEL_TYPE.LEVEL1;

	private MeshRenderer m_MeshRenderer = null;
	private Material m_BatchingMaterial = null;
	private Material m_OriginMaterial = null;
	
	public string ResourceName { get; set; }

	public Material BatchingMaterial
	{
		get
		{
			if(m_MeshRenderer == null)
			{
				m_MeshRenderer = GetComponent<MeshRenderer>();
			}

			return m_BatchingMaterial;
		}
	}

	public int BatchingVolumeIndex { get; private set; }
	public uint BatchingIndex { get; set; }

	public void SaveOriginal(Material _material = null)
	{
		var filter = GetComponent<MeshFilter>();

		if(filter == null)
		{
			return;
		}

		m_MeshRenderer = GetComponent<MeshRenderer>();

		m_BatchingMaterial = _material != null ? _material : m_MeshRenderer.material;
		m_OriginMaterial = m_MeshRenderer.sharedMaterial;
	}

	private void OnDestroy()
	{
		m_BatchingMaterial = null;
		m_OriginMaterial = null;
	}

	public void BatchingPrepare()
	{
		var filter = GetComponent<MeshFilter>();

		if(filter != null)
		{
			m_MeshRenderer.material = m_BatchingMaterial;
		}
	}

	public void BatchingUndo()
	{
		m_MeshRenderer.material = m_OriginMaterial;
	}
	
	public BATCHING_STATE_TYPE GetNextState()
	{
		var nextState = BATCHING_STATE_TYPE.Active;
		
		switch(m_CurrentState)
		{
			case BATCHING_STATE_TYPE.Active:
			{
				if(m_TargetState == BATCHING_STATE_TYPE.Idle)
				{
					nextState = BATCHING_STATE_TYPE.FadeIn;
				}				
				else if(m_TargetState == BATCHING_STATE_TYPE.InActive)
				{
					nextState = BATCHING_STATE_TYPE.FadeOut;
				}
				else if(m_TargetState == BATCHING_STATE_TYPE.Transparent)
				{
					nextState = BATCHING_STATE_TYPE.ProcessTransparent;
				}
				
				break;
			}
			case BATCHING_STATE_TYPE.ProcessOpaque:
			{
				// 프로세스나 웨이트로 시작하는 진행중이나 대기중인 상태에서는 반대방향으로의 상태전환만 허용
				if(m_TargetState == BATCHING_STATE_TYPE.Idle)
				{
					nextState = BATCHING_STATE_TYPE.ProcessOpaque;
				}				
				else if(m_TargetState == BATCHING_STATE_TYPE.InActive)
				{
					nextState = BATCHING_STATE_TYPE.FadeOut;
				}
				else if(m_TargetState == BATCHING_STATE_TYPE.Transparent)
				{
					nextState = BATCHING_STATE_TYPE.ProcessTransparent;
				}

				break;
			}
			case BATCHING_STATE_TYPE.Opaque:
			case BATCHING_STATE_TYPE.FadeIn:
			case BATCHING_STATE_TYPE.BatchingOut:
			{
				if(m_TargetState == BATCHING_STATE_TYPE.Idle)
				{
					nextState = BATCHING_STATE_TYPE.BatchingIn;
				}				
				else if(m_TargetState == BATCHING_STATE_TYPE.InActive)
				{
					nextState = BATCHING_STATE_TYPE.FadeOut;
				}
				else if(m_TargetState == BATCHING_STATE_TYPE.Transparent)
				{
					nextState = BATCHING_STATE_TYPE.ProcessTransparent;
				}

				break;
			}
			case BATCHING_STATE_TYPE.BatchingIn:
			case BATCHING_STATE_TYPE.Idle:
			{
				if(m_TargetState == BATCHING_STATE_TYPE.Idle)
				{
					nextState = BATCHING_STATE_TYPE.Idle;
				}				
				else if(m_TargetState == BATCHING_STATE_TYPE.InActive)
				{
					nextState = BATCHING_STATE_TYPE.BatchingOut;
				}
				else if(m_TargetState == BATCHING_STATE_TYPE.Transparent)
				{
					nextState = BATCHING_STATE_TYPE.BatchingOut;
				}
				
				break;
			}
			case BATCHING_STATE_TYPE.FadeOut:
			{
				if(m_TargetState == BATCHING_STATE_TYPE.Idle)
				{
					nextState = BATCHING_STATE_TYPE.FadeIn;
				}				
				else if(m_TargetState == BATCHING_STATE_TYPE.InActive)
				{
					nextState = BATCHING_STATE_TYPE.InActive;
				}
				else if(m_TargetState == BATCHING_STATE_TYPE.Transparent)
				{
					nextState = BATCHING_STATE_TYPE.ProcessTransparent;
				}

				break;
			}
			case BATCHING_STATE_TYPE.ProcessTransparent:
			{
				if(m_TargetState == BATCHING_STATE_TYPE.Idle)
				{
					nextState = BATCHING_STATE_TYPE.ProcessTransparent;
				}				
				else if(m_TargetState == BATCHING_STATE_TYPE.InActive)
				{
					nextState = BATCHING_STATE_TYPE.FadeOut;
				}
				else if(m_TargetState == BATCHING_STATE_TYPE.Transparent)
				{
					nextState = BATCHING_STATE_TYPE.ProcessTransparent;
				}
				
				break;
			}				
			case BATCHING_STATE_TYPE.Transparent:
			{
				if(m_TargetState == BATCHING_STATE_TYPE.Idle)
				{
					nextState = BATCHING_STATE_TYPE.Transparent;
				}				
				else if(m_TargetState == BATCHING_STATE_TYPE.InActive)
				{
					nextState = BATCHING_STATE_TYPE.InActive;
				}
				else if(m_TargetState == BATCHING_STATE_TYPE.Transparent)
				{
					nextState = BATCHING_STATE_TYPE.ProcessOpaque;
				}
				
				break;
			}
			case BATCHING_STATE_TYPE.InActive:
			{
				nextState = BATCHING_STATE_TYPE.InActive;

				break;
			}
		}
		return nextState;
	}

	public void RequestSetTargetState(BATCHING_STATE_TYPE _state)
	{
		SetTargetState(_state);

		// 전환가능한 상태면 상태전환을 시작한다

		switch(m_CurrentState)
		{
			case BATCHING_STATE_TYPE.Active:
			case BATCHING_STATE_TYPE.Idle:
			case BATCHING_STATE_TYPE.Opaque:
			case BATCHING_STATE_TYPE.Transparent:
				OnEndState();
				break;
		}
	}

	void OnEndState()
	{
		switch(m_CurrentState)
		{
			default:
				SetState(GetNextState());
				break;
		}
	}

	void SetTargetState(BATCHING_STATE_TYPE _state)
	{
		m_TargetState = _state;
	}

	// 강제로 상태를 끝내야 할때만 사용한다
	public void SetState(BATCHING_STATE_TYPE _state)
	{
		m_CurrentState = _state;
	}

	public void OnTransitionEnd(BATCHING_STATE_TYPE _state)
	{
		if(_state == BATCHING_STATE_TYPE.Transparent)
		{
			SetState(_state);
		}
		else if(_state == BATCHING_STATE_TYPE.Opaque)
		{
			SetState(_state);
			SetTargetState( BATCHING_STATE_TYPE.Idle );
			OnEndState();
		}
		else
		{
			OnEndState();
		}
	}
}