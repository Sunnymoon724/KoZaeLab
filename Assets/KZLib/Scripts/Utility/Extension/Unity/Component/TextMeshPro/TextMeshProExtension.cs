using UnityEngine;
using TMPro;

/// <summary>
/// Extension methods for <see cref="TMP_Text"/> safe assignment and localization.
/// </summary>
public static class TextMeshProExtension
{
	/// <summary>
	/// Sets text and optional color; deactivates the GameObject when <paramref name="text"/> is empty.
	/// </summary>
	public static void SetSafeTextMeshPro(this TMP_Text textMesh,string text,Color? color = null)
	{
		if(!_IsValid(textMesh,text))
		{
			return;
		}

		_SetSafeTextMeshPro(textMesh,text,color);
	}

	/// <summary>
	/// Sets a localization key via <see cref="LocalizeTextUI"/> when present; otherwise assigns text directly.
	/// </summary>
	public static void SetLocalizeText(this TMP_Text textMesh,string text)
	{
		if(!_IsValid(textMesh,text))
		{
			return;
		}

		var localizeTextUI = textMesh.GetComponent<LocalizeTextUI>();

		if(localizeTextUI)
		{
			localizeTextUI.SetLocalizeKey(text);
		}
		else
		{
			_SetSafeTextMeshPro(textMesh,text,null);
		}
	}

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
			LogChannel.Kit.E("TextMeshPro is null");

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
