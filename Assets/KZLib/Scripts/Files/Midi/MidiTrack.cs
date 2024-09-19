using System.Collections.Generic;

namespace KZLib.KZFiles
{
	public record MidiTrack
	{
		public long AbsoluteTime { get; }
		public IEnumerable<MidiEvent> EventGroup { get; }
		public int EventCount { get; }

		public MidiTrack(List<MidiEvent> _eventList,long _time)
		{
			AbsoluteTime = _time;

			EventCount = _eventList.Count;
			EventGroup = _eventList;
		}
	}
}