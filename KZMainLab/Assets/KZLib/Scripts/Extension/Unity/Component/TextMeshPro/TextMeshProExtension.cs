using UnityEngine;
using TMPro;

public static class TextMeshProExtension
{
	public static void SetSafeTextMeshPro(this TMP_Text _textMesh,string _text,Color? _color = null)
	{
		if(!_textMesh)
		{
			LogTag.System.E("TextMeshPro is null");

			return;
		}

		if(_text.IsEmpty())
		{
			_textMesh.gameObject.SetActiveSelf(false);

			return;
		}

		_textMesh.gameObject.SetActiveSelf(true);

		SetSafeTextMeshProInner(_textMesh,_text,_color);
	}

	public static void SetLocalizeText(this TMP_Text _textMesh,string _text)
	{
		if(!_textMesh)
		{
			LogTag.System.E("TextMeshPro is null");

			return;
		}

		if(_text.IsEmpty())
		{
			_textMesh.gameObject.SetActiveSelf(false);

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