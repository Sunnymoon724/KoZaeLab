using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace KZLib.KZFiles
{
	public class MidiFile
	{
		private const string FILE_HEADER_CHUNK = "MThd";
		private const string FILE_TRACK_CHUNK = "MTrk";

		private List<MidiTrack> m_MidiTrackList = null;

		public IEnumerable<MidiTrack> MidiTrackGroup => m_MidiTrackList;

		public int DeltaTicksPerQuarterNote => m_DeltaTicksPerQuarterNote;

		// 0 = single track, 1 = multi track synchronous, 2 = multi track asynchronous
		private short m_FileFormat = 0;
		public short FileFormat => m_FileFormat;

		private short m_DeltaTicksPerQuarterNote = 0;

		public MidiFile(string _fullPath)
		{
			using var reader = new BinaryReader(File.OpenRead(_fullPath));
			LoadMidiFile(reader);
		}

		private void LoadMidiFile(BinaryReader _reader)
		{
			var chunkId = Encoding.UTF8.GetString(_reader.ReadBytes(4));

			if(chunkId != FILE_HEADER_CHUNK)
			{
				throw new InvalidOperationException($"{chunkId} is not {FILE_HEADER_CHUNK}");
			}

			var chunkSize = _reader.ReadInt32();

			if(chunkSize != 6)
			{
				throw new InvalidOperationException("size must be 6byte.");
			}

			m_FileFormat = _reader.ReadInt16();
			int trackCount = _reader.ReadInt16();

			m_DeltaTicksPerQuarterNote = _reader.ReadInt16();
			m_MidiTrackList = new List<MidiTrack>(trackCount);

			for(var i=0;i<trackCount;i++)
			{
				m_MidiTrackList.Add(ReadTrack(_reader));
			}
		}

		private MidiTrack ReadTrack(BinaryReader _reader)
		{
			var eventList = new List<MidiEvent>();
			long totalTime = 0L;

			var chunkId = Encoding.UTF8.GetString(_reader.ReadBytes(4));

			if(chunkId != FILE_TRACK_CHUNK)
			{
				throw new InvalidOperationException($"{chunkId} is not {FILE_TRACK_CHUNK}");
			}

			var chunkSize = _reader.ReadInt32();
			var startPosition = _reader.BaseStream.Position;
			var endPosition = startPosition+chunkSize;
			
			MidiEvent nowEvent = null;

			while(_reader.BaseStream.Position < endPosition)
			{
				nowEvent = ReadNextEvent(_reader,nowEvent);
				totalTime += nowEvent.DeltaTime;
				
				nowEvent.AbsoluteTime = totalTime;

				eventList.Add(nowEvent);
			}

			if(_reader.BaseStream.Position != endPosition)
			{
				throw new ArgumentException("Track chunk size is not enough.");
			}

			return new MidiTrack(eventList,totalTime);
		}

		private MidiEvent ReadNextEvent(BinaryReader _reader,MidiEvent _previous)
		{
			var deltaTime = _reader.ReadVariableLength();
			var channel = 1;
			var status = _reader.ReadByte();

			byte code;

			if((status & 0x80) == 0)
			{
				code = _previous.CommandCode;
				channel = _previous.Channel;
				_reader.BaseStream.Position--;
			}
			else
			{
				if((status & 0xF0) == 0xF0)
				{
					code = status;
				}
				else
				{
					code = (byte)(status & 0xF0);
					channel = (status & 0x0F)+1;
				}
			}

			return code switch
			{
				//NoteOn
				0x90 => new NoteEvent(code,_reader.ReadByte(),_reader.ReadByte(),deltaTime,channel),
				//NoteOff
				0x80 or 0xA0 or 0xB0 or 0xE0 => new MidiEvent(code,_reader.ReadByte(),_reader.ReadByte(),deltaTime,channel),
				//ProgramChange
				0xC0 or 0xD0 => new MidiEvent(code,_reader.ReadByte(),0x00,deltaTime,channel),
				//TimingClock
				0xF8 or 0xFA or 0xFB or 0xFC => new MidiEvent(code,0x00,0x00,deltaTime,channel),
				//StartOfSystemExclusiveMessage
				0xF0 => ReadSystemEvent(_reader,deltaTime, channel),
				//MetaEvent
				0xFF => ReadMetaEvent(_reader,deltaTime),
				_ => throw new ArgumentException($"Not supported code. [{code:X2}]"),
			};
		}

		private MidiEvent ReadSystemEvent(BinaryReader _reader,int _deltaTime,int _channel)
		{
			var dataList = new List<byte>();
			var status = _reader.ReadByte();

			while(status != 0xF7)
			{
				dataList.Add(status);
				status = _reader.ReadByte();
			}

			return new SystemEvent(dataList.ToArray(),_deltaTime,_channel);
		}

		private MidiEvent ReadMetaEvent(BinaryReader _reader,int _deltaTime)
		{
			var status = _reader.ReadByte();
			int length = _reader.ReadVariableLength();

			switch(status) 
			{
				case 0x00: //TrackSequenceNumber
					return new TrackSequenceNumberEvent(_reader,length,_deltaTime);
				case 0x01: //TextEvent
				case 0x02: //Copyright
				case 0x03: //SequenceTrackName
				case 0x04: //TrackInstrumentName
				case 0x05: //Lyric
				case 0x06: //Marker
				case 0x07: //CuePoint
				case 0x08: //ProgramName
				case 0x09: //DeviceName
					return new MetaTextEvent(Encoding.UTF8.GetString(_reader.ReadBytes(length),0,length),status,_deltaTime);
				case 0x2F: //EndTrack
					if(length != 0)
					{
						// TBN Change do nothing with this information but no exception
						_reader.ReadBytes(length);
					}
					return new MetaEvent(status,0x00,_deltaTime);
				case 0x51: //SetTempo
					return new TempoEvent(_reader,length,_deltaTime);
				case 0x58: //TimeSignature
					return new MetaTextEvent(_reader,length,4,status,_deltaTime);
				case 0x59: //KeySignature
					return new MetaTextEvent(_reader,length,2,status,_deltaTime);
				case 0x7F: //SequencerSpecific
					return new MetaDataEvent(status,_reader.ReadBytes(length),_deltaTime);
				case 0x54: //SmpteOffset
					return new MetaTextEvent(_reader,length,5,status,_deltaTime);
				default:
				
					var data = _reader.ReadBytes(length);

					if(data.Length != length)
					{
						throw new InvalidOperationException("MetaEvent is not complete.");
					}

					return new MetaDataEvent(status,data,_deltaTime);
			}
		}
	}
}
