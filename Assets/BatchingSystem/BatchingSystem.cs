using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eBuildState
{
    Wait,
    Building,
}

public enum eBatchingTargetState
{
    None,
    Add,
    Remove,
}

public class BatchingElement
{
    protected Transform m_originalParent = null;
    protected GameObject m_batchingTargetObject = null;
    protected WorldPropObject m_batchingObject = null;
    protected Mesh m_originalMesh = null;
    protected uint m_elemIndex = 0;
   
    public uint BatchingIdx
    {
        get
        {
            return m_batchingObject.BatchingIdx;
        }
    }

    public uint ElemIndex
    {
        get
        {
            return m_elemIndex;
        }
    }

    public GameObject BatchingTargetObject
    {
        get
        {
            return m_batchingTargetObject;
        }
    }

    public BatchingElement( Transform parent, WorldPropObject batchingTarget, uint id )
    {
        m_batchingTargetObject = batchingTarget.gameObject;
        m_originalParent = m_batchingTargetObject.transform.parent;
        m_batchingObject = batchingTarget;
        m_batchingObject.BatchingIdx = id;// ( ( groupIndex << ( ( sizeof( uint ) / 2 ) * 8 ) ) | elemIndex );
        //m_elemIndex = elemIndex;
    }

    public void Destroy()
    {
        if ( m_batchingTargetObject != null )
        {
            m_batchingTargetObject.transform.SetParent( m_originalParent, true );
        }
    }
}

public class BatchingController
{
    protected Material m_material = null;
    protected GameObject m_batchingParent = null;
    protected Dictionary<int, CombineGroup> m_combineGroupList = new Dictionary<int, CombineGroup>();
    protected SortedDictionary<uint, BatchingElement> m_batchingElems = new SortedDictionary<uint, BatchingElement>();

    public BatchingController( GameObject batchingObject, Transform root )
    {
        m_batchingParent = new GameObject();
        MeshRenderer meshRenderer = batchingObject.GetComponent<MeshRenderer>();
        WorldPropObject batching = batchingObject.GetComponent<WorldPropObject>();
        if ( meshRenderer != null && batching != null )
        {
            m_material = meshRenderer.material;

            Transform elemAddTransform = null;
            int lightMapIndex = meshRenderer.lightmapIndex;
            if ( m_combineGroupList.ContainsKey( lightMapIndex ) )
            {
                m_combineGroupList[meshRenderer.lightmapIndex].AddObject( batchingObject );
            }
            else
            {
                CombineGroup newCombineGroup = new CombineGroup( batchingObject, m_batchingParent.transform );
                elemAddTransform = newCombineGroup.AddObject( batchingObject );
                m_combineGroupList.Add( lightMapIndex, newCombineGroup );
            }
            BatchingElement firstElem = new BatchingElement( elemAddTransform, batching, batching.BatchingIdx );
            m_batchingElems.Add( firstElem.BatchingIdx, firstElem );
            m_batchingParent.name = m_material.name + "_batching_parent";
        }
        else
        {
            Log.System.E( "not find mesh renderer" );
        }
        m_batchingParent.transform.SetParent( root, true );
    }

    public void Add( GameObject target )
    {
        WorldPropObject batching = target.GetComponent<WorldPropObject>();
        if ( batching != null )
        {
            MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
            Transform elemAddTransform = null;
            int lightMapIndex = meshRenderer.lightmapIndex;
            if ( m_combineGroupList.ContainsKey( lightMapIndex ) )
            {
                m_combineGroupList[meshRenderer.lightmapIndex].AddObject( target );
            }
            else
            {
                CombineGroup newCombineGroup = new CombineGroup( target, m_batchingParent.transform );
                elemAddTransform = newCombineGroup.AddObject( target );
                m_combineGroupList.Add( lightMapIndex, newCombineGroup );
            }

            BatchingElement elem = new BatchingElement( elemAddTransform, batching, batching.BatchingIdx );
            m_batchingElems.Add( elem.BatchingIdx, elem );
        }
    }

