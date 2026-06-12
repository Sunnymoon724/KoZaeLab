using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib.Utilities;

#if KZLIB_PLAY_FAB

using System;
using Newtonsoft.Json;

#endif

namespace KZLib.Networks
{
	public partial class NetworkManager : Singleton<NetworkManager>
	{
		#region Login
		public async UniTask<bool> RequestAutoLoginAsync(string sessionTicket)
		{
#if KZLIB_PLAY_FAB
			async UniTask<bool> _RequestAsync()
			{
				var playFabAccount = m_accountCredential as PlayFabAccountCredential;
				var loginOptionType = playFabAccount.LoginOptionType;

				return _GetPlayFabResult("AutoLoginAsync",await PlayFabManager.In.AutoLoginAsync(sessionTicket,loginOptionType));
			}

			return await _RequestToServerAsync(_RequestAsync,true,CommonNoticeTag.None);
#else
			await UniTask.Yield();

			return false;
#endif
		}

		public async UniTask<bool> LoginWithGuestAsync()
		{
#if KZLIB_PLAY_FAB
			async UniTask<bool> _RequestAsync()
			{
				var packet = await PlayFabManager.In.LoginWithGuestAsync();

				return _TryApplyLoginResult("LoginWithGuestAsync",packet);
			}

			return await _RequestToServerAsync(_RequestAsync,true,CommonNoticeTag.None);
#else
			await UniTask.Yield();

			return false;
#endif
		}

		public async UniTask<bool> LoginWithGoogleAsync(string sessionTicket)
		{
#if KZLIB_PLAY_FAB
			async UniTask<bool> _RequestAsync()
			{
				var packet = await PlayFabManager.In.LoginWithGoogleAsync(sessionTicket);

				return _TryApplyLoginResult("LoginWithGoogleAsync",packet);
			}

			return await _RequestToServerAsync(_RequestAsync,true,CommonNoticeTag.None);
#else
			await UniTask.Yield();

			return false;
#endif
		}

		public async UniTask<bool> LoginWithAppleAsync(string sessionTicket)
		{
#if KZLIB_PLAY_FAB
			async UniTask<bool> _RequestAsync()
			{
				var packet = await PlayFabManager.In.LoginWithAppleAsync(sessionTicket);

				return _TryApplyLoginResult("LoginWithAppleAsync",packet);
			}

			return await _RequestToServerAsync(_RequestAsync,true,CommonNoticeTag.None);
#else
			await UniTask.Yield();

			return false;
#endif
		}
		#endregion Login

		#region LogOut
		public async UniTask<bool> LogOutAsync()
		{
#if KZLIB_PLAY_FAB
			async UniTask<bool> _RequestAsync()
			{
				var packet = await PlayFabManager.In.LogOutAsync();

				if(!_GetPlayFabResult("LogOutAsync",packet))
				{
					return false;
				}

				_ClearAccountProfile();

				return true;
			}

			return await _RequestToServerAsync(_RequestAsync,true,CommonNoticeTag.None);
#else
			await UniTask.Yield();

			return false;
#endif
		}
		#endregion LogOut

#if KZLIB_PLAY_FAB
		private bool _TryApplyLoginResult(string functionName,PlayFabPacketInfo packet)
		{
			if(!_GetPlayFabResult(functionName,packet))
			{
				return false;
			}

			_TrySetAccountCredentialFromMessage(_GetRespondMessage(packet.RespondPacket));

			return true;
		}

		private void _TrySetAccountCredentialFromMessage(string message)
		{
			if(message.IsEmpty())
			{
				return;
			}

			try
			{
				var messageDict = JsonConvert.DeserializeObject<Dictionary<string,string>>(message);

				_SetAccountCredential(messageDict);
			}
			catch(Exception exception)
			{
				LogChannel.Network.W($"Failed to parse login credential message : {exception.Message}");
			}
		}
#endif

		private void _SetAccountCredential(Dictionary<string,string> messageDict)
		{
			if(messageDict == null)
			{
				return;
			}

			if(messageDict.TryGetValue("ProfileId",out var profileId))
			{
				m_accountCredential.SetAccountId(profileId);
			}

#if KZLIB_PLAY_FAB

			var playFabAccount = m_accountCredential as PlayFabAccountCredential;

			if(messageDict.TryGetValue("LoginOptionType",out var loginOptionType) && Enum.TryParse(loginOptionType,out PlayFabLoginOptionType optionType))
			{
				playFabAccount.SetLoginType(optionType);
			}
#endif
		}

		private void _ClearAccountProfile()
		{
			m_accountCredential.Clear();
		}
	}
}