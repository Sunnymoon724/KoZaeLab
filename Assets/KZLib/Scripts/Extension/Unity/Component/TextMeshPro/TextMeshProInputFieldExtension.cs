using TMPro;
using UnityEngine.Events;

public static class TextMeshProInputFieldExtension
{
	/// <summary>
	/// 이미 등록된 리스너가 있으면 제거하고 새로 등록
	/// </summary>
	public static void AddListener(this TMP_InputField _inputField,UnityAction<string> _onAction,bool _overlap = false)
	{
		if(_inputField == null || _onAction == null)
		{
			return;
		}

		if(!_overlap)
		{
			_inputField.onValueChanged.RemoveListener(_onAction);
		}

		_inputField.onValueChanged.AddListener(_onAction);
	}

	/// <summary>
	/// 그냥 하나만 등록
	/// </summary>
	public static void SetListener(this TMP_InputField _inputField,UnityAction<string> _onAction)
	{
		if(_onAction == null)
		{
			return;
		}

		_inputField.onValueChanged.RemoveAllListeners();
		_inputField.onValueChanged.AddListener(_onAction);
	}

	/// <summary>
	/// 리스너 제거
	/// </summary>
	public static void RemoveListener(this TMP_InputField _inputField,UnityAction<string> _onAction)
	{
		if(_inputField == null || _onAction == null)
		{
			return;
		}

		_inputField.onValueChanged.RemoveListener(_onAction);
	}

	/// <summary>
	/// 모든 리스너 제거
	/// </summary>
	public static void ClearListener(this TMP_InputField _inputField)
	{
		if(_inputField == null)
		{
			return;
		}

		_inputField.onValueChanged.RemoveAllListeners();
	}
}