using System;

namespace KZLib.KZFiles
{
	public class SystemEvent : MidiEvent 
	{
		private byte[] m_DataArray;

		public SystemEvent(byte[] _dataArray,int _delta,int _channel) : base(0xF0,0x00,0x00,_delta,_channel)
		{
			m_DataArray = new byte[_dataArray.Length];
			Array.Copy(_dataArray,m_DataArray,m_DataArray.Length);
		}

		// public override string ToString() 
		// {
		// 	var builder = new StringBuilder();

		// 	foreach(var data in m_DataArray)
		// 	{
		// 		builder.AppendFormat("{0:X2} ",data);
		// 	}

		// 	return string.Format("{0} Sysex: {1} bytes\r\n{2}",AbsoluteTime,m_DataArray.Length,builder.ToString());
		// }
	}
}