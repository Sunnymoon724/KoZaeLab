using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPropMeshCombine : MonoBehaviour
{
    private Dictionary<int, MeshFilter> m_combineTargets = new Dictionary<int, MeshFilter>();
    private List<CombineInstance> m_combineInstances = new List<CombineInstance>();

    private MeshFilter m_meshFilter = null;
    private MeshRenderer m_meshRenderer = null;
    private Material m_material = null;

    const string MESH_NAME_PREFIX = "combine_";

    protected bool m_copyMeshRendererCompated = false;

    bool m_isTerrainLayer = false;

    public bool IsEmpty
    {
        get
        {
            return m_combineTargets.Count == 0;
        }
    }

    public void Init(int lightMapIndex, MeshRenderer srcMeshRenderer)
    {
        m_meshFilter = gameObject.GetOrAddComponent<MeshFilter>();
        // m_meshRenderer = CommonFunc.CopyComponent(srcMeshRenderer, gameObject);
        m_meshRenderer.lightmapIndex = lightMapIndex;
        m_meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        gameObject.layer = LayerMask.NameToLayer("Background_Static");
    }

    public void SetMaterial(Material material)
    {
        m_material = new Material(material);
        m_meshRenderer.material = material;
    }

    public bool AddCombineElem(GameObject addElem)
    {
        if(m_material != null)
        {
            if (addElem.layer == LayerMask.NameToLayer("Background_Terrain"))
            {
                m_isTerrainLayer = true;
            }

            MeshFilter meshFilter = addElem.GetComponent<MeshFilter>();
            Material material = addElem.GetComponent<MeshRenderer>().material;
            
            if (meshFilter != null /*&& meshFilter.mesh != null*/ && !m_combineTargets.ContainsKey(addElem.GetInstanceID()))
            {
                m_combineTargets.Add(addElem.GetInstanceID(), meshFilter);
                addElem.transform.SetParent(transform);
                return true;
            }
        }
        return false;
    }

    public void CleanUp()
    {
        transform.parent = null;
        m_meshFilter.mesh.Clear();
        
        if( m_material )
        {
            Destroy( m_material );
            m_material = null;
        }
        if( m_meshFilter.mesh )
        {
            Destroy( m_meshFilter.mesh );
            m_meshFilter.mesh = null;
        }
        m_meshFilter = null;
    }

    public void RemoveCombineElem(GameObject removeElem)
    {
        if (m_material != null)
        {
            if(m_combineTargets.ContainsKey(removeElem.GetInstanceID()))
            {
                if(m_combineTargets[removeElem.GetInstanceID()] != null)
                {
                    m_combineTargets.Remove(removeElem.GetInstanceID());
                    removeElem.transform.SetParent(null);
                    MeshRenderer curRenderer = removeElem.gameObject.GetComponent<MeshRenderer>();
                    if (curRenderer != null)
                    {
                        curRenderer.enabled = true;
                    }

                    WorldPropObject propObject = removeElem.GetComponent<WorldPropObject>();
                    if (propObject != null)
                    {
                        propObject.BatchingUndo();
                    }
                }
            }
        }
    }


    //TODO : 측정후 코루틴으로 쪼갤 여지 있음 namkh
    public void Combine()
    {
        if(m_combineTargets.Count >= 1 && m_meshFilter != null)
        {
            if(m_isTerrainLayer)
            {
                gameObject.layer = LayerMask.NameToLayer("Background_Terrain");
            }
            if (m_meshFilter.mesh == null)
            {
                m_meshFilter.mesh = new Mesh();
                m_meshFilter.mesh.name = MESH_NAME_PREFIX + m_material.name;
            }
            else
            {
                m_meshFilter.mesh.Clear();
            }
           
            m_combineInstances.Clear();
            long indexCount = 0;
            foreach (MeshFilter cur in m_combineTargets.Values)
            {
                //if(cur != null /*&& cur.mesh != null*/)
                if (cur != null && cur.sharedMesh != null)  //  cur.sharedMesh 이게 null 로 의심되어 체크하게끔 수정함
                {
                    CombineInstance curCombineInstance = new CombineInstance();
                    curCombineInstance.mesh = cur.sharedMesh;//cur.mesh;
                    curCombineInstance.transform = transform.worldToLocalMatrix * cur.transform.localToWorldMatrix;
                    for ( int i = 0; i < curCombineInstance.mesh.subMeshCount; i++ )
                        indexCount += curCombineInstance.mesh.GetIndexCount( i );
                    MeshRenderer curMeshRenderer = cur.GetComponent<MeshRenderer>();
                    // https://console.firebase.google.com/u/0/project/wodn-b1972/crashlytics/app/android:com.nxth.wodn/issues/726abf0f9cbc075d4d3d7950dad3a716?time=last-twenty-four-hours&sessionId=5E7D70DD02AF000168FB069CA596B7D8_DNE_1_v2
                    // 이 이슈가 curMeshRenderer 가 null 일거로 의심이 되어 체크하게끔 수정한다
                    // 만약 이후에도 에러가 난다면 cur.sharedMesh 이게 null 이다
                    if(curMeshRenderer)
                    {
                        curCombineInstance.lightmapScaleOffset = curMeshRenderer.lightmapScaleOffset;
                        curCombineInstance.realtimeLightmapScaleOffset = curMeshRenderer.realtimeLightmapScaleOffset;
                        m_combineInstances.Add(curCombineInstance);
                    }
                }
            }
            
            if( indexCount >= 65535 )
            {
                m_meshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                Log.System.W( $"배칭된 메쉬의 인덱스 갯수가 많습니다. 배경파트 확인해주세요 : {m_meshFilter.gameObject.name} : inedx count : {indexCount}" );
            }
            m_meshFilter.mesh.CombineMeshes(m_combineInstances.ToArray(), true, true, true);
            
            foreach (MeshFilter cur in m_combineTargets.Values)
            {
                if (cur != null)
                {
                    MeshRenderer curRenderer = cur.gameObject.GetComponent<MeshRenderer>();
                    if (curRenderer != null)
                    {
                        curRenderer.enabled = false;
                    }
                }
            }
        }
    }
}
