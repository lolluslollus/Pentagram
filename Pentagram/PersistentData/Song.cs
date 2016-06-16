using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilz;
using Utilz.Data;

namespace Pentagram.PersistentData
{
	public class Song : OpenableObservableData
	{
		private readonly SwitchableObservableCollection<Voice> _voices = new SwitchableObservableCollection<Voice>();
		public SwitchableObservableCollection<Voice> Voices { get { return _voices; } }
		public void AddVoice(Voice voice)
		{
			_voices.Add(voice ?? new Voice(Ritmi.qq, Chiavi.Violino));
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

		public Chord(IList<Note> notes)
		{
			if (notes == null || notes.Count < 1) throw new ArgumentOutOfRangeException("Chord ctor wants some notes");
			AddNotes(notes);
		}
		public Chord(Pause pause)
		{
			if (pause == null) throw new ArgumentOutOfRangeException("Chord ctor wants a pause");
			AddPause(pause);
		}

		public void AddNotes(IList<Note> notes)
		{
			if (notes == null) return;
			_touches.AddRange(notes);
		}
		public void AddNote(Note note)
		{
			if (note == null) return;
			_touches.Add(note);
		}
		public void RemoveNote(Note note)
		{
			if (note == null) return;
			_touches.Remove(note);
		}
		public void AddPause(Pause pause)
		{
			if (pause == null) return;
			_touches.Clear();
			_touches.Add(pause);
		}
		public void RemovePause(Pause pause)
		{
			if (pause == null) return;
			_touches.Remove(pause);
		}
	}
	public class Note : Touch
	{
		private uint _ottava = 3;
		public uint Ottava { get { return _ottava; } private set { _ottava = value < 10 ? value : 10; RaisePropertyChanged(); } }
		private NoteBianche _notaBianca = NoteBianche.@do;
		public NoteBianche NotaBianca { get { return _notaBianca; } private set { _notaBianca = value; RaisePropertyChanged(); } }
		private Accidenti _accidente = Accidenti.Bequadro;
		public Accidenti Accidente { get { return _accidente; } private set { _accidente = value; RaisePropertyChanged(); } }
		private SegniSuNote _segno = SegniSuNote.None;
		public SegniSuNote Segno { get { return _segno; } private set { _segno = value; RaisePropertyChanged(); } }
		private Touch _prevLinkedTouch = null;
		public Touch PrevLinkedTouch { get { return _prevLinkedTouch; } private set { _prevLinkedTouch = value; RaisePropertyChanged(); } }
		private Touch _currLinkedTouch = null;
		public Touch CurrLinkedTouch { get { return _currLinkedTouch; } private set { _currLinkedTouch = value; RaisePropertyChanged(); } }
		private Touch _nextLinkedTouch = null;
		public Touch NextLinkedTouch { get { return _nextLinkedTouch; } private set { _nextLinkedTouch = value; RaisePropertyChanged(); } }


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
		public Note(DurateCanoniche durataCanonica, uint ottava, NoteBianche notaBianca, Accidenti accidente, Touch prevLinkedTouch, Touch currLinkedTouch, Touch nextLinkedTouch) : this(durataCanonica, ottava, notaBianca, accidente)
		{
			PrevLinkedTouch = prevLinkedTouch;
			CurrLinkedTouch = currLinkedTouch;
			NextLinkedTouch = nextLinkedTouch;
		}
		public Note(DurateCanoniche durataCanonica, uint ottava, NoteBianche notaBianca, Accidenti accidente, SegniSuNote segno, Touch prevLinkedTouch, Touch currLinkedTouch, Touch nextLinkedTouch) : this(durataCanonica, ottava, notaBianca, accidente, prevLinkedTouch, currLinkedTouch, nextLinkedTouch)
		{
			Segno = segno;
		}
	}

	public class Pause : Touch
	{
		public Pause(DurateCanoniche durataCanonica) : base(durataCanonica) { }
	}

	public class Touch : ObservableData
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
