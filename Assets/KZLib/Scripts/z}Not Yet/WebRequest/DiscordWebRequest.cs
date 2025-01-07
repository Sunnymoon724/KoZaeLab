using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		protected const int FIELDS_MAX_SIZE = 1024;
		protected const int FIELDS_MAX_COUNT = 25;
		protected const int EMBED_MAX_COUNT = 10;
		protected const int EMBED_MAX_TEXT_SIZE = 6000;

		// WebHookData -> string content,string username,EmbedData[] embeds
		// EmbedData -> int color,FieldData[] fields
		// FieldData -> string name,string value
		protected DiscordWebRequest(string _uri,string _method) : base(_uri,_method) { }
	}

	#region Post
	public class DiscordPostWebRequest : DiscordWebRequest
	{
		protected DiscordPostWebRequest(string _uri,string _content,object[] _embedArray,byte[] _file) : base(_uri,UnityWebRequest.kHttpVerbPOST)
		{
			var webHookText = JsonConvert.SerializeObject(new
			{
				content = $"```ansi\n{_content}\n```",
				username = $"{GameSettings.In.PresetNameOrDeviceId}",
				embeds = _embedArray
			});

			if(_file == null)
			{
				m_WebRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(webHookText)) { contentType = "application/json", };
			}
			else
			{
				var form = new WWWForm();
				form.AddBinaryData("files[0]",_file,"image.png","image/png");
				form.AddField("payload_json",webHookText);

				m_WebRequest.uploadHandler = new UploadHandlerRaw(form.data) { contentType = form.headers["Content-Type"], };
			}
		}
	}

	public class DiscordPostWebHookWebRequest : DiscordPostWebRequest
	{
		public static DiscordPostWebHookWebRequest Create(string _uri,string _content,IEnumerable<MessageData> _messageGroup = null,byte[] _file = null)
		{
			if(_uri.IsEmpty())
			{
				return null;
			}

			if(_messageGroup.IsNullOrEmpty())
			{
				return new DiscordPostWebHookWebRequest(_uri,_content,null,_file);
			}

			//? Fields max count = 1024 -> device message
			var dataQueue = new Queue<MessageData>();
			var maxSize = FIELDS_MAX_SIZE;
			var logCount = 0;

			foreach(var message in _messageGroup)
			{
				var header = message.Header;
				var body = message.Body;
				var index = 0;

				while(index < body.Length)
				{
					var text = body.Substring(index,Mathf.Min(body.Length-index,maxSize));

					dataQueue.Enqueue(new MessageData(header,text));
					logCount += header.Length+text.Length;

					while(logCount >= EMBED_MAX_TEXT_SIZE)
					{
						var data = dataQueue.Dequeue();

						logCount -= data.Header.Length+data.Body.Length;
					}

					index += body.Length;
				}
			}

			//? Convert embeds -> embed max count = 10
			var embedQueue = new CircularQueue<object>(EMBED_MAX_COUNT);
			var fieldList = new List<object>(FIELDS_MAX_COUNT);

			foreach(var data in dataQueue)
			{
				fieldList.Add(new {name = data.Header,value = data.Body});

				if(fieldList.Count == FIELDS_MAX_COUNT)
				{
					embedQueue.Enqueue(new { color = 10926864,fields = fieldList.ToArray() });

					fieldList.Clear();
				}
			}

			if(!fieldList.IsNullOrEmpty())
			{
				embedQueue.Enqueue(new { color = 10926864,fields = fieldList.ToArray() });
			}

			return new DiscordPostWebHookWebRequest(_uri,_content,embedQueue.ToArray(),_file);
		}

		private DiscordPostWebHookWebRequest(string _uri,string _content,object[] _embedArray,byte[] _file) : base(_uri,_content,_embedArray,_file) { }
	}
	#endregion Post
}