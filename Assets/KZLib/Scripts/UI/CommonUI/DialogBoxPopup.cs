using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogBoxPopup : BasePopup
{
	public record Param(string Title,string Message,params DialogEntryInfo[] EntryInfoArray);

	[SerializeField]
	private TMP_Text m_titleText = null;

	[SerializeField]
	private TMP_Text m_messageText = null;

	[SerializeField]
	private ReuseGridLayoutGroup m_gridLayout = null;

	public override void Open(object param)
	{
		base.Open(param);

		if(param is not Param dialogParam)
		{
			return;
		}

		if(m_titleText)
		{
			m_titleText.SetSafeTextMeshPro(dialogParam.Title);
		}

		m_messageText.SetSafeTextMeshPro(dialogParam.Message);

		var entryInfoList = new List<IEntryInfo>();
		var entryInfoArray = dialogParam.EntryInfoArray;

		for(var i=0;i<entryInfoArray.Length;i++)
		{
			var entryInfo = entryInfoArray[i];

			if(entryInfo.OnClicked == null)
			{
				LogSvc.UI.E($"{entryInfo.Name} is null");

				return;
			}

			entryInfoList.Add(entryInfo);
		}

		if(entryInfoList.IsNullOrEmpty())
		{
			return;
		}

		m_gridLayout.SetEntryInfoList(entryInfoList);
	}
}