    public void Remove( GameObject target )
    {
        WorldPropObject batchingIndex = target.GetComponent<WorldPropObject>();
        if ( batchingIndex != null && m_batchingElems.ContainsKey( batchingIndex.BatchingIdx ) )
        {

            m_batchingElems[batchingIndex.BatchingIdx].Destroy();
            m_batchingElems.Remove( batchingIndex.BatchingIdx );

            foreach ( CombineGroup cur in m_combineGroupList.Values )
            {
                cur.RemoveObject( target );
            }
            // 갯수에 따른 처리를 여기서 했었음..상태변경
            //if ( m_batchingElems.Count > 1 )
        }
    }

    public int GetGroupCount()
    {
        return m_combineGroupList.Count;
    }

    public bool IsBatched( GameObject target )
    {
        return false;
    }

    public void Destroy()
    {
        foreach ( BatchingElement cur in m_batchingElems.Values )
        {
            if ( cur != null )
            {
                cur.Destroy();
            }
        }
        m_batchingElems.Clear();

        foreach ( CombineGroup cur in m_combineGroupList.Values )
        {
            if ( cur != null )
            {
                cur.Destroy();
            }
        }
        m_combineGroupList.Clear();
        if ( m_batchingParent != null )
        {
            m_batchingParent.transform.parent = null;
            GameObject.Destroy( m_batchingParent );
        }
    }

    public IEnumerator coBuild()
    {
        if ( m_batchingElems.Count >= 1 )
        {
            if ( m_combineGroupList.Count > 0 )
            {
                foreach ( CombineGroup cur in m_combineGroupList.Values )
                {
                    yield return cur.Combine();
                }
            }
        }
    }
}

public class CombineGroup
{
    protected int m_lightMapIndex = 0;
    private bool needRebuild = true;
    protected Material m_material = null;
    protected Transform m_parent = null;
    protected Dictionary<int, WorldPropMeshCombine> m_combineMeshes = new Dictionary<int, WorldPropMeshCombine>();

    public CombineGroup( GameObject firstObject, Transform parent )
    {
        MeshRenderer meshRenderer = firstObject.GetComponent<MeshRenderer>();
        WorldPropObject worldProp = firstObject.GetComponent<WorldPropObject>();
        m_material = worldProp.BatchingMaterial;
        m_lightMapIndex = meshRenderer.lightmapIndex;
        m_parent = parent;
    }

    public IEnumerator Combine()
    {
        if ( needRebuild )
        {
			Dictionary<int, WorldPropMeshCombine>.Enumerator iter = m_combineMeshes.GetEnumerator();
			while ( iter.MoveNext() )
            {
                iter.Current.Value.Combine();
                yield return null;
            }
            needRebuild = false;
        }
    }

    public Transform AddObject( GameObject batchingObject )
    {
        WorldPropObject batching = batchingObject.GetComponent<WorldPropObject>();
        Transform addGroupTransform = null;
        if ( batching != null )
        {
            if ( !m_combineMeshes.ContainsKey( batching.BatchingVolumeIndex ) )
            {
                NewCombineGroup( batchingObject );
            }
            m_combineMeshes[batching.BatchingVolumeIndex].AddCombineElem( batchingObject );
            addGroupTransform = m_combineMeshes[batching.BatchingVolumeIndex].transform;
            needRebuild = true;
        }

        return addGroupTransform;
    }

    public void RemoveObject( GameObject batchingObject )
    {
        WorldPropObject batching = batchingObject.GetComponent<WorldPropObject>();
        if ( batching != null )
        {
            if ( m_combineMeshes.ContainsKey( batching.BatchingVolumeIndex ) )
            {
                needRebuild = true;
                m_combineMeshes[batching.BatchingVolumeIndex].RemoveCombineElem( batchingObject );
                if ( m_combineMeshes[batching.BatchingVolumeIndex].IsEmpty )
                {
                    WorldPropMeshCombine cur = m_combineMeshes[batching.BatchingVolumeIndex];
                    m_combineMeshes.Remove( batching.BatchingVolumeIndex );
                    cur.CleanUp();
                    GameObject.Destroy( cur.gameObject );
                }
            }
        }
    }

