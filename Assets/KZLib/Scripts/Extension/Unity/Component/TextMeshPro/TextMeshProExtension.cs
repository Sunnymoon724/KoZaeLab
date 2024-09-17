using UnityEngine;
using TMPro;

public static class TextMeshProExtension
{
	public static void SetSafeTextMeshPro(this TMP_Text _textMesh,string _text,Color? _color = null)
	{
		if(!IsValidTextMesh(_textMesh,_text))
		{
			return;
		}

		_textMesh.gameObject.SetActiveSelf(true);

		SetSafeTextMeshProInner(_textMesh,_text,_color);
	}

	public static void SetLocalizeText(this TMP_Text _textMesh,string _text)
	{
		if(!IsValidTextMesh(_textMesh,_text))
		{
			return;
		}

		_textMesh.gameObject.SetActiveSelf(true);

		var localize = _textMesh.GetComponent<LocalizeTextUI>();

		if(localize)
		{
			localize.SetLocalizeKey(_text);
		}
		else
		{
			SetSafeTextMeshProInner(_textMesh,_text,null);
		}
	}

	private static bool IsValidTextMesh(TMP_Text _textMesh,string _text)
	{
		if(!_textMesh)
		{
			return false;
		}

		if(_text.IsEmpty())
		{
			_textMesh.gameObject.SetActiveSelf(false);

			return false;
		}

		return true;
	}

	private static void SetSafeTextMeshProInner(TMP_Text _textMesh,string _text,Color? _color)
	{
		_textMesh.text = _text;

		if(!_color.HasValue)
		{
			return;
		}

		_textMesh.color = _color.Value;
	}
}