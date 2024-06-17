using System.IO;
using System.Text;

namespace KZLib.KZNetwork
{
	//? https://discohook.org/ 여기서 웹훅 테스트 가능
	public abstract class DiscordWebRequest : BaseWebRequest
	{
		protected const int FIELDS_VALUE_MAX_SIZE = 1024;
		protected const int FIELDS_MAX_COUNT = 25;
		protected const int EMBED_MAX_COUNT = 10;
		protected const int EMBED_MAX_TEXT_SIZE = 6000;

		//? discord webhook에서 읽는 json 구조
#pragma warning disable IDE1006
		protected record WebHookData(string content,string username,EmbedData[] embeds);
		protected record EmbedData(int color,FieldData[] fields);
		protected record FieldData(string name,string value);
#pragma warning restore IDE1006

		protected DiscordWebRequest(string _uri,string _method) : base(_uri, _method) { }

		protected void AddStreamField(MemoryStream _stream,string _boundary,string _disposition,string _type,byte[] _data)
		{
			var begin = Encoding.UTF8.GetBytes(string.Format("{0}{1}\r\n",_stream.Length > 0 ? "\r\n--" : "--",_boundary));
			var type = Encoding.UTF8.GetBytes(_type);
			var disposition = Encoding.UTF8.GetBytes(_disposition);

			_stream.Write(begin,0,begin.Length);
			_stream.Write(disposition,0,disposition.Length);
			_stream.Write(type,0,type.Length);
			_stream.Write(_data,0,_data.Length);
		}

		protected void SetStreamEnd(MemoryStream _stream,string _boundary)
		{
			var end = Encoding.UTF8.GetBytes(string.Format("\r\n--{0}--",_boundary));

			_stream.Write(end,0,end.Length);
		}
	}
}