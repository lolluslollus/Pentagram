using Pentagram.Utilz;
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
	[DataContract(IsReference = true)]
	// [DataContract]
	public sealed class Chord : Sound
	{
		public event EventHandler ChromaFlagsPositionChanged;
		private bool _isChromaFlagsBelow = false;
		[DataMember]
		public bool IsChromaFlagsBelow { get { return _isChromaFlagsBelow; } private set { if (_isChromaFlagsBelow == value) return; _isChromaFlagsBelow = value; RaisePropertyChanged(); } }

		private SegniSuNote _segno = SegniSuNote.Nil;
		[DataMember]
		public SegniSuNote Segno { get { return _segno; } private set { if (_segno == value) return; _segno = value; RaisePropertyChanged(); } }

		// we cannot make this readonly because it is serialised. we only use the setter for serialising.
		private SortedSwitchableObservableCollection<Tone> _tones = new SortedSwitchableObservableCollection<Tone>();
		[DataMember]
		public SortedSwitchableObservableCollection<Tone> Tones { get { return _tones; } private set { _tones = value; } }

		private Chord _prevJoinedChord = null;
		[DataMember]
		public Chord PrevJoinedChord { get { return _prevJoinedChord; } set { if (_prevJoinedChord == value) return; _prevJoinedChord = value; RaisePropertyChanged(); } }

		private Chord _nextJoinedChord = null;
		[DataMember]
		public Chord NextJoinedChord { get { return _nextJoinedChord; } set { if (_nextJoinedChord == value) return; _nextJoinedChord = value; RaisePropertyChanged(); } }

		public Chord(Duration duration, SegniSuNote segno, params Tone[] tones) : base(duration)
		{
			if (tones == null) throw new ArgumentOutOfRangeException("Chord ctor wants some notes");
			Segno = segno;
			_tones.AddRange(tones);
		}
		public Chord(Duration duration, SegniSuNote segno, bool isChromaFlagsBelow, params Tone[] tones) : this(duration, segno, tones)
		{
			IsChromaFlagsBelow = isChromaFlagsBelow;
		}
		public void SetChromaFlagsPositions(bool below)
		{
			IsChromaFlagsBelow = below;

			Chord otherChord = PrevJoinedChord;
			while (otherChord != null)
			{
				otherChord.IsChromaFlagsBelow = below;
				otherChord = otherChord.PrevJoinedChord;
			}

			otherChord = NextJoinedChord;
			while (otherChord != null)
			{
				otherChord.IsChromaFlagsBelow = below;
				otherChord = otherChord.NextJoinedChord;
			}

			ChromaFlagsPositionChanged?.Invoke(this, EventArgs.Empty);
		}
		//public Tone GetHighestTone()
		//{
		//	Tone result = null;
		//	foreach (Tone tone in _tones)
		//	{
		//		if (result == null || tone.CompareTo(result) > 0) result = tone;
		//	}
		//	return result;
		//}
		//public Tone GetLowestTone()
		//{
		//	Tone result = null;
		//	foreach (Tone tone in _tones)
		//	{
		//		if (result == null || tone.CompareTo(result) < 0) result = tone;
		//	}
		//	return result;
		//}
	}
}
