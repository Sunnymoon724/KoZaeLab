using System.Collections.Generic;
using System.Linq;

namespace KZLib.KZFiles
{
	public class MidiFileLoader
	{
		private record TempoData(int Track,long FromTick,double Cumulative,double Ratio,int MicrosecondsPerQuarterNote);

		private MidiFile m_MidiFile = null;

		private List<MidiEventData> m_EventDataList = null;

		public IEnumerable<MidiEventData> MidiEventGroup => m_EventDataList;
		
		public bool LoadMidiFile(string _fullPath)
		{
			m_MidiFile = new MidiFile(_fullPath);

			if(m_MidiFile == null)
			{
				return false;
			}

			CreateMidiEventData();

			return true;
		}

		private void CreateMidiEventData()
		{
			var tempoDataList = GetTempoDataList().ToArray();
			var trackEventList = new List<MidiEventData>();

			foreach(var midiTrack in m_MidiFile.MidiTrackGroup)
			{
				if(midiTrack is not MidiTrack track)
				{
					continue;
				}

				var tempoIndex = 0;

				foreach(var midiEvent in track.EventGroup)
				{
					while(tempoIndex < tempoDataList.Length-1 && tempoDataList[tempoIndex+1].FromTick < midiEvent.AbsoluteTime)
					{
						tempoIndex++;
					}

					var newTime = tempoDataList[tempoIndex].Cumulative+(midiEvent.AbsoluteTime-tempoDataList[tempoIndex].FromTick)*tempoDataList[tempoIndex].Ratio;

					if(midiEvent.CommandCode != 0x90)
					{
						continue;
					}

					trackEventList.Add(new MidiEventData(trackEventList.Count,(float)newTime,midiEvent));
				}
			}

			m_EventDataList = trackEventList;
		}

		private List<TempoData> GetTempoDataList()
		{
			var tempoDataList = new List<TempoData>();

			foreach(var track in m_MidiFile.MidiTrackGroup)
			{
				if(track == null)
				{
					continue;
				}

				var cumulativeTime = 0.0;
				var lastTempo = 0;

				foreach(var midiEvent in track.EventGroup)
				{
					if(midiEvent is TempoEvent tempoEvent)
					{
						if(lastTempo > 0)
						{
							cumulativeTime += tempoEvent.DeltaTime*tempoDataList[lastTempo].Ratio;
						}

						tempoDataList.Add(new TempoData(tempoDataList.Count,tempoEvent.AbsoluteTime,cumulativeTime,tempoEvent.MicrosecondsPerQuarterNote/(double)m_MidiFile.DeltaTicksPerQuarterNote/1000.0,tempoEvent.MicrosecondsPerQuarterNote));

						lastTempo = tempoDataList.Count-1;
					}
				}
			}

			if(tempoDataList.Count == 0)
			{
				tempoDataList.Add(new TempoData(0,0L,0.0,500.0/m_MidiFile.DeltaTicksPerQuarterNote,500000));
			}

			return tempoDataList;
		}
	}

	public record MidiEventData(int IndexTrack,float RealTime,MidiEvent EventData);
}