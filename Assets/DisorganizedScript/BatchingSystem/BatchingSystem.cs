using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BUILD_STATE_TYPE { Wait, Building, }

public enum BATCHING_TARGET_STATE_TYPE { None, Add, Remove, }

public class BatchingData
{
	protected Transform m_OriginalParent = null;
	protected GameObject m_BatchingTargetObject = null;
	protected BatchingObject m_BatchingObject = null;
	protected Mesh m_OriginalMesh = null;

	public uint BatchingIndex => m_BatchingObject.BatchingIndex;

	public GameObject BatchingTargetObject => m_BatchingTargetObject;

	public BatchingData(Transform _parent,BatchingObject _target,uint _id)
	{
		m_BatchingTargetObject = _target.gameObject;
		m_OriginalParent = m_BatchingTargetObject.transform.parent;
		m_BatchingObject = _target;
		m_BatchingObject.BatchingIndex = _id;
	}

	public void Destroy()
	{
		if(m_BatchingTargetObject != null)
		{
			m_BatchingTargetObject.transform.SetParent(m_OriginalParent,true);
		}
	}
}

public class BatchingController
{
	protected Material m_Material = null;
	protected GameObject m_BatchingParent = null;
	protected Dictionary<int,CombineGroup> m_CombineGroupList = new Dictionary<int,CombineGroup>();
	protected SortedDictionary<uint,BatchingData> m_BatchingDataDict = new SortedDictionary<uint,BatchingData>();

	public BatchingController(GameObject _target,Transform _root)
	{
		m_BatchingParent = new GameObject();

		var renderer = _target.GetComponent<MeshRenderer>();

		AddData(_target);

		if(renderer != null)
		{
			m_Material = renderer.material;
			m_BatchingParent.name = string.Format("{0}_Batching_Parent",m_Material.name);
		}

		m_BatchingParent.transform.SetParent(_root,true);
	}

	public void AddData(GameObject _target)
	{
		var renderer = _target.GetComponent<MeshRenderer>();
		var batching = _target.GetComponent<BatchingObject>();

		if(renderer == null || batching == null)
		{
			return;
		}

		var data = new BatchingData(AddTransform(renderer,_target,renderer.lightmapIndex),batching,batching.BatchingIndex);

		m_BatchingDataDict.Add(data.BatchingIndex,data);
	}

	private Transform AddTransform(MeshRenderer _renderer,GameObject _target,int _lightMapIndex)
	{
		if(m_CombineGroupList.ContainsKey(_lightMapIndex))
		{
			m_CombineGroupList[_renderer.lightmapIndex].AddObject(_target);

			return null;
		}

		var combineGroup = new CombineGroup(_target,m_BatchingParent.transform);
		var data = combineGroup.AddObject(_target);

		m_CombineGroupList.Add(_lightMapIndex,combineGroup);

		return data;
	}

	public void Remove(GameObject _target)
	{
		var batchingIndex = _target.GetComponent<BatchingObject>();

		if(batchingIndex != null && m_BatchingDataDict.ContainsKey(batchingIndex.BatchingIndex))
		{

			m_BatchingDataDict[batchingIndex.BatchingIndex].Destroy();
			m_BatchingDataDict.Remove( batchingIndex.BatchingIndex );

			foreach(var cur in m_CombineGroupList.Values)
			{
				cur.RemoveObject(_target);
			}
			// 갯수에 따른 처리를 여기서 했었음..상태변경
			//if ( m_BatchingDataDict.Count > 1 )
		}
	}

	public int GroupCount => m_CombineGroupList.Count;

	public bool IsBatched(GameObject _target)
	{
		return false;
	}

	public void Destroy()
	{
		foreach(var data in m_BatchingDataDict.Values)
		{
			if(data == null)
			{
				continue;
			}

			data.Destroy();
		}

		m_BatchingDataDict.Clear();

		foreach(var data in m_CombineGroupList.Values)
		{
			if(data == null)
			{
				continue;
			}

			data.Destroy();
		}

		m_CombineGroupList.Clear();

		if(m_BatchingParent != null)
		{
			m_BatchingParent.transform.parent = null;

			Tools.DestroyObject(m_BatchingParent);
		}
	}

	public IEnumerator CoBuild()
	{
		if(m_BatchingDataDict.Count >= 1 && m_CombineGroupList.Count > 0)
		{
			foreach(var combine in m_CombineGroupList.Values)
			{
				yield return combine.CoCombine();
			}
		}
	}
}

public class CombineGroup
{
	protected int m_LightMapIndex = 0;
	private bool m_NeedRebuild = true;
	protected Material m_Material = null;
	protected Transform m_Parent = null;
	protected Dictionary<int,MeshCombine> m_CombineMeshDict = new Dictionary<int,MeshCombine>();

