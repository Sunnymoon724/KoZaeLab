using UnityEngine;
using TMPro;

public static class TextMeshProExtension
{
	public static void SetSafeTextMeshPro(this TMP_Text textMesh,string text,Color? color = null)
	{
		if(!_IsValid(textMesh,text))
		{
			return;
		}

		_SetSafeTextMeshPro(textMesh,text,color);
	}

	// public static void SetLocalizeText(this TMP_Text textMesh,string text)
	// {
	// 	if(!_IsValid(textMesh,text))
	// 	{
	// 		return;
	// 	}

	// 	var localizeTextUI = textMesh.GetComponent<LocalizeTextUI>();

	// 	if(localizeTextUI)
	// 	{
	// 		localizeTextUI.SetLocalizeKey(text);
	// 	}
	// 	else
	// 	{
	// 		_SetSafeTextMeshPro(textMesh,text,null);
	// 	}
	// }

	private static void _SetSafeTextMeshPro(TMP_Text textMesh,string text,Color? color)
	{
		textMesh.text = text;

		if(!color.HasValue)
		{
			return;
		}

		textMesh.color = color.Value;
	}

	private static bool _IsValid(TMP_Text textMesh,string text)
	{
		if(!textMesh)
		{
			KZLogType.System.E("TextMeshPro is null");

			textMesh.gameObject.EnsureActive(false);

			return false;
		}

		if(text.IsEmpty())
		{
			textMesh.gameObject.EnsureActive(false);

			return false;
		}

		textMesh.gameObject.EnsureActive(true);

		return true;
	}
}