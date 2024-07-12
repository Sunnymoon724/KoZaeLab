#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace KZLib.KZWindow
{
	public class MeshFindWindow : OdinEditorWindow
	{
		private static Mesh m_MeshObject = null;

		private static Mesh m_ReplaceMeshObject = null;

		private static bool m_ReplaceMesh = false;

		[TitleGroup("메쉬 검색",BoldTitle = false,Order = 0)]
		[HorizontalGroup("메쉬 검색/0",Order = 0),LabelText("메쉬 이름"),ShowInInspector,InlineButton(nameof(OnToggleReplace),SdfIconType.ChevronDown,"")]
		private Mesh MeshObject
		{
			get => m_MeshObject;
			set
			{
				if(m_MeshObject == value)
				{
					return;
				}

				m_MeshObject = value;

				if(m_MeshObject == null)
				{
					return;
				}

				m_MeshDataList.Clear();

				foreach(var pair in CommonUtility.LoadAssetDataGroup<GameObject>("t:prefab"))
				{
					foreach(var filter in pair.Item2.GetComponentsInChildren<MeshFilter>(true))
					{
						if(filter.sharedMesh == m_MeshObject)
						{
							m_MeshDataList.AddNotOverlap(new MeshData(pair.Item2,pair.Item1));

							continue;
						}
					};
				}

				if(m_MeshDataList.IsNullOrEmpty())
				{
					CommonUtility.DisplayInfo("메쉬를 가진 프리펩이 없습니다.");
				}
			}
		}

		[HorizontalGroup("메쉬 검색/1",Order = 1),LabelText("변경 메쉬"),ShowInInspector,ShowIf(nameof(m_ReplaceMesh))]
		private Mesh ReplaceMeshObject
		{
			get => m_ReplaceMeshObject;
			set
			{
				if(m_ReplaceMeshObject == value)
				{
					return;
				}

				m_ReplaceMeshObject = value;
			}
		}

		private void OnToggleReplace()
		{
			m_ReplaceMesh = !m_ReplaceMesh;
		}

		private bool IsExistMesh => m_MeshObject != null;

		[VerticalGroup("2",Order = 2),LabelText("프리펩 리스트"),SerializeField,ShowIf(nameof(IsExistPrefab)),ListDrawerSettings(ShowFoldout = false,DraggableItems = false,HideAddButton = true,HideRemoveButton = true,OnTitleBarGUI = nameof(OnMeshToolBar))]
		private List<MeshData> m_MeshDataList = new();
		private bool IsExistPrefab => IsExistMesh && !m_MeshDataList.IsNullOrEmpty();

		private void OnMeshToolBar()
		{
			if(SirenixEditorGUI.ToolbarButton(SdfIconType.ArrowRepeat))
			{
				foreach(var meshData in m_MeshDataList)
				{
					meshData.OnChangeMesh();
				};
			}
		}

		private struct MeshData
		{
			[SerializeField,HideLabel,InlineButton(nameof(OnChangeMesh),SdfIconType.ArrowRepeat,"")]
			private GameObject m_Prefab;

			private readonly string m_Path;

			public readonly void OnChangeMesh()
			{
				if(!m_ReplaceMesh || m_MeshObject == m_ReplaceMeshObject)
				{
					return;
				}

				var changed = false;

				foreach(var filter in m_Prefab.GetComponentsInChildren<MeshFilter>(true))
				{
					if(filter.sharedMesh != m_MeshObject)
					{
						continue;
					}

					filter.sharedMesh = m_ReplaceMeshObject;

					changed = true;
				};

				if(changed)
				{
					PrefabUtility.SaveAsPrefabAsset(m_Prefab,m_Path,out var result);

					if(result)
					{
						LogTag.Editor.W(string.Format("{0}의 {1}가 변경 하여 저장 했습니다.",m_Path,changed));
					}
					else
					{
						LogTag.Editor.W(string.Format("{0}의 {1}을 변경하려고 했으나 저장에 실패했습니다.",m_Path,changed));
					}
				}
			}

			public MeshData(GameObject _prefab,string _path)
			{
				m_Prefab = _prefab;
				m_Path = _path;
			}
		}
	}
}
#endif