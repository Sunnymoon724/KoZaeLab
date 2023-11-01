using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZSample
{
	public class MeshCombineCon : BaseComponent
	{
		[SerializeField]
		private MeshFilter m_Sample = null;

		[SerializeField]
		private CombinedMeshFilter m_CombinedMeshFilter = null;

		[SerializeField]
		private List<MeshFilter> m_MeshFilterList = new();

		[Button("오브젝트 추가하기")]
		private void OnAddMesh()
		{
			var data = Tools.CopyObject(m_Sample);

			m_CombinedMeshFilter.AddMesh(data);
			m_MeshFilterList.AddNotOverlap(data);

			m_CombinedMeshFilter.Combine();
		}

		[Button("오브젝트 랜덤 제거하기")]
		private void OnRemoveMesh()
		{
			if(!m_MeshFilterList.RemoveRndValue(out var data))
			{
				return;
			}

			m_CombinedMeshFilter.RemoveMesh(data);

			Tools.DestroyObject(data.gameObject);

			m_CombinedMeshFilter.Combine();
		}

		[Button("메쉬 비우기")]
		private void OnCleanMesh()
		{
			m_CombinedMeshFilter.CleanUp();

			foreach(var data in m_MeshFilterList)
			{
				Tools.DestroyObject(data.gameObject);
			}

			m_MeshFilterList.Clear();
		}
	}
}