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

			m_EventDataList = GetMidiEventList();
			
			return true;
		}

		private List<MidiEventData> GetMidiEventList()
		{
			var tempoDataList = GetTempoDataList();
			var trackEventList = new List<MidiEventData>();
			var tempo = 0;
			var index = 0;

			var trackIterator = m_MidiFile.MidiTrackGroup.GetEnumerator();

			while(trackIterator.MoveNext())
			{
				if(trackIterator.Current is not MidiTrack track)
				{
					continue;
				}

				var iterator = track.EventGroup.GetEnumerator();
				tempo = 0;

				while(iterator.MoveNext())
				{
					var midiEvent = iterator.Current;

					while(tempo < tempoDataList.Count-1 && tempoDataList[tempo+1].FromTick < midiEvent.AbsoluteTime)
					{
						tempo++;
					}

					var newTime = tempoDataList[tempo].Cumulative+(midiEvent.AbsoluteTime-tempoDataList[tempo].FromTick)*tempoDataList[tempo].Ratio;

					if(midiEvent.CommandCode != 0x90)
					{
						continue;
					}
					
					trackEventList.Add(new MidiEventData(index,(float)newTime,midiEvent));
                }

				index++;
			}

			return trackEventList.Count == 0 ? trackEventList : trackEventList.OrderBy(x=>x.EventData.AbsoluteTime).ToList();
		}

		private List<TempoData> GetTempoDataList()
		{
			var tempoDataList = new List<TempoData>();
			var index = 0;
			var trackIterator = m_MidiFile.MidiTrackGroup.GetEnumerator();

			while(trackIterator.MoveNext())
			{
				var track = trackIterator.Current;

				if(track == null)
				{
					continue;
				}

				var iterator = track.EventGroup.GetEnumerator();

				while(iterator.MoveNext())
				{
					if(iterator.Current is not TempoEvent tempoEvent || tempoDataList.Count >= 1)
					{
						continue;
					}

					tempoDataList.Add(new TempoData(index,tempoEvent.AbsoluteTime,tempoDataList.Count > 0 ? tempoDataList[^1].Cumulative+tempoEvent.DeltaTime*tempoDataList[^1].Ratio : 0.0,tempoEvent.MicrosecondsPerQuarterNote/(double)m_MidiFile.DeltaTicksPerQuarterNote/1000.0,tempoEvent.MicrosecondsPerQuarterNote));
				}

				index++;
			}

			if(tempoDataList.Count == 0)
			{
				tempoDataList.Add(new TempoData(0,0L,0.0,500.0/m_MidiFile.DeltaTicksPerQuarterNote,500000));
			}
			else
			{
				if(tempoDataList.Count > 1)
				{
					tempoDataList = tempoDataList.OrderBy(x=>x.FromTick).ToList();
				}
			}

			return tempoDataList;
		}
	}

	public record MidiEventData(int IndexTrack,float RealTime,MidiEvent EventData);
}