using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public class DiscordPostWebRequest : DiscordWebRequest
	{
		protected DiscordPostWebRequest(string _uri,string _content,EmbedData[] _embedArray,byte[] _file) : base(_uri,UnityWebRequest.kHttpVerbPOST)
		{
			//? 제목 - [버전]
			var content = string.Format("```ansi\n{0} - [{1}]\n```",_content,GameSettings.In.GameVersion);
			//? 기기ID
			var username = string.Format("{0}",GameSettings.In.GetPresetOrDeviceID());

			var webHook = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new WebHookData(content,username,_embedArray)));

			if(_file == null)
			{
				m_WebRequest.uploadHandler = new UploadHandlerRaw(webHook) { contentType = "application/json", };
			}
			else
			{
				using var stream = new MemoryStream();
				var boundary = Encoding.ASCII.GetString(UnityWebRequest.GenerateBoundary());

				AddStreamField(stream,boundary,"Content-Disposition: form-data; name=\"files[0]\"; filename=\"image.png\"\r\n","Content-Type: application/octet-stream\r\n\r\n",_file);
				AddStreamField(stream,boundary,"Content-Disposition: form-data; name=\"payload_json\"\r\n","Content-Type: application/octet-stream\r\n\r\n",webHook);
				SetStreamEnd(stream,boundary);

				m_WebRequest.uploadHandler = new UploadHandlerRaw(stream.ToArray()) { contentType = string.Format("multipart/form-data; boundary={0}",boundary), };
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

			//? 필드 1개의 맥스가 1024 이므로 그거에 맞춰서 분할한다.
			var dataQueue = new Queue<MessageData>();
			var maxSize = FIELDS_VALUE_MAX_SIZE;
			var logCount = 0;

			foreach(var message in _messageGroup)
			{
				var head = message.Head;
				var body = message.Body;
				var index = 0;

				while(index < body.Length)
				{
					var text = body.Substring(index,Mathf.Min(body.Length-index,maxSize));

					dataQueue.Enqueue(new MessageData(head,text));
					logCount += head.Length+text.Length;

					while(logCount >= EMBED_MAX_TEXT_SIZE)
					{
						var data = dataQueue.Dequeue();

						logCount -= data.Head.Length+data.Body.Length;
					}

					index += body.Length;
				}
			}

			//? 그렇게 분할이 끝난 데이터로 embed를 만든다. (embed 의 최대는 10개 이므로 순환 큐로 버린다.)
			var embedQueue = new CircularQueue<EmbedData>(EMBED_MAX_COUNT);
			var fieldList = new List<FieldData>(FIELDS_MAX_COUNT);

			foreach(var data in dataQueue)
			{
				fieldList.Add(new FieldData(data.Head,data.Body));

				if(fieldList.Count == FIELDS_MAX_COUNT)
				{
					embedQueue.Enqueue(new EmbedData(10926864,fieldList.ToArray()));

					fieldList.Clear();
				}
			}

			if(!fieldList.IsNullOrEmpty())
			{
				embedQueue.Enqueue(new EmbedData(10926864,fieldList.ToArray()));
			}

			return new DiscordPostWebHookWebRequest(_uri,_content,embedQueue.ToArray(),_file);
		}

		private DiscordPostWebHookWebRequest(string _uri,string _content,EmbedData[] _embedArray,byte[] _file) : base(_uri,_content,_embedArray,_file) { }
	}
}