	public CombineGroup(GameObject _object,Transform _parent)
	{
		var renderer = _object.GetComponent<MeshRenderer>();
		var batching = _object.GetComponent<BatchingObject>();

		m_Material = batching.BatchingMaterial;
		m_LightMapIndex = renderer.lightmapIndex;
		m_Parent = _parent;
	}

	public IEnumerator CoCombine()
	{
		if(m_NeedRebuild)
		{
			var iterator = m_CombineMeshDict.GetEnumerator();

			while(iterator.MoveNext())
			{
				iterator.Current.Value.Combine();

				yield return null;
			}

			m_NeedRebuild = false;
		}
	}
	
	public Transform AddObject(GameObject _object)
	{
		var batching = _object.GetComponent<BatchingObject>();

		if(batching == null)
		{
			return null;
		}
		
		if(!m_CombineMeshDict.ContainsKey(batching.BatchingVolumeIndex))
		{
			NewCombineGroup(_object);
		}

		m_CombineMeshDict[batching.BatchingVolumeIndex].AddCombineData(_object);

		m_NeedRebuild = true;

		return m_CombineMeshDict[batching.BatchingVolumeIndex].transform;
	}

	public void RemoveObject(GameObject _object)
	{
		var batching = _object.GetComponent<BatchingObject>();

		if(batching == null || !m_CombineMeshDict.ContainsKey(batching.BatchingVolumeIndex))
		{
			return;
		}

		m_NeedRebuild = true;
		m_CombineMeshDict[batching.BatchingVolumeIndex].RemoveCombineData(_object);

		if(!m_CombineMeshDict[batching.BatchingVolumeIndex].IsEmpty)
		{
			return;
		}
		
		var combine = m_CombineMeshDict[batching.BatchingVolumeIndex];
		m_CombineMeshDict.Remove(batching.BatchingVolumeIndex );

		combine.CleanUp();
		Tools.DestroyObject(combine.gameObject);
	}

	public void Destroy()
	{
		var iterator = m_CombineMeshDict.GetEnumerator();

		while(iterator.MoveNext())
		{
			if(iterator.Current.Value == null)
			{
				continue;
			}

			iterator.Current.Value.CleanUp();

			Tools.DestroyObject(iterator.Current.Value.gameObject);
		}

		m_CombineMeshDict.Clear();
	}

	protected MeshCombine NewCombineGroup(GameObject _object)
	{
		var BatchingObject = _object.GetComponent<BatchingObject>();
		var combineGroupObject = new GameObject();

		combineGroupObject.name = string.Format("lightmap_index_{0}_volume_index_{1}_{2}",m_LightMapIndex,BatchingObject.BatchingVolumeIndex,m_Material.name);
		var combine = combineGroupObject.GetOrAddComponent<MeshCombine>();
		var renderer = _object.GetComponent<MeshRenderer>();

		combine.Initialize(m_LightMapIndex);
		combine.SetMaterial(renderer.material);
		combine.AddCombineData(_object);

		m_CombineMeshDict.Add(BatchingObject.BatchingVolumeIndex,combine);

		combineGroupObject.transform.SetParent(m_Parent);

		return combine;
	}
}

public class BatchingSystem
{
	public class WaitBatching
	{
		public GameObject BatchingObject { get; }
		public BATCHING_TARGET_STATE_TYPE BatchingState { get; set; }

		public WaitBatching(GameObject _object,BATCHING_TARGET_STATE_TYPE _state)
		{
			BatchingObject = _object;
			BatchingState = _state;
		}
	}

	protected BUILD_STATE_TYPE m_BuildState = BUILD_STATE_TYPE.Wait;
	protected GameObject m_BatchingRoot = null;
	protected Dictionary<string,BatchingController> m_BatchingControllerDict = new Dictionary<string,BatchingController>();
	protected Dictionary<GameObject,WaitBatching> m_WaitBatchingDict = new Dictionary<GameObject,WaitBatching>();
	protected Dictionary<int,GameObject> m_ObjectDict = new Dictionary<int, GameObject>();
	private List<BatchingObject> m_AddedObjectList = new List<BatchingObject>();
	private List<BatchingObject> m_RemovedObjectList = new List<BatchingObject>();
	private string INSTANCE = " (Instance)";

	public void Add(GameObject _target)
	{
		if(m_WaitBatchingDict.TryGetValue(_target,out var data))
		{
			data.BatchingState = BATCHING_TARGET_STATE_TYPE.Add;
		}
		else
		{
			m_WaitBatchingDict.Add(_target,new WaitBatching(_target,BATCHING_TARGET_STATE_TYPE.Add));
		}
	}

