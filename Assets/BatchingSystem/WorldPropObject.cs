using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPropObject : MonoBehaviour
{
    public enum WorldPropDisplayLevel
    {
        LEVLE1 = 0,
        LEVLE2,
        LEVLE3,
        LEVLE4,
        LEVLE5,
    }

    public enum WorldPropState
    {
        Active,
        Opaque,
        ProcessOpaque,
        //WaitFadeIn,
        FadeIn,
        BatchingIn,
        Idle, // 거리안에 있는 경우 타겟 상태
        BatchingOut,
        //WaitFadeOut,
        FadeOut,
        Transparent,
        ProcessTransparent,
        InActive, // 거리밖에 있는 경우 타겟 상태
    }

    public WorldPropState m_targetState = WorldPropState.Active;
    public WorldPropState m_currentState = WorldPropState.Active;

    //static int instanceID = 0;
    uint m_batchingIndex = 0;
    int m_batchingVolumeIndex = 0;
    int m_mapObjID = 0;
    //public int m_instanceID = 0;

    MeshRenderer m_meshRenderer = null;
    Material m_batchingMaterial = null;
    //Mesh m_originMesh = null;
    Material m_originMaterial = null;
    //Material m_activeMaterial = null;

    private string m_resourceName = string.Empty;
    
    public string ResourceName
    {
        get
        {
            return m_resourceName;
        }
        set
        {
            m_resourceName = value;
        }
    }

    public Material BatchingMaterial
    {
        get
        {
            if( m_meshRenderer == null )
                m_meshRenderer = GetComponent<MeshRenderer>();
            return m_batchingMaterial;
        }
    }

    public WorldPropDisplayLevel m_worldPropLevel = WorldPropDisplayLevel.LEVLE1;
    public int WorldPropLevel
    {
        get
        {
            return (int)m_worldPropLevel;
        }
        set
        {
            m_worldPropLevel = (WorldPropDisplayLevel)value;
        }
    }

    public int BatchingVolumeIndex
    {
        get
        {
            return m_batchingVolumeIndex;
        }
        set
        {
            m_batchingVolumeIndex = value;
        }
    }

    public int MapObjectID
    {
        get
        {
            return m_mapObjID;
        }
        set
        {
            m_mapObjID = value;
        }
    }

    public void SaveOrginal( Material baseMaterial = null )
    {
        
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            m_meshRenderer = GetComponent<MeshRenderer>();
            if( baseMaterial != null )
                m_batchingMaterial = baseMaterial;
            else
                m_batchingMaterial = m_meshRenderer.material;
            m_originMaterial = m_meshRenderer.sharedMaterial;
            //m_originMesh = meshFilter.sharedMesh;
        }
    }

    private void OnDestroy()
    {
        m_batchingMaterial = null;
        m_originMaterial = null;
        //m_originMesh = null;
    }

    public void BatchingPrepare()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            m_meshRenderer.material = m_batchingMaterial;
        }
    }

    public void BatchingUndo()
    {
        m_meshRenderer.material = m_originMaterial;
        //MeshFilter meshFilter = GetComponent<MeshFilter>();
        //meshFilter.mesh = m_originMesh;
    }

    public uint BatchingIdx
    {
        get
        {
            return m_batchingIndex;
        }
        set
        {
            m_batchingIndex = value;
        }
    }

    public WorldPropState GetNextState()
    {
        WorldPropState nextState = WorldPropState.Active;
        // ToDo: 빠르게 수정하기 위해서 이렇게 처리함,깔끔하게 구성하려면 별도의 상태기계 구현해야 함 - ckj
        switch ( m_currentState )
        {
            case WorldPropState.Active:
                if ( m_targetState == WorldPropState.Idle )
                    nextState = WorldPropState.FadeIn;
                else if( m_targetState == WorldPropState.InActive )
                    nextState = WorldPropState.FadeOut;
                else if( m_targetState == WorldPropState.Transparent )
                    nextState = WorldPropState.ProcessTransparent;
                break;
            case WorldPropState.ProcessOpaque:
                // 프로세스나 웨이트로 시작하는 진행중이나 대기중인 상태에서는 반대방향으로의 상태전환만 허용
                if ( m_targetState == WorldPropState.Idle )
                    nextState = WorldPropState.ProcessOpaque;
                else if ( m_targetState == WorldPropState.InActive )
                    nextState = WorldPropState.FadeOut;
                else if ( m_targetState == WorldPropState.Transparent )
                    nextState = WorldPropState.ProcessTransparent;
                break;
            case WorldPropState.Opaque:
                if ( m_targetState == WorldPropState.Idle )
                    nextState = WorldPropState.BatchingIn;
                else if ( m_targetState == WorldPropState.InActive )
                    nextState = WorldPropState.FadeOut;
                else if ( m_targetState == WorldPropState.Transparent )
                    nextState = WorldPropState.ProcessTransparent;
                break;
            case WorldPropState.FadeIn:
                if ( m_targetState == WorldPropState.Idle )
                    nextState = WorldPropState.BatchingIn;
                else if ( m_targetState == WorldPropState.InActive )
                    nextState = WorldPropState.FadeOut;
                else if ( m_targetState == WorldPropState.Transparent )
                    nextState = WorldPropState.ProcessTransparent;
                break;
            case WorldPropState.BatchingIn:
                if ( m_targetState == WorldPropState.Idle )
                    nextState = WorldPropState.Idle;
                else if ( m_targetState == WorldPropState.InActive )
                    nextState = WorldPropState.BatchingOut;
                else if ( m_targetState == WorldPropState.Transparent )
                    nextState = WorldPropState.BatchingOut;
                break;
            case WorldPropState.Idle:
                if ( m_targetState == WorldPropState.Idle )
                    nextState = WorldPropState.Idle;
                else if ( m_targetState == WorldPropState.InActive )
                    nextState = WorldPropState.BatchingOut;
                else if ( m_targetState == WorldPropState.Transparent )
                    nextState = WorldPropState.BatchingOut;
                break;
            case WorldPropState.BatchingOut:
                if ( m_targetState == WorldPropState.Idle )
                    nextState = WorldPropState.BatchingIn;
                else if ( m_targetState == WorldPropState.InActive )
                    nextState = WorldPropState.FadeOut;
                else if ( m_targetState == WorldPropState.Transparent )
                    nextState = WorldPropState.ProcessTransparent;
                break;
            case WorldPropState.FadeOut:
                if ( m_targetState == WorldPropState.Idle )
                    nextState = WorldPropState.FadeIn;
                else if ( m_targetState == WorldPropState.InActive )
                    nextState = WorldPropState.InActive;
                else if ( m_targetState == WorldPropState.Transparent )
                    nextState = WorldPropState.ProcessTransparent;
                break;
            case WorldPropState.ProcessTransparent:
                if ( m_targetState == WorldPropState.Idle )
                    nextState = WorldPropState.ProcessTransparent;
                else if ( m_targetState == WorldPropState.InActive )
                    nextState = WorldPropState.FadeOut;
                else if ( m_targetState == WorldPropState.Transparent )
                    nextState = WorldPropState.ProcessTransparent;
                break;
            case WorldPropState.Transparent:
                if ( m_targetState == WorldPropState.Idle )
                    nextState = WorldPropState.Transparent;
                else if ( m_targetState == WorldPropState.InActive )
                    nextState = WorldPropState.InActive;
                else if ( m_targetState == WorldPropState.Opaque )
                    nextState = WorldPropState.ProcessOpaque;
                break;
            case WorldPropState.InActive:
                nextState = WorldPropState.InActive;
                break;
        }
        return nextState;
    }

    public void RequestSetTargetState( WorldPropState state )
    {
        SetTargetState( state );
        // 전환가능한 상태면 상태전환을 시작한다
        switch( m_currentState )
        {
            case WorldPropState.Active:
            case WorldPropState.Idle:
            case WorldPropState.Opaque:
            case WorldPropState.Transparent:
                //if ( state != m_currentState && ( state == WorldPropState.ProcessOpaque || state == WorldPropState.ProcessTransparent ) )
                //    SetState( state );
                OnEndState();
                break;
        }
    }

    void OnStartState()
    {
        switch ( m_currentState )
        {
            case WorldPropState.InActive:
                // MapHandler.Instance.RequestRemove( this );
                break;
        }
    }

    void OnEndState()
    {
        // // 필요하다면 다음 상태를 찾아서 시작한다
        // if ( MapHandler.Instance.IsVisibleProp( this ) )
        // {
        //     if( m_targetState != WorldPropState.Transparent && m_targetState != WorldPropState.Opaque )
        //         SetTargetState( WorldPropState.Idle );
        // }
        // else
        //     SetTargetState( WorldPropState.InActive );

        switch ( m_currentState )
        {
            default:
                SetState( GetNextState() );
                break;
        }
    }

    void SetTargetState( WorldPropState worldProp )
    {
        m_targetState = worldProp;
    }

    // 강제로 상태를 끝내야 할때만 사용한다
    public void SetState( WorldPropState newState )
    {
        m_currentState = newState;
        OnStartState();
    }

    public void OnTransitionEnd( WorldPropState endState )
    {
        if( endState == WorldPropState.Transparent )
        {
            SetState( endState );
        }
        else if( endState == WorldPropState.Opaque )
        {
            SetState( endState );
            SetTargetState( WorldPropState.Idle );
            OnEndState();
        }
        else
            OnEndState();
    }
}
