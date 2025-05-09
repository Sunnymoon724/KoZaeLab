#if KZLIB_PLAY_FAB
using System;
using System.IO;
using System.Text;
using KZLib.KZUtility;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;

namespace KZLib
{
	public partial class PlayFabMgr : Singleton<PlayFabMgr>
	{
		private bool m_disposed = false;

		private string m_myPlayFabId = string.Empty;

		private GetUserDataResult m_userDataResult = null;

		protected override void Initialize()
		{
			
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

		private void _WriteResult(string methodName,object request,object result,long responseTime)
		{
#if UNITY_EDITOR
			var requestText = JsonConvert.SerializeObject(request,Formatting.Indented);

			_WriteDump(methodName,requestText,JsonConvert.SerializeObject(result,Formatting.Indented),null,responseTime);
#endif
		}

		private void _WriteError(string methodName,object request,PlayFabError playFabError,long responseTime)
		{
#if UNITY_EDITOR
			var requestText = JsonConvert.SerializeObject(request,Formatting.Indented);

			_WriteDump(methodName,requestText,null,playFabError,responseTime);
#endif
			LogTag.System.E($"PlayFabError - [Code : {playFabError.Error} / Message : {playFabError.ErrorMessage} / Details : {JsonConvert.SerializeObject(playFabError.ErrorDetails,Formatting.Indented)}]");
		}

#if UNITY_EDITOR
		private void _WriteDump(string requestMethodName,string requestText,string responseText,PlayFabError playFabError,long responseTime)
		{
			var dumpBuilder = new StringBuilder();

			dumpBuilder.Append("================= [PlayFab Dump] =================\n\n");
			dumpBuilder.Append($"[TIME]\n{DateTime.Now:yyyy\\/MM\\/dd\\ HH:mm:ss}\n\n");
			dumpBuilder.Append($"[REQUEST]\n");
			dumpBuilder.Append($"[REQUEST]\n{requestMethodName}\n");

			if(!requestText.IsEmpty())
			{
				dumpBuilder.Append($"[REQUEST CONTENT]\n{requestText}\n");
			}

			dumpBuilder.Append("\n[RESPONSE]\n");
			dumpBuilder.Append($"[RESPONSE TIME]\n{responseTime}\n");

			if(playFabError == null)
			{
				dumpBuilder.Append($"[RESPONSE RESULT]\nSUCCESS\n");
				dumpBuilder.Append($"[RESPONSE CODE]\n{PlayFabErrorCode.Success}\n");
				dumpBuilder.Append($"[RESPONSE CONTENT]\n{responseText}\n");
			}
			else
			{
				dumpBuilder.Append($"[RESPONSE RESULT]\nFAILURE\n");
				dumpBuilder.Append($"[RESPONSE CODE]\n{playFabError.Error}\n");
				dumpBuilder.Append($"[RESPONSE ERROR MESSAGE]\n{playFabError.ErrorMessage}\n");

				if(playFabError.ErrorDetails != null)
				{
					dumpBuilder.Append($"[RESPONSE ERROR DETAILS]\n{JsonConvert.SerializeObject(playFabError.ErrorDetails,Formatting.Indented)}\n");
				}
			}

			var filePath = Path.Combine(Global.PROJECT_PARENT_PATH,"NetworkDump",$"{requestMethodName}.log");

			FileUtility.WriteTextToFile(filePath,dumpBuilder.ToString());
		}
#endif
	}
}
#endif