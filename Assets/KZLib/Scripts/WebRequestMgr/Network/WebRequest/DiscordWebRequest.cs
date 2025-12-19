using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	//? https://discohook.org/ -> webhook test link

	/// <summary>
	/// Discord Webhook
	/// </summary>
	public abstract class DiscordWebRequest : BaseWebRequest
	{
		protected const int c_fileMaxSize = 1024;
		protected const int c_fileMaxCount = 25;
		protected const int c_embedMaxCount = 10;
		protected const int c_embedMaxTextSize = 6000;

		// WebHook -> string content,string username,Embed[] embeds
		// Embed -> int color,Field[] fields
		// Field -> string name,string value
		protected DiscordWebRequest(string name,string uri,string method) : base(name,uri,method) { }
	}

	public class PostDiscordWebRequest : DiscordWebRequest
	{
		protected PostDiscordWebRequest(string name,string uri,string content,object[] embedArray,byte[] file) : base(name,uri,UnityWebRequest.kHttpVerbPOST)
		{
			var webHookText = JsonConvert.SerializeObject(new
			{
				content = $"```ansi\n{content}\n```",
				username = $"{SystemInfo.deviceUniqueIdentifier}",
				embeds = embedArray
			});

			if(file == null)
			{
				_CreateUploadHandler(webHookText,"application/json");
			}
			else
			{
				var form = new WWWForm();
				form.AddBinaryData("files[0]",file,"image.png","image/png");
				form.AddField("payload_json",webHookText);

				var rawData = form.data; // create this time
				var contentType = form.headers["Content-Type"];

				_CreateUploadHandler(rawData,contentType);
			}
		}
	}

	public class PostDiscordWebHookWebRequest : PostDiscordWebRequest
	{
		private const string c_postDiscordWebHook = "Post DiscordWebHook";

		public static PostDiscordWebHookWebRequest Create(string uri,string content,IEnumerable<MessageInfo> messageGroup = null,byte[] file = null)
		{
			if(uri.IsEmpty())
			{
				return null;
			}

			if(messageGroup.IsNullOrEmpty())
			{
				return new PostDiscordWebHookWebRequest(c_postDiscordWebHook,uri,content,null,file);
			}

			//? Fields max count = 1024 -> device message
			var messageInfoQueue = new Queue<MessageInfo>();
			var maxSize = c_fileMaxSize;
			var logCount = 0;

			foreach(var message in messageGroup)
			{
				var header = message.Header;
				var body = message.Body;
				var index = 0;

				while(index < body.Length)
				{
					var text = body.Substring(index,Mathf.Min(body.Length-index,maxSize));

					messageInfoQueue.Enqueue(new MessageInfo(header,text));
					logCount += header.Length+text.Length;

					while(logCount >= c_embedMaxTextSize)
					{
						var messageInfo = messageInfoQueue.Dequeue();

						logCount -= messageInfo.Header.Length+messageInfo.Body.Length;
					}

					index += body.Length;
				}
			}

			//? Convert embeds -> embed max count = 10
			var embedQueue = new CircularQueue<object>(c_embedMaxCount);
			var fieldList = new List<object>(c_fileMaxCount);

			foreach(var messageInfo in messageInfoQueue)
			{
				fieldList.Add(new {name = messageInfo.Header,value = messageInfo.Body});

				if(fieldList.Count == c_fileMaxCount)
				{
					embedQueue.Enqueue(new { color = 10926864,fields = fieldList.ToArray() });

					fieldList.Clear();
				}
			}

			if(!fieldList.IsNullOrEmpty())
			{
				embedQueue.Enqueue(new { color = 10926864,fields = fieldList.ToArray() });
			}

			return new PostDiscordWebHookWebRequest(c_postDiscordWebHook,uri,content,embedQueue.ToArray(),file);
		}

		private PostDiscordWebHookWebRequest(string name,string uri,string content,object[] embedArray,byte[] file) : base(name,uri,content,embedArray,file) { }
	}
}