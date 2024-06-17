using System;
using System.Collections.Generic;
using System.Linq;
using KZLib.KZAttribute;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	public interface IMetaDataTable
	{
		IEnumerable<IMetaData> DataGroup { get; }
#if UNITY_EDITOR
		void OnRefresh();
#endif
		IMetaDataTable Initialize();
	}

	public interface IMetaData
	{
		int MetaId { get; }
		string Version { get; }
		bool IsExist { get; }
	}

	public abstract class MetaDataTable : SerializedScriptableObject,IMetaDataTable
	{
		public abstract IEnumerable<IMetaData> DataGroup { get; }

#if UNITY_EDITOR
		protected abstract string TableName { get; }

		[BoxGroup("옵션",ShowLabel = false,Order = 0)]
		[HorizontalGroup("옵션/갱신",Order = 0),Button("갱신 하기",ButtonSizes.Large)]
		public virtual void OnRefresh()
		{
			IsValidate();

			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			if(MetaDataMgr.HasInstance)
			{
				MetaDataMgr.In.Reload();
			}

			OnShowAllList();
		}

		[HorizontalGroup("옵션/기타",Order = 1),ShowInInspector,LabelText("현재 경로"),KZDocumentPath(false),LabelWidth(75)]
		protected string CurrentPath => MetaSettings.In.GetSheetPath(TableName);

		[HorizontalGroup("옵션/기타",Order = 1)]
		[HorizontalGroup("옵션/기타/추출",Order = 0),Button("ToJson",ButtonSizes.Medium),PropertyTooltip("현재 경로로 익스포트 합니다.")]
		public void OnExportToJson()
		{
			var filePath = CommonUtility.PathCombine(CommonUtility.GetParentAbsolutePath(CurrentPath),string.Format("{0}.json",TableName));
			var text = JsonConvert.SerializeObject(DataGroup);

			CommonUtility.WriteDataToFile(filePath,text);
		}

		protected virtual void IsValidate()
		{
			var iterator = DataGroup.GetEnumerator();
			var index = 0;
			var indexList = new List<int>();

			while(iterator.MoveNext())
			{
				var data = iterator.Current;

				if(data == null)
				{
					CommonUtility.DisplayError(string.Format("{0} 번쨰 데이터가 null 입니다.",index));
				}

				if(data.MetaId == 0)
				{
					CommonUtility.DisplayError(string.Format("{0} 번쨰 메타아이디가 0 입니다.",index));
				}

				if(indexList.Contains(data.MetaId))
				{
					CommonUtility.DisplayError(string.Format("{0} 번쨰 메타아이디[{1}]가 중복 입니다!",index,data.MetaId));
				}

				if(data.MetaId != -1)
				{
					indexList.Add(data.MetaId);
				}

				index++;
			}
		}

		[BoxGroup("테이블",ShowLabel = false,Order = 1)]
		[VerticalGroup("테이블/버튼",Order = 0)]
		[HorizontalGroup("테이블/버튼/0",Order = 0),Button("전체 리스트",ButtonSizes.Large)]
		protected virtual void OnShowAllList()
		{
			SetMetaDataList(DataGroup);
		}

		[BoxGroup("테이블",ShowLabel = false,Order = 1)]
		[HorizontalGroup("테이블/버튼/0",Order = 0),Button("사용 가능한 리스트",ButtonSizes.Large)]
		protected virtual void OnShowExistList()
		{
			SetMetaDataList(DataGroup.Where(x=>x.IsExist));
		}

		[BoxGroup("테이블",ShowLabel = false,Order = 1)]
		[HorizontalGroup("테이블/버튼/0",Order = 0),Button("오류가 있는 리스트",ButtonSizes.Large)]
		protected virtual void OnShowNotExistList()
		{
			SetMetaDataList(DataGroup.Where(x=>!x.IsExist));
		}

		protected abstract void SetMetaDataList(IEnumerable<IMetaData> _dataGroup);

		private void OnEnable()
		{
			OnShowAllList();
		}
#endif
		public virtual IMetaDataTable Initialize() { return this; }
	}
}