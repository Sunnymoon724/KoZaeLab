using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshCombine : MonoBehaviour
{
	private Dictionary<int,MeshFilter> m_CombineDict = new Dictionary<int,MeshFilter>();
	private List<CombineInstance> m_CombineInstanceList = new List<CombineInstance>();

	private MeshFilter m_MeshFilter = null;
	private MeshRenderer m_MeshRenderer = null;
	private Material m_Material = null;

	private const int MAX_INDEX_COUNT = 65535;

	// bool m_isTerrainLayer = false;

	public bool IsEmpty => m_CombineDict.Count == 0;

	public void Initialize(int _lightMapIndex)
	{
		m_MeshFilter = gameObject.GetOrAddComponent<MeshFilter>();

		m_MeshRenderer.lightmapIndex = _lightMapIndex;
		m_MeshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

		// gameObject.layer = LayerMask.NameToLayer("Background_Static");
	}

	public void SetMaterial(Material _material)
	{
		m_Material = new Material(_material);
		m_MeshRenderer.material = _material;
	}

	public bool AddCombineData(GameObject _data)
	{
		if(m_Material != null)
		{
			// if(_data.layer == LayerMask.NameToLayer("Background_Terrain"))
			// {
			// 	m_isTerrainLayer = true;
			// }

			var meshFilter = _data.GetComponent<MeshFilter>();
			var material = _data.GetComponent<MeshRenderer>().material;
			
			if(meshFilter != null && !m_CombineDict.ContainsKey(_data.GetInstanceID()))
			{
				m_CombineDict.Add(_data.GetInstanceID(),meshFilter);
				_data.transform.SetParent(transform);

				return true;
			}
		}
		
		return false;
	}

	public void CleanUp()
	{
		transform.parent = null;
		
		m_MeshFilter.mesh.Clear();
		
		if(m_Material != null)
		{
			Destroy(m_Material);

			m_Material = null;
		}

		if(m_MeshFilter.mesh != null)
		{
			Destroy(m_MeshFilter.mesh);

			m_MeshFilter.mesh = null;
		}

		m_MeshFilter = null;
	}

	public void RemoveCombineData(GameObject _data)
	{
		if(m_Material == null)
		{
			return;
		}

		m_CombineDict.RemoveSafe(_data.GetInstanceID());

		_data.transform.SetParent(null);

		var renderer = _data.gameObject.GetComponent<MeshRenderer>();

		if(renderer != null)
		{
			renderer.enabled = true;
		}

		var batching = _data.GetComponent<BatchingObject>();

		if(batching != null)
		{
			batching.BatchingUndo();
		}
	}
	
	public void Combine()
	{
		if(m_CombineDict.Count < 1 || m_MeshFilter == null)
		{
			return;
		}
		
		// if (m_isTerrainLayer)
		// {
		// 	gameObject.layer = LayerMask.NameToLayer("Background_Terrain");
		// }

		if(m_MeshFilter.mesh == null)
		{
			m_MeshFilter.mesh = new Mesh();
			m_MeshFilter.mesh.name = string.Format("Combine_{0}",m_Material.name);
		}
		else
		{
			m_MeshFilter.mesh.Clear();
		}
		
		m_CombineInstanceList.Clear();
		var count = 0L;

		foreach(var filter in m_CombineDict.Values)
		{
			if(filter == null || filter.sharedMesh == null)
			{
				continue;
			}

			//  filter.sharedMesh 이게 null 로 의심되어 체크하게끔 수정함
			var combineInstance = new CombineInstance();
			combineInstance.mesh = filter.sharedMesh;
			combineInstance.transform = transform.worldToLocalMatrix*filter.transform.localToWorldMatrix;

			for(var i=0;i<combineInstance.mesh.subMeshCount;i++)
			{
				count += combineInstance.mesh.GetIndexCount(i);
			}
				
			var meshRenderer = filter.GetComponent<MeshRenderer>();

			// 만약 이후에도 에러가 난다면 filter.sharedMesh 이게 null 이다
			if(meshRenderer != null)
			{
				combineInstance.lightmapScaleOffset = meshRenderer.lightmapScaleOffset;
				combineInstance.realtimeLightmapScaleOffset = meshRenderer.realtimeLightmapScaleOffset;

				m_CombineInstanceList.Add(combineInstance);
			}
		}

		if(count >= MAX_INDEX_COUNT)
		{
			m_MeshFilter.mesh.indexFormat = IndexFormat.UInt32;

			Log.Data.W("배칭된 메쉬의 인덱스 갯수가 많습니다. 배경파트 확인해주세요 : {0} : index count : {1}",m_MeshFilter.gameObject.name,count);
		}
		
		m_MeshFilter.mesh.CombineMeshes(m_CombineInstanceList.ToArray(),true,true,true);

		foreach(var filter in m_CombineDict.Values)
		{
			if(filter == null)
			{
				continue;
			}

			var meshRenderer = filter.gameObject.GetComponent<MeshRenderer>();
			
			if(meshRenderer != null)
			{
				meshRenderer.enabled = false;
			}
		}
	}
}