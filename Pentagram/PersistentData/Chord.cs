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
	[DataContract(IsReference=true)]
	// [DataContract]
	public sealed class Chord : Sound
	{
		private SegniSuNote _segno = SegniSuNote.Nil;
		[DataMember]
		public SegniSuNote Segno { get { return _segno; } private set { _segno = value; RaisePropertyChanged(); } }

		// we cannot make this readonly because it is serialised. we only use the setter for serialising.
		private SwitchableObservableCollection<Tone> _tones = new SwitchableObservableCollection<Tone>();
		[DataMember]
		public SwitchableObservableCollection<Tone> Tones { get { return _tones; } private set { _tones = value; } }

		private Chord _prevJoinedChord = null;
		[DataMember]
		public Chord PrevJoinedChord { get { return _prevJoinedChord; } set { _prevJoinedChord = value; RaisePropertyChanged(); } }

		private Chord _nextJoinedChord = null;
		[DataMember]
		public Chord NextJoinedChord { get { return _nextJoinedChord; } set { _nextJoinedChord = value; RaisePropertyChanged(); } }

		public Chord(Duration duration, SegniSuNote segno, Tone tone) : base(duration)
		{
			if (tone == null) throw new ArgumentOutOfRangeException("Chord ctor wants a tone");
			Segno = segno;
			_tones.Add(tone);
		}
		public Chord(Duration duration, SegniSuNote segno, params Tone[] tones) : base(duration)
		{
			if (tones == null) throw new ArgumentOutOfRangeException("Chord ctor wants some notes");
			Segno = segno;
			_tones.AddRange(tones);
		}
		public Tone GetHighestTone()
		{
			Tone result = null;
			foreach (Tone tone in _tones)
			{
				if (result == null || tone.CompareTo(result) > 0) result = tone;
			}
			return result;
		}
		public Tone GetLowestTone()
		{
			Tone result = null;
			foreach (Tone tone in _tones)
			{
				if (result == null || tone.CompareTo(result) < 0) result = tone;
			}
			return result;
		}
	}
}
