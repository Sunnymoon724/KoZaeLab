#if UNITY_EDITOR
using Sirenix.OdinInspector;

/// <summary>
/// 엑셀 파일 사용 세팅
/// </summary>
public abstract partial class ExcelSettings<TObject> : OuterBaseSettings<TObject> where TObject : SerializedScriptableObject
{
	protected abstract bool IsShowAddButton { get; }

	[BoxGroup("엑셀 설정",ShowLabel = false)]
	[HorizontalGroup("엑셀 설정/추가",Order = 0),Button("시트 추가",ButtonSizes.Large),PropertyTooltip("유니티 내부 폴더에 있으면 안됩니다."),ShowIf(nameof(IsShowAddButton))]
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

	protected abstract void OnSetSheetData(string _filePath);
}
#endif