#if KZLIB_PLAY_FAB
using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using KZLib.KZData;
using KZLib.KZUtility;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace KZLib
{
	public partial class PlayFabMgr : Singleton<PlayFabMgr>
	{
		public async UniTask<LoginResult> AutoLoginAsync()
		{
			var optionCfg = ConfigMgr.In.Access<ConfigData.OptionConfig>();

			var logInType = optionCfg.PlayFabLogInType;

			if(logInType == PlayFabLogInOptionType.None)
			{
				return null;
			}

			switch(logInType)
			{
				case PlayFabLogInOptionType.GuestLogIn:
				{
					return await LoginWithGuestAsync();
				}
				case PlayFabLogInOptionType.GoogleLogIn:
				{
					return await LoginWithGoogleAsync(string.Empty);
				}
				case PlayFabLogInOptionType.AppleLogIn:
				{
					return await LoginWithAppleAsync(string.Empty);
				}
				default:
				{
					throw new NotSupportedException($"{logInType} is not supported.");
				}
			}
		}

		//? Log in -> Guest / Google / Apple
		#region Login
		public async UniTask<LoginResult> LoginWithGuestAsync()
		{
			var requestMethodName = "LoginWithGuestAsync";

			var stopwatch = Stopwatch.StartNew();

			var source = new UniTaskCompletionSource<LoginResult>();

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

				_SaveLoginType(PlayFabLogInOptionType.GuestLogIn);

				_WriteResult(requestMethodName,request,result,stopwatch.ElapsedMilliseconds);

				source.TrySetResult(result);
			},(playFabError) =>
			{
				stopwatch.Stop();

				_WriteError(requestMethodName,request,playFabError,stopwatch.ElapsedMilliseconds);

				source.TrySetException(new Exception(playFabError.ErrorMessage));
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

				_SaveLoginType(PlayFabLogInOptionType.GuestLogIn);

				_WriteResult(requestMethodName,request,result,stopwatch.ElapsedMilliseconds);

				source.TrySetResult(result);
			},(playFabError) =>
			{
				stopwatch.Stop();

				_WriteError(requestMethodName,request,playFabError,stopwatch.ElapsedMilliseconds);

				source.TrySetException(new Exception(playFabError.ErrorMessage));
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

				_SaveLoginType(PlayFabLogInOptionType.GuestLogIn);

				_WriteResult(requestMethodName,request,result,stopwatch.ElapsedMilliseconds);

				source.TrySetResult(result);
			},(playFabError) =>
			{
				stopwatch.Stop();

				_WriteError(requestMethodName,request,playFabError,stopwatch.ElapsedMilliseconds);

				source.TrySetException(new Exception(playFabError.ErrorMessage));
			});
#endif
			var result = await source.Task;

			m_myPlayFabId = result.PlayFabId;

			return result;
		}

		public async UniTask<LoginResult> LoginWithGoogleAsync(string serverAuthCode)
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			var requestMethodName = "LoginWithGoogleAsync";

			var request = new LoginWithGoogleAccountRequest()
			{
				ServerAuthCode = serverAuthCode,
				CreateAccount = true,
			};

			var source = new UniTaskCompletionSource<LoginResult>();

			var stopwatch = Stopwatch.StartNew();

			PlayFabClientAPI.LoginWithGoogleAccount(request,(result) =>
			{
				stopwatch.Stop();

				_SaveLoginType(PlayFabLogInOptionType.GoogleLogIn);

				_WriteResult(requestMethodName,request,result,stopwatch.ElapsedMilliseconds);

				source.TrySetResult(result);
			},(playFabError) =>
			{
				stopwatch.Stop();

				_WriteError(requestMethodName,request,playFabError,stopwatch.ElapsedMilliseconds);

				source.TrySetException(new Exception(playFabError.ErrorMessage));
			});

			var result = await source.Task;

			m_myPlayFabId = result.PlayFabId;

			return result;
#else
			LogTag.System.W($"aos Only");

			await UniTask.Yield();

			return null;
#endif
		}

		public async UniTask<LoginResult> LoginWithAppleAsync(string identityToken)
		{
#if UNITY_IOS && !UNITY_EDITOR
			var requestMethodName = "LoginWithAppleAsync";

			var request = new LoginWithAppleRequest()
			{
				IdentityToken = identityToken,
				CreateAccount = true,
			};

			var source = new UniTaskCompletionSource<LoginResult>();

			var stopwatch = Stopwatch.StartNew();

			PlayFabClientAPI.LoginWithApple(request,(result) =>
			{
				stopwatch.Stop();

				_SaveLoginType(PlayFabLogInOptionType.AppleLogIn);

				_WriteResult(requestMethodName,request,result,stopwatch.ElapsedMilliseconds);

				source.TrySetResult(result);
			},(playFabError) =>
			{
				stopwatch.Stop();

				_WriteError(requestMethodName,request,playFabError,stopwatch.ElapsedMilliseconds);

				source.TrySetException(new Exception(playFabError.ErrorMessage));
			});

			var result = await source.Task;

			m_myPlayFabId = result.PlayFabId;

			return result;
#else
			LogTag.System.W($"ios Only");

			await UniTask.Yield();

			return null;
#endif
		}

		private void _SaveLoginType(PlayFabLogInOptionType playFabLogInType)
		{
			var optionCfg = ConfigMgr.In.Access<ConfigData.OptionConfig>();

			optionCfg.SetPlayFabLogInType(playFabLogInType);
		}
		#endregion Login

		#region LogOut
		public async UniTask LogOutAsync()
		{
			var requestMethodName = "LogOutAsync";

			var stopwatch = Stopwatch.StartNew();

			PlayFabClientAPI.ForgetAllCredentials();

			_WriteResult(requestMethodName,null,null,stopwatch.ElapsedMilliseconds);

			await UniTask.Yield();
		}
		#endregion LogOut

		#region Get
		public async UniTask<GetPlayerProfileResult> GetPlayerProfileAsync(string playFabId)
		{
			var requestMethodName = "GetPlayerProfileAsync";

			var request = new GetPlayerProfileRequest()
			{
				PlayFabId = playFabId,
				ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true }
			};

			var source = new UniTaskCompletionSource<GetPlayerProfileResult>();

			var stopwatch = Stopwatch.StartNew();

			PlayFabClientAPI.GetPlayerProfile(request,(result) =>
			{
				stopwatch.Stop();

				_WriteResult(requestMethodName,request,result,stopwatch.ElapsedMilliseconds);

				source.TrySetResult(result);
			},(playFabError) =>
			{
				stopwatch.Stop();

				_WriteError(requestMethodName,request,playFabError,stopwatch.ElapsedMilliseconds);

				source.TrySetException(new Exception(playFabError.ErrorMessage));
			});

			return await source.Task;
		}
		#endregion Get
	}
}
#endif