	public void Remove(GameObject _target)
	{
		if(m_WaitBatchingDict.TryGetValue(_target,out var data))
		{
			data.BatchingState = BATCHING_TARGET_STATE_TYPE.Remove;
		}
		else
		{
			m_WaitBatchingDict.Add(_target,new WaitBatching(_target,BATCHING_TARGET_STATE_TYPE.Remove));
		}
	}

	public bool IsBatched(GameObject _target)
	{
		return m_ObjectDict.ContainsKey(_target.GetInstanceID());
	}

	private void StartBatch()
	{
		m_BuildState = BUILD_STATE_TYPE.Building;

		if(m_BatchingRoot == null)
		{
			m_BatchingRoot = new GameObject("Batching_Root");
		}

		foreach(var data in m_WaitBatchingDict.Values)
		{
			// 인스턴스 아이디기준으로 중복되는 오브젝트는 거름

			if(data.BatchingState == BATCHING_TARGET_STATE_TYPE.Add && !IsBatched(data.BatchingObject))
			{
				ProcessAdd(data.BatchingObject);
			}
			else if(data.BatchingState == BATCHING_TARGET_STATE_TYPE.Remove && IsBatched(data.BatchingObject))
			{
				ProcessRemove(data.BatchingObject);
			}
		}

		m_WaitBatchingDict.Clear();
	}

	void ProcessAdd(GameObject _target)
	{
		var batching = _target.GetComponent<BatchingObject>();

		if(batching == null)
		{
			return;
		}
		
		m_AddedObjectList.Add(batching);
		m_ObjectDict.Add(_target.GetInstanceID(),_target);

		// 배칭용 머티리얼로 바꾸고, 이름에서 인스턴스뗀후 키로 사용
		batching.BatchingPrepare();

		var renderer = _target.GetComponent<MeshRenderer>();
		var filter = _target.GetComponent<MeshFilter>();

		if(renderer == null || renderer.material == null || filter == null)
		{
			return;
		}
		
		var materialName = renderer.material.name;
		int removeBeg = materialName.IndexOf(INSTANCE);

		if(removeBeg != -1)
		{
			materialName = materialName.Remove(removeBeg);
		}

		if(m_BatchingControllerDict.ContainsKey(materialName))
		{
			m_BatchingControllerDict[materialName].AddData(_target);
		}
		else
		{
			m_BatchingControllerDict.Add(materialName,new BatchingController(_target,m_BatchingRoot.transform));
		}
	}

	private void ProcessRemove(GameObject _target)
	{
		var renderer = _target.GetComponent<MeshRenderer>();

		if(renderer == null && renderer.material == null)
		{
			return;
		}

		m_ObjectDict.Remove(_target.GetInstanceID());
		m_RemovedObjectList.Add(_target.GetComponent<BatchingObject>());

		var materialName = renderer.material.name;		
		var removeTag = materialName.IndexOf(INSTANCE);

		if(removeTag != -1)
		{
			materialName = materialName.Remove(removeTag);
		}

		if(!m_BatchingControllerDict.ContainsKey(materialName))
		{
			return;
		}

		m_BatchingControllerDict[materialName].Remove(_target);

		if(m_BatchingControllerDict[materialName].GroupCount == 0)
		{
			m_BatchingControllerDict[materialName].Destroy();
			m_BatchingControllerDict.Remove(materialName);
		}
	}

	public void Destroy()
	{
		foreach(var controller in m_BatchingControllerDict.Values)
		{
			if(controller == null)
			{
				continue;
			}

			controller.Destroy();
		}

		m_BatchingControllerDict.Clear();
		m_WaitBatchingDict.Clear();
		m_ObjectDict.Clear();
		m_AddedObjectList.Clear();
		m_RemovedObjectList.Clear();

		Tools.DestroyObject(m_BatchingRoot);
	}

	public IEnumerator BuildCoroutine()
	{
		while(true)
		{
			if(m_BuildState != BUILD_STATE_TYPE.Wait)
			{
				continue;
			}

			StartBatch();

			foreach(var pair in m_BatchingControllerDict)
			{
				yield return pair.Value.CoBuild();
			}
			
			yield return null;

			m_BuildState = BUILD_STATE_TYPE.Wait;

			foreach(var data in m_AddedObjectList)
			{
				data.OnTransitionEnd(BatchingObject.BATCHING_STATE_TYPE.BatchingIn);
			}

			foreach(var data in m_RemovedObjectList)
			{
				data.OnTransitionEnd(BatchingObject.BATCHING_STATE_TYPE.BatchingOut);
			}
				
			m_AddedObjectList.Clear();
			m_RemovedObjectList.Clear();

			yield return null;
		}
	}
}