#if KZLIB_PLAY_FAB
using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using KZLib.Utilities;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using UnityEngine;

#if !UNITY_EDITOR

using System.Collections.Generic;
using Newtonsoft.Json;

#endif

namespace KZLib
{
	public partial class PlayFabManager : Singleton<PlayFabManager>
	{
		public async UniTask<PlayFabPacketInfo> AutoLoginAsync(string sessionTicket,PlayFabLoginOptionType loginOptionType)
		{
			if(loginOptionType == PlayFabLoginOptionType.None)
			{
				return null;
			}

			switch(loginOptionType)
			{
				case PlayFabLoginOptionType.GuestLogin:
				{
					return await LoginWithGuestAsync();
				}
				case PlayFabLoginOptionType.GoogleLogin:
				{
					return await LoginWithGoogleAsync(sessionTicket); // 아직 미구현
				}
				case PlayFabLoginOptionType.AppleLogin:
				{
					return await LoginWithAppleAsync(sessionTicket); // 아직 미구현
				}
				default:
				{
					throw new NotSupportedException($"{loginOptionType} is not supported.");
				}
			}
		}

		//? Log in -> Guest / Google / Apple
		#region Login
		public async UniTask<PlayFabPacketInfo> LoginWithGuestAsync()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			static void _SendPacket(PlayFabRequestCommon commonRequest,Action<PlayFabResultCommon> onSendResult,Action<PlayFabError> onSendError)
			{
				var aosRequest = commonRequest as LoginWithAndroidDeviceIDRequest;

				PlayFabClientAPI.LoginWithAndroidDeviceID(aosRequest,onSendResult,onSendError);
			}

			var request = new LoginWithAndroidDeviceIDRequest()
			{
				AndroidDevice = SystemInfo.deviceModel,
				OS = SystemInfo.operatingSystem,
				AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
				CreateAccount = true,
			};

			return await _LoginAsync(request,PlayFabLoginOptionType.GuestLogin,_SendPacket);
#elif UNITY_IOS && !UNITY_EDITOR
			static void _SendPacket(PlayFabRequestCommon commonRequest,Action<PlayFabResultCommon> onSendResult,Action<PlayFabError> onSendError)
			{
				var iosRequest = commonRequest as LoginWithIOSDeviceIDRequest;

				PlayFabClientAPI.LoginWithIOSDeviceID(iosRequest,onSendResult,onSendError);
			}

			var request = new LoginWithIOSDeviceIDRequest()
			{
				DeviceModel = SystemInfo.deviceModel,
				OS = SystemInfo.operatingSystem,
				DeviceId = SystemInfo.deviceUniqueIdentifier,
				CreateAccount = true,
			};

			return await _LoginAsync(request,PlayFabLoginOptionType.GuestLogin,_SendPacket);
#else
			static void _SendPacket(PlayFabRequestCommon commonRequest,Action<PlayFabResultCommon> onSendResult,Action<PlayFabError> onSendError)
			{
				var customRequest = commonRequest as LoginWithCustomIDRequest;

				PlayFabClientAPI.LoginWithCustomID(customRequest,onSendResult,onSendError);
			}

			var request = new LoginWithCustomIDRequest()
			{
				CustomId = SystemInfo.deviceUniqueIdentifier,
				CreateAccount = true
			};

			return await _LoginAsync(request,PlayFabLoginOptionType.GuestLogin,_SendPacket);
#endif
		}

		public async UniTask<PlayFabPacketInfo> LoginWithGoogleAsync(string sessionTicket)
		{
			static void _SendPacket(PlayFabRequestCommon commonRequest,Action<PlayFabResultCommon> onSendResult,Action<PlayFabError> onSendError)
			{
				var googleRequest = commonRequest as LoginWithGoogleAccountRequest;

				PlayFabClientAPI.LoginWithGoogleAccount(googleRequest,onSendResult,onSendError);
			}

			var request = new LoginWithGoogleAccountRequest()
			{
				ServerAuthCode = sessionTicket,
				CreateAccount = true,
			};

			return await _LoginAsync(request,PlayFabLoginOptionType.GoogleLogin,_SendPacket);
		}

		public async UniTask<PlayFabPacketInfo> LoginWithAppleAsync(string sessionTicket)
		{
			static void _SendPacket(PlayFabRequestCommon commonRequest,Action<PlayFabResultCommon> onSendResult,Action<PlayFabError> onSendError)
			{
				var appleRequest = commonRequest as LoginWithAppleRequest;

				PlayFabClientAPI.LoginWithApple(appleRequest,onSendResult,onSendError);
			}

			var request = new LoginWithAppleRequest()
			{
				IdentityToken = sessionTicket,
				CreateAccount = true,
			};

			return await _LoginAsync(request,PlayFabLoginOptionType.AppleLogin,_SendPacket);
		}

		private async UniTask<PlayFabPacketInfo> _LoginAsync(PlayFabRequestCommon request,PlayFabLoginOptionType loginOptionType,Action<PlayFabRequestCommon,Action<PlayFabResultCommon>,Action<PlayFabError>> onSendPacket)
		{
#if !UNITY_EDITOR
			NetworkPacket _CreatePacket(PlayFabResultCommon commonResult)
			{
				var loginResult = commonResult as LoginResult;

				var message = JsonConvert.SerializeObject(new Dictionary<string,string>()
				{
					{ "LoginOptionType", $"{loginOptionType}" },
					{ "ProfileId",loginResult.PlayFabId },
				});

				return new NetworkPacket(0,message,false);
			}

			return await _ExecuteAsync(request,onSendPacket,_CreatePacket);
#else
			LogChannel.System.W("editor is not supported");

			await UniTask.Yield();

			return null;
#endif
		}
		#endregion Login

		#region LogOut
		public async UniTask<PlayFabPacketInfo> LogOutAsync()
		{
			var stopwatch = Stopwatch.StartNew();

			PlayFabClientAPI.ForgetAllCredentials();

			await UniTask.Yield();

			return new PlayFabPacketInfo(null,new NetworkPacketInfo(0,string.Empty,false),stopwatch.ElapsedMilliseconds);
		}
		#endregion LogOut
	}
}
#endif