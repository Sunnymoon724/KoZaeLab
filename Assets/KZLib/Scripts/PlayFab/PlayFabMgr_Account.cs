#if KZLIB_PLAY_FAB
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace KZLib
{
	public partial class PlayFabMgr : Singleton<PlayFabMgr>
	{
		public async UniTask<PlayFabPacket> AutoLoginAsync(string sessionTicket,PlayFabLoginOptionType loginOptionType)
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
		public async UniTask<PlayFabPacket> LoginWithGuestAsync()
		{
			var stopwatch = Stopwatch.StartNew();
			var source = new UniTaskCompletionSource<PlayFabPacket>();

#if UNITY_ANDROID && !UNITY_EDITOR
			var request = new LoginWithAndroidDeviceIDRequest()
			{
				AndroidDevice = SystemInfo.deviceModel,
				OS = SystemInfo.operatingSystem,
				AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
				CreateAccount = true,
			};

			PlayFabClientAPI.LoginWithAndroidDeviceID(request,(result) =>
			{
				stopwatch.Stop();

				var message = JsonConvert.SerializeObject(new Dictionary<string,string>()
				{
					{ "LoginOptionType", $"{PlayFabLoginOptionType.GuestLogin}" },
					{ "ProfileId",result.PlayFabId },
				});

				source.TrySetResult(new PlayFabPacket(request,new NetworkPacket(0,message,false),stopwatch.ElapsedMilliseconds));
			},(playFabError) =>
			{
				stopwatch.Stop();
				source.TrySetResult(_CreateErrorPacket(playFabError,stopwatch.ElapsedMilliseconds));
			});
#elif UNITY_IOS && !UNITY_EDITOR
			var request = new LoginWithIOSDeviceIDRequest()
			{
				DeviceModel = SystemInfo.deviceModel,
				OS = SystemInfo.operatingSystem,
				DeviceId = SystemInfo.deviceUniqueIdentifier,
				CreateAccount = true,
			};

			PlayFabClientAPI.LoginWithIOSDeviceID(request,(result) =>
			{
				stopwatch.Stop();

				var message = JsonConvert.SerializeObject(new Dictionary<string,string>()
				{
					{ "LoginOptionType", $"{PlayFabLoginOptionType.GuestLogin}" },
					{ "ProfileId",result.PlayFabId },
				});

				source.TrySetResult(new PlayFabPacket(request,new NetworkPacket(0,message,false),stopwatch.ElapsedMilliseconds));
			},(playFabError) =>
			{
				stopwatch.Stop();
				source.TrySetResult(_CreateErrorPacket(playFabError,stopwatch.ElapsedMilliseconds));
			});
#else
			var request = new LoginWithCustomIDRequest()
			{
				CustomId = SystemInfo.deviceUniqueIdentifier,
				CreateAccount = true
			};

			PlayFabClientAPI.LoginWithCustomID(request,(result) =>
			{
				stopwatch.Stop();

				var message = JsonConvert.SerializeObject(new Dictionary<string,string>()
				{
					{ "LoginOptionType", $"{PlayFabLoginOptionType.GuestLogin}" },
					{ "ProfileId",result.PlayFabId },
				});

				source.TrySetResult(new PlayFabPacket(request,new NetworkPacket(0,message,false),stopwatch.ElapsedMilliseconds));
			},(playFabError) =>
			{
				stopwatch.Stop();
				source.TrySetResult(_CreateErrorPacket(playFabError,stopwatch.ElapsedMilliseconds));
			});
#endif
			return await source.Task;
		}

		public async UniTask<PlayFabPacket> LoginWithGoogleAsync(string sessionTicket)
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			var request = new LoginWithGoogleAccountRequest()
			{
				ServerAuthCode = sessionTicket,
				CreateAccount = true,
			};

			var source = new UniTaskCompletionSource<PlayFabPacket>();
			var stopwatch = Stopwatch.StartNew();

			PlayFabClientAPI.LoginWithGoogleAccount(request,(result) =>
			{
				stopwatch.Stop();

				var message = JsonConvert.SerializeObject(new Dictionary<string,string>()
				{
					{ "LoginOptionType", $"{PlayFabLoginOptionType.GoogleLogin}" },
					{ "ProfileId",result.PlayFabId },
				});

				source.TrySetResult(new PlayFabPacket(request,new NetworkPacket(0,message,false),stopwatch.ElapsedMilliseconds));
			},(playFabError) =>
			{
				stopwatch.Stop();
				source.TrySetResult(_CreateErrorPacket(playFabError,stopwatch.ElapsedMilliseconds));
			});

			return await source.Task;
#else
			LogSvc.System.W("aos Only");

			await UniTask.Yield();

			return null;
#endif
		}

		public async UniTask<PlayFabPacket> LoginWithAppleAsync(string sessionTicket)
		{
#if UNITY_IOS && !UNITY_EDITOR
			var request = new LoginWithAppleRequest()
			{
				IdentityToken = sessionTicket,
				CreateAccount = true,
			};

			var source = new UniTaskCompletionSource<PlayFabPacket>();

			var stopwatch = Stopwatch.StartNew();

			PlayFabClientAPI.LoginWithApple(request,(result) =>
			{
				stopwatch.Stop();

				var message = JsonConvert.SerializeObject(new Dictionary<string,string>()
				{
					{ "LoginOptionType", $"{PlayFabLoginOptionType.AppleLogin}" },
					{ "ProfileId",result.PlayFabId },
				});

				source.TrySetResult(new PlayFabPacket(request,new NetworkPacket(0,message,false),stopwatch.ElapsedMilliseconds));
			},(playFabError) =>
			{
				stopwatch.Stop();
				source.TrySetResult(_CreateErrorPacket(playFabError,stopwatch.ElapsedMilliseconds));
			});

			return await source.Task;
#else
			LogSvc.System.W("ios Only");

			await UniTask.Yield();

			return null;
#endif
		}
		#endregion Login

		#region LogOut
		public async UniTask<PlayFabPacket> LogOutAsync()
		{
			var stopwatch = Stopwatch.StartNew();

			PlayFabClientAPI.ForgetAllCredentials();

			await UniTask.Yield();

			return new PlayFabPacket(null,new NetworkPacket(0,string.Empty,false),stopwatch.ElapsedMilliseconds);
		}
		#endregion LogOut
	}
}
#endif