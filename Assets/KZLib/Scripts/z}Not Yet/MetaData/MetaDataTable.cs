#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using KZLib.KZAttribute;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace KZLib
{
	public class MetaDataTable
	{
		[BoxGroup("테이블",ShowLabel = false,Order = 1)]
		[VerticalGroup("테이블/1",Order = 1),LabelText("데이터 리스트"),TableList(IsReadOnly = true,AlwaysExpanded = true,ShowPaging = true,NumberOfItemsPerPage = 10),ShowInInspector]
		private readonly List<IMetaData> m_MetaDataList = new();

		protected string m_TableName = null;

		public MetaDataTable(string _tableName,IEnumerable<IMetaData> _dataGroup)
		{
			m_TableName = _tableName;

			m_MetaDataList.AddRange(_dataGroup);
		}

		[BoxGroup("옵션",ShowLabel = false,Order = 0)]
		[HorizontalGroup("옵션/갱신",Order = 0),Button("갱신 하기",ButtonSizes.Large)]
		public virtual void OnRefresh()
		{
			// TODO 엑셀 갱신 하기

			// IsValidate();

			// EditorUtility.SetDirty(this);
			// AssetDatabase.SaveAssets();
			// AssetDatabase.Refresh();

			// if(MetaDataMgr.HasInstance)
			// {
			// 	MetaDataMgr.In.Reload();
			// }

			OnShowAllList();
		}

		[HorizontalGroup("옵션/기타",Order = 1),ShowInInspector,LabelText("현재 경로"),KZDocumentPath(false),LabelWidth(75)]
		protected string CurrentPath => MetaSettings.In.GetSheetPath(m_TableName);

		[HorizontalGroup("옵션/기타",Order = 1)]
		[HorizontalGroup("옵션/기타/추출",Order = 0),Button("ToJson",ButtonSizes.Medium),PropertyTooltip("현재 경로로 익스포트 합니다.")]
		public void OnExportToJson()
		{
			var filePath = FileUtility.PathCombine(FileUtility.GetParentAbsolutePath(CurrentPath,true),string.Format("{0}.json",m_TableName));
			var text = JsonConvert.SerializeObject(m_MetaDataList);

			FileUtility.WriteTextToFile(filePath,text);
		}

		protected virtual void IsValidate()
		{
			var indexSet = new HashSet<int>();

			for(var i=0;i<m_MetaDataList.Count;i++)
			{
				var data = m_MetaDataList[i];

				if(data == null)
				{
					UnityUtility.DisplayError(string.Format("{0} 번쨰 데이터가 null 입니다.",i));
				}

				if(data.MetaId == 0)
				{
					UnityUtility.DisplayError(string.Format("{0} 번쨰 메타아이디가 0 입니다.",i));
				}

				if(indexSet.Contains(data.MetaId))
				{
					UnityUtility.DisplayError(string.Format("{0} 번쨰 메타아이디[{1}]가 중복 입니다!",i,data.MetaId));
				}

				if(data.MetaId != -1)
				{
					indexSet.Add(data.MetaId);
				}
			}
		}

		[BoxGroup("테이블",ShowLabel = false,Order = 1)]
		[VerticalGroup("테이블/버튼",Order = 0)]
		[HorizontalGroup("테이블/버튼/0",Order = 0),Button("전체 리스트",ButtonSizes.Large)]
		protected virtual void OnShowAllList()
		{
			SetMetaDataList(m_MetaDataList);
		}

		[BoxGroup("테이블",ShowLabel = false,Order = 1)]
		[HorizontalGroup("테이블/버튼/0",Order = 0),Button("사용 가능한 리스트",ButtonSizes.Large)]
		protected virtual void OnShowExistList()
		{
			SetMetaDataList(m_MetaDataList.Where(x=>x.IsExist));
		}

		[BoxGroup("테이블",ShowLabel = false,Order = 1)]
		[HorizontalGroup("테이블/버튼/0",Order = 0),Button("오류가 있는 리스트",ButtonSizes.Large)]
		protected virtual void OnShowNotExistList()
		{
			SetMetaDataList(m_MetaDataList.Where(x=>!x.IsExist));
		}

		protected virtual void SetMetaDataList(IEnumerable<IMetaData> _dataGroup)
		{
			m_MetaDataList.Clear();

			foreach(var data in _dataGroup)
			{
				// m_MetaDataList.Add(data as PartsData);
			}
		}
	}
}
#endif