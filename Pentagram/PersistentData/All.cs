using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilz;
using Utilz.Data;

namespace Pentagram.PersistentData
{
	public class All : OpenableObservableData
	{
		private readonly SwitchableObservableCollection<Song> _songs = new SwitchableObservableCollection<Song>();
		public SwitchableObservableCollection<Song> Songs { get { return _songs; } }

		private Song _currentSong = null;
		public Song CurrentSong { get { return _currentSong; } private set { _currentSong = value; RaisePropertyChanged(); } }
		public void AddSong(Song song)
		{
			if (song == null) return;
			_songs.Add(song);
		}
		public void RemoveSong(Song song)
		{
			if (song == null) return;
			_songs.Remove(song);
		}
		public void SetCurrentSong(Song song)
		{
			if (song == null || _songs.FirstOrDefault(so => so == song) == null) return;
			CurrentSong = song;
		}
	}

	public class Song : OpenableObservableData
	{
		private string _name = "Song";
		public string Name { get { return _name; } set { _name = value; RaisePropertyChanged(); } }
		private readonly SwitchableObservableCollection<Voice> _voices = new SwitchableObservableCollection<Voice>();
		public SwitchableObservableCollection<Voice> Voices { get { return _voices; } }

		public Song(string name)
		{
			Name = name;
		}
		public void AddVoice(Voice voice)
		{
			if (voice == null) return;
			_voices.Add(voice);
		}
		public void RemoveVoice(Voice voice)
		{
			if (voice == null) return;
			_voices.Remove(voice);
		}
	}

	public class Voice : OpenableObservableData
	{
		private Ritmi _ritmo = Ritmi.qq;
		public Ritmi Ritmo { get { return _ritmo; } private set { _ritmo = value; RaisePropertyChanged(); } }

		private Chiavi _chiave = Chiavi.Violino;
		public Chiavi Chiave { get { return _chiave; } private set { _chiave = value; RaisePropertyChanged(); } }

		private readonly SwitchableObservableCollection<Chord> _chords = new SwitchableObservableCollection<Chord>();
		public SwitchableObservableCollection<Chord> Chords { get { return _chords; } }

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
		/*
		public void AddToChord(Chord chord, Touch touch)
		{
			if (chord == null) return;
			var selectedChord = _chords.FirstOrDefault(cho => cho == chord);
			if (selectedChord == null) return;
			if (touch is Note) selectedChord.AddNote(touch as Note);
			else selectedChord.AddPause(touch as Pause);
		}
		*/
	}
	public enum Ritmi { qq, tq, dq }
	public enum Chiavi { Violino, Basso }
	public enum NoteBianche { @do, re, mi, fa, sol, la, si }
	public enum DurateCanoniche { Breve, Semibreve, Minima, Semiminima, Croma, Semicroma, Biscroma, Semibiscroma }
	public enum SegniSuNote { None, Accento, Trillo }
	public enum Accidenti { Bequadro, Diesis, Bemolle, DoppioDiesis, DoppioBemolle }
	public class Chord
	{
		private readonly SwitchableObservableCollection<Touch> _touches = new SwitchableObservableCollection<Touch>();
		public SwitchableObservableCollection<Touch> Touches { get { return _touches; } }
		private readonly SwitchableObservableCollection<Chord> _prevJoinedChords = new SwitchableObservableCollection<Chord>();
		public SwitchableObservableCollection<Chord> PrevJoinedChords { get { return _prevJoinedChords; } }
		private readonly SwitchableObservableCollection<Chord> _nextJoinedChords = new SwitchableObservableCollection<Chord>();
		public SwitchableObservableCollection<Chord> NextJoinedChords { get { return _nextJoinedChords; } }

