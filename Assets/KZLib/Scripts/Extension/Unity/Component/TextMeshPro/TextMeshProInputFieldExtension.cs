using TMPro;
using UnityEngine.Events;

public static class TextMeshProInputFieldExtension
{
	/// <summary>
	/// Add One Kind Listener
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
	/// Set One Listener
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
	/// Remove Listener
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
	/// Clear Listener
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