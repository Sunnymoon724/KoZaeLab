using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace KZLib
{
	//? https://discohook.org/ -> webhook test link

	/// <summary>
	/// Discord Webhook
	/// </summary>
	public abstract class DiscordWebRequest : BaseWebRequest
	{
		protected const int c_file_max_size = 1024;
		protected const int c_file_max_count = 25;
		protected const int c_embed_max_count = 10;
		protected const int c_embed_max_text_size = 6000;

		// WebHookData -> string content,string username,EmbedData[] embeds
		// EmbedData -> int color,FieldData[] fields
		// FieldData -> string name,string value
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
				m_webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(webHookText)) { contentType = "application/json", };
			}
			else
			{
				var form = new WWWForm();
				form.AddBinaryData("files[0]",file,"image.png","image/png");
				form.AddField("payload_json",webHookText);

				m_webRequest.uploadHandler = new UploadHandlerRaw(form.data) { contentType = form.headers["Content-Type"], };
			}
		}
	}

	public class PostDiscordWebHookWebRequest : PostDiscordWebRequest
	{
		private const string c_post_discord_webHook = "Post DiscordWebHook";

		public static PostDiscordWebHookWebRequest Create(string uri,string content,IEnumerable<MessageData> messageGroup = null,byte[] file = null)
		{
			if(uri.IsEmpty())
			{
				return null;
			}

			if(messageGroup.IsNullOrEmpty())
			{
				return new PostDiscordWebHookWebRequest(c_post_discord_webHook,uri,content,null,file);
			}

			//? Fields max count = 1024 -> device message
			var dataQueue = new Queue<MessageData>();
			var maxSize = c_file_max_size;
			var logCount = 0;

			foreach(var message in messageGroup)
			{
				var header = message.Header;
				var body = message.Body;
				var index = 0;

				while(index < body.Length)
				{
					var text = body.Substring(index,Mathf.Min(body.Length-index,maxSize));

					dataQueue.Enqueue(new MessageData(header,text));
					logCount += header.Length+text.Length;

					while(logCount >= c_embed_max_text_size)
					{
						var data = dataQueue.Dequeue();

						logCount -= data.Header.Length+data.Body.Length;
					}

					index += body.Length;
				}
			}

			//? Convert embeds -> embed max count = 10
			var embedQueue = new CircularQueue<object>(c_embed_max_count);
			var fieldList = new List<object>(c_file_max_count);

			foreach(var data in dataQueue)
			{
				fieldList.Add(new {name = data.Header,value = data.Body});

				if(fieldList.Count == c_file_max_count)
				{
					embedQueue.Enqueue(new { color = 10926864,fields = fieldList.ToArray() });

					fieldList.Clear();
				}
			}

			if(!fieldList.IsNullOrEmpty())
			{
				embedQueue.Enqueue(new { color = 10926864,fields = fieldList.ToArray() });
			}

			return new PostDiscordWebHookWebRequest(c_post_discord_webHook,uri,content,embedQueue.ToArray(),file);
		}

		private PostDiscordWebHookWebRequest(string name,string uri,string content,object[] embedArray,byte[] file) : base(name,uri,content,embedArray,file) { }
	}
}