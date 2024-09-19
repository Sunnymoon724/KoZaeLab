using System;

namespace KZLib.KZFiles
{
	public record NoteEvent : MidiEvent
	{
		public NoteEvent(byte _command,byte _data1,byte _data2,int _delta,int _channel) : base(_command,_data1,_data2,_delta,_channel)
		{
			if(m_Data2 > 127)
			{
				m_Data2 &= 127;
			}

			if(m_Data2 == 0x00)
			{
				CommandCode = 0x80;
			}
		}

		private static readonly string[] NOTE_NAME_ARRAY = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

		public virtual byte NoteNumber
		{
			get => m_Data1;
			set
			{
				if(value < 0 || value > 127)
				{
					throw new ArgumentOutOfRangeException($"노트는 1-127 사이 입니다. [{value}]");
				}

				m_Data1 = value;
			}
		}
		
		public byte Velocity
		{
			get => m_Data2;
			set
			{
				if(value < 0 || value > 127)
				{
					throw new ArgumentOutOfRangeException($"속도는 1-127 사이 입니다. [{value}]");
				}

				m_Data2 = value;
			}
		}
		
		public string NoteName
		{
			get
			{
				if(Channel == 10)
				{
					return NoteNumber switch
					{
						35 => "Acoustic Bass Drum",
						36 => "Bass Drum 1",
						37 => "Side Stick",
						38 => "Acoustic Snare",
						39 => "Hand Clap",
						40 => "Electric Snare",
						41 => "Low Floor Tom",
						42 => "Closed Hi-Hat",
						43 => "High Floor Tom",
						44 => "Pedal Hi-Hat",
						45 => "Low Tom",
						46 => "Open Hi-Hat",
						47 => "Low-Mid Tom",
						48 => "Hi-Mid Tom",
						49 => "Crash Cymbal 1",
						50 => "High Tom",
						51 => "Ride Cymbal 1",
						52 => "Chinese Cymbal",
						53 => "Ride Bell",
						54 => "Tambourine",
						55 => "Splash Cymbal",
						56 => "Cowbell",
						57 => "Crash Cymbal 2",
						58 => "Vibraslap",
						59 => "Ride Cymbal 2",
						60 => "Hi Bongo",
						61 => "Low Bongo",
						62 => "Mute Hi Conga",
						63 => "Open Hi Conga",
						64 => "Low Conga",
						65 => "High Timbale",
						66 => "Low Timbale",
						67 => "High Agogo",
						68 => "Low Agogo",
						69 => "Cabasa",
						70 => "Maracas",
						71 => "Short Whistle",
						72 => "Long Whistle",
						73 => "Short Guiro",
						74 => "Long Guiro",
						75 => "Claves",
						76 => "Hi Wood Block",
						77 => "Low Wood Block",
						78 => "Mute Cuica",
						79 => "Open Cuica",
						80 => "Mute Triangle",
						81 => "Open Triangle",
						_ => $"Drum {NoteNumber}",
					};
				}

				return $"{NOTE_NAME_ARRAY[NoteNumber % 12]}{(NoteNumber / 12)}";
			}
		}

		public override string ToString()
		{
			return $"{base.ToString()} {NoteName} Vel:{Velocity}";
		}
	}
}