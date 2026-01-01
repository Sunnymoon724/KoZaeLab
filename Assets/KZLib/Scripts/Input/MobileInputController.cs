using UnityEngine.InputSystem;

namespace KZLib
{
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