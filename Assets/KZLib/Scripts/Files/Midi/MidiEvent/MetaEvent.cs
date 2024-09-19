using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KZLib.KZFiles
{
	public record MetaEvent : MidiEvent 
	{
		public byte MetaStatus => m_Data1;
		
		public MetaEvent(byte _status,byte _data2,int _delta) : base(0xFF,_status,_data2,_delta,1) { }
		
		public override string ToString() 
		{
			return $"{AbsoluteTime} {MetaStatus}";
		}
	}

	public record MetaTextEvent : MetaEvent 
	{
		public string Text { get; }

		public MetaTextEvent(string _text,byte _status,int _delta) : base(_status,0x00,_delta)
		{
			Text = _text;
		}

		public MetaTextEvent(BinaryReader _reader,int _length,int _count,byte _status,int _delta) : base(_status,0x00,_delta)
		{
			if(_length != _count)
			{
				throw new Exception("값이 유효하지 않습니다.");
			}

			Text = Encoding.UTF8.GetString(_reader.ReadBytes(_count));
		}
		
		public override string ToString() 
		{
			return $"{base.ToString()} {Text}";
		}
	}

	public record TrackSequenceNumberEvent : MetaEvent
	{
		public ushort SequenceNumber { get; }

		public TrackSequenceNumberEvent(BinaryReader _reader,int _length,int _delta) : base(0x00,0x00,_delta)
		{
			if(_length == 2)
			{
				SequenceNumber = (ushort)((_reader.ReadByte() << 8)+_reader.ReadByte());
			}
			else
			{
				_reader.ReadBytes(2);

				SequenceNumber = 0;
			}
		}
		
		public override string ToString()
		{
			return $"{base.ToString()} {SequenceNumber}";
		}
	}

	public record MetaDataEvent : MetaEvent
	{
		public byte[] DataArray { get; }
		
		public MetaDataEvent(byte _status,byte[] _dataArray,int _delta) : base(_status,0x00,_delta)
		{
			DataArray = new byte[_dataArray.Length];

			Array.Copy(_dataArray,DataArray,DataArray.Length);
		}
	}

	public record TempoEvent : MetaEvent 
	{
		public int MicrosecondsPerQuarterNote { get; }
		
		public TempoEvent(BinaryReader _reader,int _length,int _delta) : base(0x51,0x00,_delta)
		{
			if(_length != 3) 
			{
				throw new Exception("템포의 길이가 유효하지 않습니다.");
			}

			MicrosecondsPerQuarterNote = (_reader.ReadByte() << 16) + (_reader.ReadByte() << 8) + _reader.ReadByte();
		}
		
		public override string ToString() 
		{
			return $"{base.ToString()} {60000000.0d/MicrosecondsPerQuarterNote:F2} bpm ({MicrosecondsPerQuarterNote})";
		}
	}
}