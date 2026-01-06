using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using KZLib.KZData;
using KZLib.KZUtility;
using MessagePipe;
using Newtonsoft.Json;

namespace KZLib.KZNetwork
{
	public partial class NetworkManager : Singleton<NetworkManager>
	{
		private const string c_titleScene = "TitleScene";

		private bool m_disposed = false;

		private bool m_isRequesting = false;
		public bool IsRequesting => m_isRequesting;

		private AccountCredential m_accountCredential = null;
#if KZLIB_PLAY_FAB
		private string m_publicKey = null;
#endif
		public bool HasAccountToken => m_accountCredential.HasAccountToken;

		public string AccountId => m_accountCredential.AccountId;

		protected override void Initialize()
		{
			base.Initialize();

#if KZLIB_PLAY_FAB
			m_accountCredential = new PlayFabAccountCredential();
#else
			
#endif
		}

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				
			}

			m_disposed = true;

			base.Release(disposing);
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
				CommonUtility.LockInput();
			}

			var result = await onRequestFunc.Invoke();

			if(touchBlock)
			{
				CommonUtility.UnLockInput();
			}

			if(result && noticeTag != CommonNoticeTag.None)
			{
				GlobalMessagePipe.GetPublisher<CommonNoticeTag,EmptyNoticeInfo>().Publish(noticeTag,EmptyNoticeInfo.Empty);
			}

			m_isRequesting = false;

			return result;
		}

#if KZLIB_PLAY_FAB
		private bool _GetPlayFabResult(string functionName,PlayFabPacketInfo playFabPacket)
		{
			var respondPacket = playFabPacket.RespondPacket;
			var code = respondPacket.Code;
			var isSuccess = code == 0;
			var message = respondPacket.IsEncrypted ? CryptoUtility.RSA.DecryptFromString(respondPacket.Message,m_publicKey) : respondPacket.Message;
			var requestText = JsonConvert.SerializeObject(playFabPacket.RequestPacket,Formatting.Indented);

			_WriteDump(functionName,requestText,isSuccess,message,playFabPacket.Duration);

			if(!isSuccess)
			{
				LogSvc.Network.E($"Respond Error : {code}");

				var errorPrt = ProtoManager.In.GetProto<NetworkErrorProto>(code);

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
			SceneStateManager.In.ChangeSceneWithLoading(c_titleScene,CommonUINameTag.CommonTransitionPanel);
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
					var type = Type.GetType($"{typeText}, Assembly-CSharp") ?? throw new NullReferenceException($"{typeText} is not found");
					var newAffix = JsonConvert.DeserializeObject(affixText,type) as IAffix ?? throw new NullReferenceException($"{affixText} is not {type} type");
					
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
			catch(Exception _ex)
			{
				LogSvc.Network.E($"Convert is fail : {_ex.Message}");
			}
		}
		
		private void _ShowErrorPopup(string _text,Action _closeCallBack)
		{
			// TODO 에러 팝업창 만들어서 띄우기
		}
		
		private void _ShowErrorToast(string _text)
		{
			// TODO 에러 토스트 만들어서 띄우기
		}

#if UNITY_EDITOR
		private void _WriteDump(string requestMethodName,string requestText,bool responseResult,string responseText,long responseTime)
		{
			var dumpBuilder = new StringBuilder();
			var currentTime = GameTimeManager.In.GetCurrentTime(true);

			dumpBuilder.Append("================= [Network Dump] =================\n\n");
			dumpBuilder.Append($"[TIME]\n{currentTime:yyyy\\/MM\\/dd\\ HH:mm:ss}\n\n");
			dumpBuilder.Append($"[REQUEST]\n");
			dumpBuilder.Append($"[REQUEST]\n{requestMethodName}\n");

			if(!requestText.IsEmpty())
			{
				dumpBuilder.Append($"[REQUEST CONTENT]\n{requestText}\n");
			}

			dumpBuilder.Append("\n[RESPONSE]\n");
			dumpBuilder.Append($"[RESPONSE TIME]\n{responseTime}\n");
			
			if(responseResult)
			{
				dumpBuilder.Append($"[RESPONSE RESULT]\nSUCCESS\n");
				dumpBuilder.Append($"[RESPONSE CONTENT]\n{responseText}\n");
			}
			else
			{
				var errorDict = JsonConvert.DeserializeObject<Dictionary<string,string>>(responseText);

				dumpBuilder.Append($"[RESPONSE RESULT]\nFAILURE\n");
				dumpBuilder.Append($"[RESPONSE ERROR MESSAGE]\n{errorDict["Message"]}\n");

				if(errorDict.TryGetValue("Details",out var details))
				{
					dumpBuilder.Append($"[RESPONSE ERROR DETAILS]\n{details}\n");
				}
			}

			var filePath = Path.Combine(Global.PROJECT_PARENT_PATH,"NetworkDump",$"{requestMethodName}.log");

			FileUtility.WriteTextToFile(filePath,dumpBuilder.ToString());
		}
#endif
	}
}