		public Chord(Note note)
		{
			if (note == null) throw new ArgumentOutOfRangeException("Chord ctor wants a note");
			_touches.Add(note);
		}
		public Chord(IList<Note> notes)
		{
			if (notes == null) throw new ArgumentOutOfRangeException("Chord ctor wants some notes");
			_touches.AddRange(notes);
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
	public class Note : Touch, IComparable
	{
		private uint _ottava = 3;
		public uint Ottava { get { return _ottava; } private set { _ottava = value < 10 ? value : 10; RaisePropertyChanged(); } }
		private NoteBianche _notaBianca = NoteBianche.@do;
		public NoteBianche NotaBianca { get { return _notaBianca; } private set { _notaBianca = value; RaisePropertyChanged(); } }
		private Accidenti _accidente = Accidenti.Bequadro;
		public Accidenti Accidente { get { return _accidente; } private set { _accidente = value; RaisePropertyChanged(); } }
		private SegniSuNote _segno = SegniSuNote.None;
		public SegniSuNote Segno { get { return _segno; } private set { _segno = value; RaisePropertyChanged(); } }
		//private Touch _prevLinkedTouch = null;
		//public Touch PrevLinkedTouch { get { return _prevLinkedTouch; } private set { _prevLinkedTouch = value; RaisePropertyChanged(); } }
		//private Touch _currLinkedTouch = null;
		//public Touch CurrLinkedTouch { get { return _currLinkedTouch; } private set { _currLinkedTouch = value; RaisePropertyChanged(); } }
		//private Touch _nextLinkedTouch = null;
		//public Touch NextLinkedTouch { get { return _nextLinkedTouch; } private set { _nextLinkedTouch = value; RaisePropertyChanged(); } }

		public Note(DurateCanoniche durataCanonica, uint ottava, NoteBianche notaBianca, Accidenti accidente) : base(durataCanonica)
		{
			Ottava = ottava;
			NotaBianca = notaBianca;
			Accidente = accidente;
		}
		public Note(DurateCanoniche durataCanonica, uint ottava, NoteBianche notaBianca, Accidenti accidente, SegniSuNote segno) : this(durataCanonica, ottava, notaBianca, accidente)
		{
			Segno = segno;
		}

		public int CompareTo(object obj)
		{
			var otherNote = obj as Note;

			if (Ottava > otherNote.Ottava) return +1;
			if (Ottava < otherNote.Ottava) return -1;
			if ((int)NotaBianca > (int)otherNote.NotaBianca) return +1;
			if ((int)NotaBianca < (int)otherNote.NotaBianca) return -1;
			return 0;
		}
		//public Note(DurateCanoniche durataCanonica, uint ottava, NoteBianche notaBianca, Accidenti accidente, Touch prevLinkedTouch, Touch currLinkedTouch, Touch nextLinkedTouch) : this(durataCanonica, ottava, notaBianca, accidente)
		//{
		//	PrevLinkedTouch = prevLinkedTouch;
		//	CurrLinkedTouch = currLinkedTouch;
		//	NextLinkedTouch = nextLinkedTouch;
		//}
		//public Note(DurateCanoniche durataCanonica, uint ottava, NoteBianche notaBianca, Accidenti accidente, SegniSuNote segno, Touch prevLinkedTouch, Touch currLinkedTouch, Touch nextLinkedTouch) : this(durataCanonica, ottava, notaBianca, accidente, prevLinkedTouch, currLinkedTouch, nextLinkedTouch)
		//{
		//	Segno = segno;
		//}
	}

	public class Pause : Touch
	{
		public Pause(DurateCanoniche durataCanonica) : base(durataCanonica) { }
	}

	public abstract class Touch : ObservableData
	{
		public uint Durata64
		{
			get
			{
				switch (_durataCanonica)
				{
					case DurateCanoniche.Breve:
						return GetDurata64(128);
					case DurateCanoniche.Semibreve:
						return GetDurata64(64);
					case DurateCanoniche.Minima:
						return GetDurata64(32);
					case DurateCanoniche.Semiminima:
						return GetDurata64(16);
					case DurateCanoniche.Croma:
						return GetDurata64(8);
					case DurateCanoniche.Semicroma:
						return GetDurata64(4);
					case DurateCanoniche.Biscroma:
						return GetDurata64(2);
					case DurateCanoniche.Semibiscroma:
						return GetDurata64(1);
					default:
						return 1;
				}
			}
		}
		private uint GetDurata64(uint basicDurata)
		{
			return basicDurata + ((2 ^ PuntiDiValore - 1) / 2 ^ PuntiDiValore) * basicDurata;
		}
		private DurateCanoniche _durataCanonica = DurateCanoniche.Semibreve;
		public DurateCanoniche DurataCanonica { get { return _durataCanonica; } private set { _durataCanonica = value; RaisePropertyChanged(); } }
		private uint _puntiDiValore = 0;
		public uint PuntiDiValore { get { return _puntiDiValore; } private set { _puntiDiValore = value > 3 ? 3 : value < 0 ? 0 : value; RaisePropertyChanged(); } }

		public Touch(DurateCanoniche durataCanonica)
		{
			DurataCanonica = durataCanonica;
		}
	}
}