    public void Destroy()
    {
        Dictionary<int, WorldPropMeshCombine>.Enumerator iter = m_combineMeshes.GetEnumerator();
        while ( iter.MoveNext() )
        {
            if ( iter.Current.Value != null )
            {
                iter.Current.Value.CleanUp();
                GameObject.Destroy( iter.Current.Value.gameObject );
            }
        }
        m_combineMeshes.Clear();
    }

    protected WorldPropMeshCombine NewCombineGroup( GameObject batchingObject )
    {
        WorldPropObject worldPropObject = batchingObject.GetComponent<WorldPropObject>();
        GameObject combineGroupObj = new GameObject();
        combineGroupObj.name = string.Format( "lightmap_index_{0}_volume_index_{1}_", m_lightMapIndex, worldPropObject.BatchingVolumeIndex ) + m_material.name;
        WorldPropMeshCombine combineGroup = combineGroupObj.GetOrAddComponent<WorldPropMeshCombine>();
        MeshRenderer meshRenderer = batchingObject.GetComponent<MeshRenderer>();
        combineGroup.Init( m_lightMapIndex, meshRenderer );

        combineGroup.SetMaterial( meshRenderer.material );
        combineGroup.AddCombineElem( batchingObject );

        m_combineMeshes.Add( worldPropObject.BatchingVolumeIndex, combineGroup );
        combineGroupObj.transform.SetParent( m_parent );

        return combineGroup;
    }
}

public class BatchingSystem
{

    public class WaitBatching
    {
        public GameObject m_object = null;
        public eBatchingTargetState m_state = eBatchingTargetState.None;
        public WaitBatching( GameObject obj, eBatchingTargetState newState )
        {
            m_object = obj; m_state = newState;
        }
    }

    protected eBuildState m_buildState = eBuildState.Wait;
    protected GameObject m_batchingRoot = null;
    protected Dictionary<string, BatchingController> m_batchingControllers = new Dictionary<string, BatchingController>();
    protected Dictionary<GameObject, WaitBatching> m_waitList = new Dictionary<GameObject, WaitBatching>();
    protected Dictionary<int, GameObject> m_objectDic = new Dictionary<int, GameObject>();
    private List<WorldPropObject> addedObjList = new List<WorldPropObject>();
    private List<WorldPropObject> removedObjList = new List<WorldPropObject>();
    private string strInstance = " (Instance)";
    public void Add( GameObject target )
    {
        WaitBatching waitObj = null;
        if ( m_waitList.TryGetValue( target, out waitObj ) )
            waitObj.m_state = eBatchingTargetState.Add;
        else
        {
            waitObj = new WaitBatching( target, eBatchingTargetState.Add );
            m_waitList.Add( target, waitObj );
        }
    }

    public void Remove( GameObject target )
    {
        WaitBatching waitObj = null;
        if ( m_waitList.TryGetValue( target, out waitObj ) )
            waitObj.m_state = eBatchingTargetState.Remove;
        else
        {
            waitObj = new WaitBatching( target, eBatchingTargetState.Remove );
            m_waitList.Add( target, waitObj );
        }
    }

    public bool IsBatched( GameObject target )
    {
        if ( m_objectDic.ContainsKey( target.GetInstanceID() ) )
            return true;
        return false;
    }

