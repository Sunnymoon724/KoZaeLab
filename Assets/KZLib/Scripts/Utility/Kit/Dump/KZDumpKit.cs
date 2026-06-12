using System.Collections.Generic;
using System.IO;
using System.Text;
using KZLib;
using Newtonsoft.Json;

/// <summary>
/// Editor-only utility methods for writing formatted network request/response dumps to disk.
/// </summary>
public static class KZDumpKit
{
	private const string c_dumpFolderName = "NetworkDump";

	/// <summary>
	/// Writes a formatted PlayFab request/response dump to the NetworkDump folder.
	/// </summary>
	public static void WritePlayFabDump(string requestMethodName,string requestText,bool responseResult,string responseText,long responseTimeMs)
	{
#if UNITY_EDITOR
		var builder = _CreateBuilder("Network Dump");

		_AppendTime(builder);
		_AppendLine(builder,"REQUEST",requestMethodName);

		if(!requestText.IsEmpty())
		{
			builder.Append($"[REQUEST CONTENT]\n{requestText}\n");
		}

		_AppendResponse(builder,responseTimeMs,responseResult,responseText,0,null,false,true);
		_WriteFile($"{requestMethodName}.log",builder);
#endif
	}

	/// <summary>
	/// Writes a formatted HTTP request/response dump to the NetworkDump folder.
	/// </summary>
	public static void WriteHttpDump(string fileName,long responseTimeMs,bool success,long responseCode,string responseContent,string errorMessage,string requestUrl,string requestMethod,string requestPayload,string requestContentType)
	{
#if UNITY_EDITOR
		var builder = _CreateBuilder("HTTP Dump");

		_AppendTime(builder);
		_AppendLine(builder,"FULL URL",requestUrl);
		_AppendLine(builder,"REQUEST METHOD",requestMethod);

		if(!requestContentType.IsEmpty())
		{
			if(!requestPayload.IsEmpty())
			{
				builder.Append($"[REQUEST PAYLOAD]\n{KZExternalKit.PrettifyJson(requestPayload)}\n");
			}

			builder.Append($"\n[REQUEST CONTENT TYPE]\n{requestContentType}\n");
		}

		_AppendResponse(builder,responseTimeMs,success,responseContent,responseCode,errorMessage,true,false);
		_WriteFile(fileName,builder);
#endif
	}

#if UNITY_EDITOR
	private static StringBuilder _CreateBuilder(string title)
	{
		var builder = new StringBuilder();

		builder.Append($"================= [{title}] =================\n\n");

		return builder;
	}

	private static void _AppendTime(StringBuilder builder)
	{
		var currentTime = ServerClockManager.In.GetNow(true);

		builder.Append($"[TIME]\n{currentTime:yyyy\\/MM\\/dd\\ HH:mm:ss}\n\n");
	}

	private static void _AppendLine(StringBuilder builder,string label,string value)
	{
		builder.Append($"[{label}]\n{value}\n\n");
	}

	/// <summary>
	/// Appends response metadata and content, optionally parsing PlayFab failure JSON for Message/Details fields.
	/// </summary>
	private static void _AppendResponse(StringBuilder builder,long responseTimeMs,bool success,string responseContent,long responseCode,string errorMessage,bool prettifyContent,bool parsePlayFabFailureJson)
	{
		builder.Append("\n[RESPONSE]\n");
		builder.Append($"[RESPONSE TIME]\n{responseTimeMs}\n");
		builder.Append($"[RESPONSE RESULT]\n{(success ? "SUCCESS" : "FAILURE")}\n");

		if(responseCode != 0)
		{
			builder.Append($"[RESPONSE CODE]\n{responseCode}\n");
		}

		if(!responseContent.IsEmpty())
		{
			var content = prettifyContent ? KZExternalKit.PrettifyJson(responseContent) : responseContent;

			builder.Append($"[RESPONSE CONTENT]\n{content}\n");
		}

		if(success)
		{
			return;
		}

		if(parsePlayFabFailureJson && !responseContent.IsEmpty())
		{
			try
			{
				var errorDict = JsonConvert.DeserializeObject<Dictionary<string,string>>(responseContent);

				if(errorDict != null && errorDict.TryGetValue("Message",out var message))
				{
					builder.Append($"[RESPONSE ERROR MESSAGE]\n{message}\n");

					if(errorDict.TryGetValue("Details",out var details))
					{
						builder.Append($"[RESPONSE ERROR DETAILS]\n{details}\n");
					}

					return;
				}
			}
			catch
			{
				// Fall back to raw error text below.
			}
		}

		if(!errorMessage.IsEmpty())
		{
			builder.Append($"[RESPONSE ERROR MESSAGE]\n{errorMessage}\n");
		}
	}

	private static void _WriteFile(string fileName,StringBuilder builder)
	{
		var filePath = Path.Combine(Global.ProjectParentPath,c_dumpFolderName,fileName);

		KZFileKit.WriteTextToFile(filePath,builder.ToString());
	}
#endif
}
