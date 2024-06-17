#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 엑셀 파일 사용 세팅
/// </summary>
public abstract partial class ExcelSettings<TObject> : OuterBaseSettings<TObject> where TObject : SerializedScriptableObject
{
	protected string m_ErrorLog = null;

	protected abstract bool IsShowAddButton { get; }
	protected abstract bool IsShowCreateButton { get; }
	protected abstract bool IsCreateAble { get; }

	[BoxGroup("엑셀 설정",ShowLabel = false)]
	[HorizontalGroup("엑셀 설정/추가",Order = 0),Button("시트 추가",ButtonSizes.Large),PropertyTooltip("유니티 내부 폴더에 있으면 안됩니다."),ShowIf(nameof(IsShowAddButton))]
#pragma warning disable IDE0051
    private void OnAddExcel()
    {
		var filePath = CommonUtility.GetExcelFilePath();

		if(filePath.IsEmpty())
		{
			return;
		}

		if(CommonUtility.IsIncludeAssetsHeader(filePath))
		{
			CommonUtility.DisplayError(string.Format("파일 {0}는 유니티 내부 폴더에 있으면 안됩니다.",filePath));
		}

		var localPath = filePath[(CommonUtility.GetProjectParentPath().Length+1)..];

		OnSetSheetData(localPath);
	}
#pragma warning restore IDE0051

	protected abstract void OnSetSheetData(string _filePath);

	[HorizontalGroup("엑셀 설정/생성",Order = 2),Button("생성 하기",ButtonSizes.Large),ShowIf(nameof(IsShowCreateButton)),EnableIf(nameof(IsCreateAble)),PropertyTooltip("$m_ErrorLog")]
	protected abstract void OnCreateButton();
}
#endif