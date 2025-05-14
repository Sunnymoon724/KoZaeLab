#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace KZLib.KZWindow
{
	public class MeshFinderWindow : OdinEditorWindow
	{
		private Mesh m_selectionMesh = null;
		private Mesh m_replaceMesh = null;

		[TitleGroup("Find Mesh",BoldTitle = false,Order = 0)]
		[HorizontalGroup("Find Mesh/0",Order = 0),ShowInInspector]
		private Mesh SelectionMesh
		{
			get => m_selectionMesh;
			set
			{
				if(m_selectionMesh == value)
				{
					return;
				}

				m_selectionMesh = value;
				ReplaceMesh = null;

				if(m_selectionMesh == null)
				{
					return;
				}

				m_prefabList.Clear();

				foreach(var assetPath in CommonUtility.FindAssetPathGroup("t:prefab"))
				{
					var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

					foreach(var meshFilter in asset.GetComponentsInChildren<MeshFilter>(true))
					{
						if(meshFilter.sharedMesh == m_selectionMesh)
						{
							m_prefabList.AddNotOverlap(new Prefab(asset,assetPath,value,m_replaceMesh,IsValidReplace));
						}
					}
				}

				if(m_prefabList.IsNullOrEmpty())
				{
					CommonUtility.DisplayInfo("There is no prefab with a mesh.");
				}
			}
		}

		[HorizontalGroup("Find Mesh/1",Order = 1),ShowInInspector]
		private Mesh ReplaceMesh
		{
			get => m_replaceMesh;
			set
			{
				if(m_replaceMesh == value)
				{
					return;
				}

				m_replaceMesh = value;

				if(m_replaceMesh == null)
				{
					return;
				}

				var prefabList = new List<Prefab>(m_prefabList);

				m_prefabList.Clear();

				var isValidReplace = SelectionMesh && SelectionMesh != ReplaceMesh;

				foreach(var prefab in prefabList)
				{
					m_prefabList.AddNotOverlap(new Prefab(prefab,value,isValidReplace));
				}
			}
		}

		[HorizontalGroup("Find Mesh/2",Order = 2),SerializeField,ShowIf(nameof(HasPrefab)),ListDrawerSettings(ShowFoldout = false,DraggableItems = false,HideAddButton = true,HideRemoveButton = true)]
		private List<Prefab> m_prefabList = new();
		private bool HasPrefab => SelectionMesh && !m_prefabList.IsNullOrEmpty();

		private bool IsValidReplace => ReplaceMesh && SelectionMesh != ReplaceMesh && HasPrefab;

		[VerticalGroup("Find Mesh/3",Order = 3),ShowIf(nameof(HasPrefab)),EnableIf(nameof(IsValidReplace))]
		protected void OnMeshToolBar()
		{
			foreach(var meshData in m_prefabList)
			{
				meshData.OnChangeMesh();
			}
		}

		private readonly struct Prefab
		{
			[HorizontalGroup("0",width: 0.95f),ShowInInspector,HideLabel,ReadOnly]
			private readonly GameObject m_prefab;
			private readonly Mesh m_selectionMesh;
			private readonly Mesh m_replaceMesh;
			private readonly bool m_isExistReplace;
			private readonly string m_filePath;

			[HorizontalGroup("0",width: 0.05f),Button(SdfIconType.ArrowRepeat,""),EnableIf(nameof(m_isExistReplace))]
			public void OnChangeMesh()
			{
				var changed = false;

				foreach(var meshFilter in m_prefab.GetComponentsInChildren<MeshFilter>(true))
				{
					if(meshFilter.sharedMesh != m_selectionMesh)
					{
						continue;
					}

					meshFilter.sharedMesh = m_replaceMesh;
					changed = true;
				}

				if(changed)
				{
					PrefabUtility.SaveAsPrefabAsset(m_prefab,m_filePath,out var result);

					if(result)
					{
						KZLogType.Editor.I($"{m_prefab.name} is changed. and saved to {m_filePath}.");
					}
					else
					{
						KZLogType.Editor.W($"{m_prefab.name} try to change and save. but not saved to {m_filePath}.");
					}
				}
			}

			public Prefab(GameObject prefab,string filePath,Mesh selectionMesh,Mesh replaceMesh,bool isExistReplace)
			{
				m_prefab = prefab;
				m_filePath = filePath;
				m_selectionMesh = selectionMesh;
				m_replaceMesh = replaceMesh;
				m_isExistReplace = isExistReplace;
			}

			public Prefab(Prefab prefab,Mesh replaceMesh,bool isExistReplace)
			{
				m_prefab = prefab.m_prefab;
				m_filePath = prefab.m_filePath;
				m_selectionMesh = prefab.m_selectionMesh;
				m_replaceMesh = replaceMesh;
				m_isExistReplace = isExistReplace;
			}
		}
	}
}
#endif