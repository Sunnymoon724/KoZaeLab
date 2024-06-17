#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace KZLib.KZWindow
{
	public partial class MetaDataWindow : OdinEditorWindow
	{
		[TabGroup("메타 데이터","스리프트 파트",Order = 1)]
		[HorizontalGroup("메타 데이터/스리프트 파트/추가",Order = 1),Button("시트 추가2",ButtonSizes.Large),PropertyTooltip("유니티 내부 폴더에 있으면 안됩니다.")]
		private void OnAddExcelSheet2()
		{
			// var filePath = CommonUtility.GetExcelFilePath();

			// if(filePath.IsEmpty())
			// {
			// 	return;
			// }

			// if(CommonUtility.IsIncludeAssetsHeader(filePath))
			// {
			// 	CommonUtility.DisplayError(string.Format("파일 {0}는 유니티 내부 폴더에 있으면 안됩니다.",filePath));

			// 	return;
			// }

			// OnSetSheetData(filePath[(CommonUtility.GetProjectParentPath().Length+1)..]);
		}
	}
}
#endif