    void StartBatch()
    {
        m_buildState = eBuildState.Building;
        if ( m_batchingRoot == null )
        {
            m_batchingRoot = new GameObject();
            m_batchingRoot.name = "batching_root";
        }
        foreach ( WaitBatching data in m_waitList.Values )
        {
            // 인스턴스 아이디기준으로 중복되는 오브젝트는 거름
            if ( data.m_state == eBatchingTargetState.Add && !IsBatched( data.m_object ) )
            {
                ProcessAdd( data.m_object );
            }
            else if ( data.m_state == eBatchingTargetState.Remove && IsBatched( data.m_object ) )
            {
                ProcessRemove( data.m_object );
            }
        }
        m_waitList.Clear();
    }

    void ProcessAdd( GameObject target )
    {
        WorldPropObject batching = target.GetComponent<WorldPropObject>();
        if ( batching != null )
        {
            addedObjList.Add( batching );
            m_objectDic.Add( target.GetInstanceID(), target );
            // 배칭용 머티리얼로 바꾸고, 이름에서 인스턴스뗀후 키로 사용
            batching.BatchingPrepare();
            MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
            MeshFilter meshFilter = target.GetComponent<MeshFilter>();
            if ( meshRenderer != null && meshRenderer.material != null && meshFilter != null )
            {
                string materialName = meshRenderer.material.name;
                
                int removeBeg = materialName.IndexOf( strInstance );
                if ( removeBeg != -1 )
                {
                    materialName = materialName.Remove( removeBeg );
                }
                if ( m_batchingControllers.ContainsKey( materialName ) )
                {
                    m_batchingControllers[materialName].Add( target );
                }
                else
                {
                    BatchingController unit = new BatchingController( target, m_batchingRoot.transform );
                    m_batchingControllers.Add( materialName, unit );
                }
            }
            else
                Log.System.I( "batching add failed : ",meshRenderer,meshFilter);
        }
    }

    void ProcessRemove( GameObject target )
    {
        MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
        if ( meshRenderer != null )
        {
            if ( meshRenderer.material != null )
            {
                m_objectDic.Remove( target.GetInstanceID() );
                removedObjList.Add( target.GetComponent<WorldPropObject>() );
                string materialName = meshRenderer.material.name;
                
                int removeBeg = materialName.IndexOf( strInstance );
                if ( removeBeg != -1 )
                {
                    materialName = materialName.Remove( removeBeg );
                }

                if ( m_batchingControllers.ContainsKey( materialName ) )
                {
                    m_batchingControllers[materialName].Remove( target );

                    if ( m_batchingControllers[materialName].GetGroupCount() == 0 )
                    {
                        m_batchingControllers[materialName].Destroy();
                        m_batchingControllers.Remove( materialName );
                    }
                }
            }
        }
    }

    public eBuildState BuildState
    {
        get
        {
            return m_buildState;
        }
    }

    public void Destroy()
    {
        foreach ( BatchingController cur in m_batchingControllers.Values )
        {
            if ( cur != null )
            {
                cur.Destroy();
            }
        }
        m_batchingControllers.Clear();
        m_waitList.Clear();
        m_objectDic.Clear();
        addedObjList.Clear();
        removedObjList.Clear();

        GameObject.Destroy( m_batchingRoot );
    }

    public IEnumerator BuildCoroutine()
    {
        while ( true )
        {
            if ( m_buildState == eBuildState.Wait )
            {
                StartBatch();
                foreach ( KeyValuePair<string, BatchingController> pair in m_batchingControllers )
                    yield return pair.Value.coBuild();
                yield return null;

                // 로딩하지 않는 재접 기다린다
                // while (NetworkManager.Instance.p_ReConnect == 2)
                // {
                //     yield return null;
                // }

                m_buildState = eBuildState.Wait;
                foreach ( WorldPropObject prop in addedObjList )
                    prop.OnTransitionEnd( WorldPropObject.WorldPropState.BatchingIn );
                foreach ( WorldPropObject prop in removedObjList )
                    prop.OnTransitionEnd( WorldPropObject.WorldPropState.BatchingOut );
                addedObjList.Clear();
                removedObjList.Clear();
            }
            yield return null;
        }
    }
}