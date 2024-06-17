using System;
using UnityEngine;

namespace KZLib
{
	public record LogData : MessageData
	{
		public LogType DataType { get; }
		public string Time { get; }

		public LogData(LogType _type,string _text) : base(string.Format("<{0}> [{1}]",_type,DateTime.Now.ToString("MM/dd HH:mm:ss:ff")),_text)
		{
			DataType |= _type;
			// m_LogType |= (long)(1 << (int)_type);
			Time = string.Format("[{0}]",DateTime.Now.ToString("MM/dd HH:mm:ss:ff"));
		}
	}
}