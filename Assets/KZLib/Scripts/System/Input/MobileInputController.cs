using UnityEngine.InputSystem;

namespace KZLib.Inputs
{
	/// <summary>
	/// Handles the <c>AndroidBackButton</c> action (Escape / gamepad B / <c>*/{Back}</c> in <c>MobileInputAction</c>).
	/// Forwards to <see cref="UIManager.PressBackButton"/> on the top 2D window.
	/// </summary>
	public class MobileInputController : InputController
	{
		private const string c_androidBackButton = "AndroidBackButton";

		protected override void SubscribeInputAction()
		{
			var androidBackButtonAction = _TryGetInputAction(c_androidBackButton);

			if(androidBackButtonAction != null)
			{
				androidBackButtonAction.started += OnPressedBackButton;
			}
		}

		protected override void UnsubscribeInputAction()
		{
			var androidBackButtonAction = _TryGetInputAction(c_androidBackButton);

			if(androidBackButtonAction != null)
			{
				androidBackButtonAction.started -= OnPressedBackButton;
			}
		}

		private void OnPressedBackButton(InputAction.CallbackContext _)
		{
			if(UIManager.HasInstance)
			{
				UIManager.In.PressBackButton();
			}
		}
	}
}
