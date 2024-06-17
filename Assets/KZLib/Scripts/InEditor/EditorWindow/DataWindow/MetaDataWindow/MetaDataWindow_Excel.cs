#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace KZLib.KZWindow
{
	public partial class MetaDataWindow : OdinEditorWindow
	{
		private enum DataType { String, Int, Long, Float, Enum, Bool, Vector3, }

		private const string META_ID = "MetaId";
		private const string VERSION = "Version";

		private string m_ErrorLog = null;

		[TabGroup("메타 데이터","엑셀 파트",Order = 0)]
		[HorizontalGroup("메타 데이터/엑셀 파트/추가",Order = 0),Button("시트 추가",ButtonSizes.Large),PropertyTooltip("유니티 내부 폴더에 있으면 안됩니다.")]
		private void OnAddExcelSheet()
		{
			var filePath = CommonUtility.GetExcelFilePath();

			if(filePath.IsEmpty())
			{
				return;
			}

			if(CommonUtility.IsIncludeAssetsHeader(filePath))
			{
				CommonUtility.DisplayError(string.Format("파일 {0}는 유니티 내부 폴더에 있으면 안됩니다.",filePath));

				return;
			}

			var localPath = filePath[(CommonUtility.GetProjectParentPath().Length+1)..];

			m_ExcelSheetList.Add(new ExcelSheetData(localPath,OnRemoveSheet));
		}

		[PropertySpace(10)]
		// [HorizontalGroup("메타 데이터/엑셀 파트/리스트",Order = 1),SerializeField,LabelText(" "),ListDrawerSettings(ShowFoldout = false,HideAddButton = true,DraggableItems = false,NumberOfItemsPerPage = 1),ShowIf(nameof(IsExistSheet))]
		// private List<ExcelSheetData> m_ExcelSheetList = new();

		[HorizontalGroup("메타 데이터/엑셀 파트/리스트",Order = 1),SerializeField,LabelText(" "),TableList(AlwaysExpanded = true,NumberOfItemsPerPage = 1,ShowPaging = true,IsReadOnly = true),ShowIf(nameof(IsExistSheet))]
		private List<ExcelSheetData> m_ExcelSheetList = new();

		private bool IsExistSheet => m_ExcelSheetList.Count > 0;

		[HorizontalGroup("메타 데이터/엑셀 파트/변환",Order = 2),Button("변환 하기",ButtonSizes.Large),ShowIf(nameof(IsExistSheet)),EnableIf(nameof(IsConvertAble)),PropertyTooltip("$m_ErrorLog")]
		private void OnConvertToThrift()
		{
			// var classType = Type.GetType(string.Format("MetaData.{0}Table",SheetName));
			// var scriptData = new ScriptData(SheetName,SheetName,m_HeaderList);

			// if(classType == null)
			// {
			// 	//? script 만들기
			// 	var scriptPath = CommonUtility.PathCombine(CommonUtility.GetFullPath(GameSettings.In.MetaScriptPath),string.Format("{0}Table.cs",SheetName));

			// 	scriptData.WriteScript(scriptPath);
			// }

			// //? enum 만들기
			// if(scriptData.IsInEnum(out var orderList))
			// {
			// 	var enumDict = File.GetEnumDict(SheetName,orderList);
			// 	var removeList = new List<string>(enumDict.Count);

			// 	foreach(var key in enumDict.Keys)
			// 	{
			// 		var enumType = Type.GetType(string.Format("MetaData.{0}",key));

			// 		if(enumType != null)
			// 		{
			// 			removeList.Add(key);
			// 		}
			// 	}

			// 	foreach(var remove in removeList)
			// 	{
			// 		Log.Data.W("{0}는 이미 존재하여서 생략합니다.",remove);

			// 		enumDict.Remove(remove);
			// 	}

			// 	CreateEnum(enumDict);
			// }

			// AssetDatabase.Refresh();
		}

		private bool IsConvertAble()
		{
			m_ErrorLog = string.Empty;

			var builder = new StringBuilder();

			for(var i=0;i<m_ExcelSheetList.Count;i++)
			{
				var sheet = m_ExcelSheetList[i];

				if(!sheet.IsConvertAble(out var error))
				{
					builder.AppendFormat("{0} 번째 시트 데이터가 오류 입니다. [{1}]\n",i+1,error);
				}
			}

			m_ErrorLog = builder.ToString();

			return builder.Length <= 0;
		}

		private void OnRemoveSheet(string _filePath)
		{
			var sheet = m_ExcelSheetList.Find(x => x.AbsoluteFilePath.IsEqual(_filePath));

			if(sheet!= null)
			{
				m_ExcelSheetList.Remove(sheet);
			}
		}
	}
}
#endif