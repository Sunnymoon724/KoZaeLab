using System;
using Cysharp.Threading.Tasks;
using KZLib.Data;
using KZLib.Utilities;
using MessagePipe;
using Newtonsoft.Json;

namespace KZLib.Networks
{
	// Incomplete: will be finalized when backend server integration and live testing are done.
	public partial class NetworkManager : Singleton<NetworkManager>
	{
		private const string c_titleScene = "TitleScene";

		private bool m_isRequesting = false;
		public bool IsRequesting => m_isRequesting;

		private AccountCredential m_accountCredential = null;
#if KZLIB_PLAY_FAB
		// Incomplete: RSA public key loading is not implemented yet.
		private string m_publicKey = null;
#endif
		public bool HasAccountToken => m_accountCredential.HasAccountToken;

		public string AccountId => m_accountCredential.AccountId;

		private NetworkManager() { }

		protected override void _Initialize()
		{
			base._Initialize();

#if KZLIB_PLAY_FAB
			m_accountCredential = new PlayFabAccountCredential();
#else
			m_accountCredential = new StubAccountCredential();
#endif
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				
			}

			base._Release(disposing);
		}

		public async UniTask<bool> RequestToServerAsync(string functionName,object parameter,bool touchBlock,CommonNoticeTag noticeTag)
		{
#if KZLIB_PLAY_FAB
			async UniTask<bool> _RequestAsync()
			{
				return _GetPlayFabResult(functionName,await PlayFabManager.In.ExecuteCloudScriptAsync(functionName,parameter));
			}

			return await _RequestToServerAsync(_RequestAsync,touchBlock,noticeTag);
#else
			await UniTask.Yield();

			return false;
#endif
		}

		private async UniTask<bool> _RequestToServerAsync(Func<UniTask<bool>> onRequestFunc,bool touchBlock,CommonNoticeTag noticeTag)
		{
			if(IsRequesting)
			{
				return false;
			}

			m_isRequesting = true;

			if(touchBlock)
			{
				KZInputKit.LockInput();
			}

			try
			{
				var result = await onRequestFunc.Invoke();

				if(result && noticeTag != CommonNoticeTag.None)
				{
					GlobalMessagePipe.GetPublisher<CommonNoticeTag,EmptyNoticeInfo>().Publish(noticeTag,EmptyNoticeInfo.Empty);
				}

				return result;
			}
			finally
			{
				if(touchBlock)
				{
					KZInputKit.UnLockInput();
				}

				m_isRequesting = false;
			}
		}

#if KZLIB_PLAY_FAB
		private string _GetRespondMessage(NetworkPacketInfo respondPacket)
		{
			return respondPacket.IsEncrypted ? KZCryptoKit.RSA.DecryptFromString(respondPacket.Message,m_publicKey) : respondPacket.Message;
		}

		private bool _GetPlayFabResult(string functionName,PlayFabPacketInfo playFabPacket)
		{
			var respondPacket = playFabPacket.RespondPacket;
			var code = respondPacket.Code;
			var isSuccess = code == 0;
			var message = _GetRespondMessage(respondPacket);
			var requestText = JsonConvert.SerializeObject(playFabPacket.RequestPacket,Formatting.Indented);

			KZDumpKit.WritePlayFabDump(functionName,requestText,isSuccess,message,playFabPacket.Duration);

			if(!isSuccess)
			{
				LogChannel.Network.E($"Respond Error : {code}");

				var errorPrt = ProtoManager.In.GetProto<INetworkErrorProto>(code);

				if(errorPrt == null)
				{
					string text = string.Format( "{0} : {1}", code, respondPacket.Message );

					_ShowErrorPopup(text,null);
				}
				else
				{
					switch( errorPrt.ResultMainType )
					{
						case NetworkErrorResultType.Popup:
							{
								void _Close()
								{
									if( errorPrt.ResultSubType == NetworkErrorResultType.Title )
									{
										_ChangeTitleScene();
									}
								}

								_ShowErrorPopup(errorPrt.Description,_Close);
								break;
							}
						case NetworkErrorResultType.Toast:
							{
								_ShowErrorToast(errorPrt.Description);
								break;
							}
						case NetworkErrorResultType.Title:
							{
								_ChangeTitleScene();
								break;
							}
					}
				}
			}
			else
			{
				_SetPacket(message);
			}

			return isSuccess;
		}

		private void _ChangeTitleScene()
		{
			SceneStateManager.In.ChangeScene(c_titleScene,new SceneChangeInfo(CommonUINameTag.CommonTransitionPanel));
		}
#endif

		private void _SetPacket(string message)
		{
			try
			{
				var respondArray = JsonConvert.DeserializeObject<NetworkRespondInfo[]>(message);

				for(var i=0;i<respondArray.Length;i++)
				{
					var respond = respondArray[i];
					var typeText = respond.Type;
					var affixText = respond.Content;
					// Incomplete: type resolution is hard-coded to Assembly-CSharp; needs multi-assembly lookup before production use.
					var type = Type.GetType($"{typeText}, Assembly-CSharp") ?? throw new NullReferenceException($"{typeText} is not found. TypeText must be assigned.");
					var newAffix = JsonConvert.DeserializeObject(affixText,type) as IAffix ?? throw new NullReferenceException($"{affixText} is not {type} type. AffixText must be assigned.");

					if(respond.IsUpdate)
					{
						AffixManager.In.Update(newAffix);
					}
					else
					{
						AffixManager.In.Set(newAffix);
					}
				}
			}
			catch(Exception exception)
			{
				LogChannel.Network.E($"Convert is fail : {exception.Message}. Exception must be assigned.");
			}
		}
		
		private void _ShowErrorPopup(string text,Action closeCallBack)
		{
			// Incomplete: error popup UI is not implemented yet.
		}

		private void _ShowErrorToast(string text)
		{
			// Incomplete: error toast UI is not implemented yet.
		}
	}
}