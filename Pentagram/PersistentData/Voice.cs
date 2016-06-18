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
	public class Voice : OpenableObservableData
	{
		private Ritmi _ritmo = All.DEFAULT_RITMO;
		[DataMember]
		public Ritmi Ritmo { get { return _ritmo; } private set { _ritmo = value; RaisePropertyChanged(); } }

		private Chiavi _chiave = All.DEFAULT_CHIAVE;
		[DataMember]
		public Chiavi Chiave { get { return _chiave; } private set { _chiave = value; RaisePropertyChanged(); } }

		// we cannot make this readonly because it is serialised. we only use the setter for serialising.
		private SwitchableObservableCollection<Chord> _chords = new SwitchableObservableCollection<Chord>();
		[DataMember]
		public SwitchableObservableCollection<Chord> Chords { get { return _chords; } private set { _chords = value; } }

		public Voice(Ritmi ritmo, Chiavi chiave)
		{
			Ritmo = ritmo;
			Chiave = chiave;
		}

		public void AddChord(Chord chord)
		{
			if (chord == null) return;
			_chords.Add(chord);
		}
		public void RemoveChord(Chord chord)
		{
			if (chord == null) return;
			var idx = _chords.IndexOf(chord);

			for (int i = idx - 1; i >= 0; i--)
			{
				if (_chords[i].PrevJoinedChords.Contains(chord)) _chords[i].PrevJoinedChords.Remove(chord);
				else break;
			}
			for (int i = idx + 1; i < _chords.Count; i++)
			{
				if (_chords[i].NextJoinedChords.Contains(chord)) _chords[i].NextJoinedChords.Remove(chord);
				else break;

			}

			_chords.Remove(chord);
		}
		public void AddNoteToChord(Chord chord, Note note)
		{
			if (chord == null || note == null) return;
			var selectedChord = _chords.FirstOrDefault(cho => cho == chord);
			if (selectedChord == null) return;
			selectedChord.Touches.Add(note);
		}
		public void RemoveNoteFromChord(Chord chord, Note note)
		{
			if (chord == null || note == null) return;
			var selectedChord = _chords.FirstOrDefault(cho => cho == chord);
			if (selectedChord == null) return;
			selectedChord.Touches.Remove(note);
			if (selectedChord.Touches.Count < 1) _chords.Remove(selectedChord);
		}
		public void AddPauseToChord(Chord chord, Pause pause)
		{
			if (chord == null || pause == null) return;
			var selectedChord = _chords.FirstOrDefault(cho => cho == chord);
			if (selectedChord == null) return;
			selectedChord.Touches.Clear();
			selectedChord.Touches.Add(pause);
		}
		public void RemovePauseFromChord(Chord chord, Pause pause)
		{
			if (chord == null || pause == null) return;
			var selectedChord = _chords.FirstOrDefault(cho => cho == chord);
			if (selectedChord == null) return;
			selectedChord.Touches.Remove(pause);
			if (selectedChord.Touches.Count < 1) _chords.Remove(selectedChord);
		}
		public void LinkChord1ToChord2(Chord chord1, Chord chord2)
		{
			int chord1Idx = _chords.IndexOf(chord1);
			int chord2Idx = _chords.IndexOf(chord2);
			if (chord1Idx + 1 == chord2Idx)
			{
				if (!chord2.PrevJoinedChords.Contains(chord1)) chord2.PrevJoinedChords.Add(chord1);
				if (!chord1.NextJoinedChords.Contains(chord2)) chord1.NextJoinedChords.Add(chord2);
			}
			else if (chord1Idx - 1 == chord2Idx)
			{
				if (!chord2.NextJoinedChords.Contains(chord1)) chord2.NextJoinedChords.Add(chord1);
				if (!chord1.PrevJoinedChords.Contains(chord2)) chord1.PrevJoinedChords.Add(chord2);
			}
		}
		public void UnlinkChord1FromChord2(Chord chord1, Chord chord2)
		{
			int chord1Idx = _chords.IndexOf(chord1);
			int chord2Idx = _chords.IndexOf(chord2);
			if (chord1Idx < chord2Idx)
			{
				chord2.PrevJoinedChords.Remove(chord1);
				chord1.NextJoinedChords.Remove(chord2);
			}
			else if (chord1Idx > chord2Idx)
			{
				chord2.NextJoinedChords.Remove(chord1);
				chord1.PrevJoinedChords.Remove(chord2);
			}
		}
	}
}
