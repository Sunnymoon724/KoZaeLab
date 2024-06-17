using System.Collections.Generic;

namespace KZLib.KZFiles
{
	public record MidiTrack
	{
		public long AbsoluteTime { get; }
		private readonly List<MidiEvent> m_EventList = null;

		public IEnumerable<MidiEvent> EventGroup => m_EventList;
		
		public MidiTrack(List<MidiEvent> _eventList,long _time)
		{
			AbsoluteTime = _time;

			m_EventList = new List<MidiEvent>(_eventList.Count);
			m_EventList.AddRange(_eventList);
		}

		public int EventCount => m_EventList.Count;
	}
}