using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Utilz;
using Utilz.Data;

namespace Pentagram.PersistentData
{
	[DataContract]
	public class Chord
	{
		// we cannot make this readonly because it is serialised. we only use the setter for serialising.
		private SwitchableObservableCollection<Touch> _touches = new SwitchableObservableCollection<Touch>();
		[DataMember]
		public SwitchableObservableCollection<Touch> Touches { get { return _touches; } private set { _touches = value; } }
		// we cannot make this readonly because it is serialised. we only use the setter for serialising.
		private SwitchableObservableCollection<Chord> _prevJoinedChords = new SwitchableObservableCollection<Chord>();
		[DataMember]
		public SwitchableObservableCollection<Chord> PrevJoinedChords { get { return _prevJoinedChords; } private set { _prevJoinedChords = value; } }
		// we cannot make this readonly because it is serialised. we only use the setter for serialising.
		private SwitchableObservableCollection<Chord> _nextJoinedChords = new SwitchableObservableCollection<Chord>();
		[DataMember]
		public SwitchableObservableCollection<Chord> NextJoinedChords { get { return _nextJoinedChords; } private set { _nextJoinedChords = value; } }

		public Chord(Note note)
		{
			if (note == null) throw new ArgumentOutOfRangeException("Chord ctor wants a note");
			_touches.Add(note);
		}
		public Chord(params Note[] notes)
		{
			if (notes == null) throw new ArgumentOutOfRangeException("Chord ctor wants some notes");
			_touches.AddRange(notes);
		}
		public Chord(Pause pause)
		{
			if (pause == null) throw new ArgumentOutOfRangeException("Chord ctor wants a pause");
			_touches.Add(pause);
		}
		public Note GetHighestNote()
		{
			Note result = null;
			foreach (Note note in _touches.Where(tou => tou is Note))
			{
				if (result == null || note.CompareTo(result) > 0) result = note;
			}
			return result;
		}
		public Note GetLowestNote()
		{
			Note result = null;
			foreach (Note note in _touches.Where(tou => tou is Note))
			{
				if (result == null || note.CompareTo(result) < 0) result = note;
			}
			return result;
		}
	}
}
