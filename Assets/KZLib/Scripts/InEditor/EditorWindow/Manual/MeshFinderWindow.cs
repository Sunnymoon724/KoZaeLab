#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace KZLib.Windows
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

				m_prefabHashSet.Clear();

				bool _Execute(string assetPath,int index,int totalCount)
				{
					var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

					if(!asset)
					{
						return true;
					}

					var meshFilterArray = asset.GetComponentsInChildren<MeshFilter>(true);

					for(var i=0;i<meshFilterArray.Length;i++)
					{
						if(meshFilterArray[i].sharedMesh == m_selectionMesh)
						{
							m_prefabHashSet.Add(new Prefab(asset,assetPath,value,m_replaceMesh,IsValidReplace));
						}
					}

					return true;
				}

				KZAssetKit.ExecuteMatchedAssetPath("t:prefab",null,_Execute);

				if(m_prefabHashSet.IsNullOrEmpty())
				{
					KZEditorKit.DisplayInfo("There is no prefab with a mesh.");
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

				var prefabList = new List<Prefab>(m_prefabHashSet);

				m_prefabHashSet.Clear();

				var isValidReplace = SelectionMesh && SelectionMesh != ReplaceMesh;

				for(var i=0;i<prefabList.Count;i++)
				{
					m_prefabHashSet.Add(new Prefab(prefabList[i],value,isValidReplace));
				}
			}
		}

		[HorizontalGroup("Find Mesh/2",Order = 2),SerializeField,ShowIf(nameof(HasPrefab)),ListDrawerSettings(ShowFoldout = false,DraggableItems = false,HideAddButton = true,HideRemoveButton = true)]
		private HashSet<Prefab> m_prefabHashSet = new();
		private bool HasPrefab => SelectionMesh && !m_prefabHashSet.IsNullOrEmpty();

		private bool IsValidReplace => ReplaceMesh && SelectionMesh != ReplaceMesh && HasPrefab;

		[VerticalGroup("Find Mesh/3",Order = 3),ShowIf(nameof(HasPrefab)),EnableIf(nameof(IsValidReplace))]
		protected void OnMeshToolBar()
		{
			foreach(var prefab in m_prefabHashSet)
			{
				prefab.OnChangeMesh();
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

				var meshFilterArray = m_prefab.GetComponentsInChildren<MeshFilter>(true);
				
				for(var i=0;i<meshFilterArray.Length;i++)
				{
					var meshFilter = meshFilterArray[i];

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
						LogChannel.Editor.I($"{m_prefab.name} is changed. and saved to {m_filePath}.");
					}
					else
					{
						LogChannel.Editor.W($"{m_prefab.name} try to change and save. but not saved to {m_filePath}.");
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