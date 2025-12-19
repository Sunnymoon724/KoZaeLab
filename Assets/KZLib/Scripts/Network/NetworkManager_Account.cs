using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;

#if KZLIB_PLAY_FAB

using System;
using Newtonsoft.Json;

#endif

namespace KZLib.KZNetwork
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
				var messageDict = JsonConvert.DeserializeObject<Dictionary<string,string>>(packet.RespondPacket.Message);

				_SetAccountCredential(messageDict);

				return _GetPlayFabResult("LoginWithGuestAsync",packet);
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
				var messageDict = JsonConvert.DeserializeObject<Dictionary<string,string>>(packet.RespondPacket.Message);

				_SetAccountCredential(messageDict);

				return _GetPlayFabResult("LoginWithGoogleAsync",packet);
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
				var messageDict = JsonConvert.DeserializeObject<Dictionary<string,string>>(packet.RespondPacket.Message);

				_SetAccountCredential(messageDict);

				return _GetPlayFabResult("LoginWithAppleAsync",packet);
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

				_ClearAccountProfile();

				return _GetPlayFabResult("LogOutAsync",packet);
			}

			return await _RequestToServerAsync(_RequestAsync,true,CommonNoticeTag.None);
#else
			await UniTask.Yield();

			return false;
#endif
		}
		#endregion LogOut

		private void _SetAccountCredential(Dictionary<string,string> messageDict)
		{
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