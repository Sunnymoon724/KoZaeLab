using System;

namespace KZLib.KZFiles
{
	public record SystemEvent : MidiEvent 
	{
		private readonly byte[] m_DataArray;

		public SystemEvent(byte[] _dataArray,int _delta,int _channel) : base(0xF0,0x00,0x00,_delta,_channel)
		{
			m_DataArray = new byte[_dataArray.Length];

			Array.Copy(_dataArray,m_DataArray,m_DataArray.Length);
		}
	}
}