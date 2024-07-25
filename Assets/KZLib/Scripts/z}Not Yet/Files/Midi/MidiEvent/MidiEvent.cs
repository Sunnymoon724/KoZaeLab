using System;

namespace KZLib.KZFiles
{
	public record MidiEvent
	{
		private int m_Channel;
		public byte CommandCode { get; protected set; }
		public int DeltaTime { get; }
		public long AbsoluteTime { get; set; }

		protected byte m_Data1;
		protected byte m_Data2;

		public MidiEvent(byte _command,byte _data1,byte _data2,int _delta,int _channel)
		{
			CommandCode = _command;
			Channel = _channel;
			DeltaTime = _delta;

			m_Data1 = _data1;
			m_Data2 = _data2;
		}

		public virtual int Channel
		{
			get => m_Channel;
			set
			{
				if((value < 1) || (value > 16))
				{
					throw new ArgumentOutOfRangeException(string.Format("채널은 1-16 사이 입니다. [{0}]",value));
				}

				m_Channel = value;
			}
		}
		
		public static bool IsEndTrack(MidiEvent _event)
		{
			if(_event == null)
			{
				return false;
			}

			var metaEvent = _event as MetaEvent;

			if(metaEvent != null)
			{
				return metaEvent.MetaStatus == 0x2F;
			}

			return false;
		}
		
		public override string ToString()
		{
			return (CommandCode >= 0xF0) ? string.Format("{0} {1}",AbsoluteTime,CommandCode) : string.Format("{0} {1} Ch: {2}",AbsoluteTime,CommandCode,m_Channel);
		}
	}
}