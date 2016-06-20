using Pentagram.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilz.Data;

namespace Pentagram.ViewModels
{
	public class VoiceVM : ObservableData
	{
		private static Tone _defaultTone0 = new Tone(4, NoteBianche.@do, Accidenti.Bequadro);
		private static Tone _defaultTone1 = new Tone(4, NoteBianche.mi, Accidenti.Diesis);
		private static Tone _defaultTone2 = new Tone(4, NoteBianche.sol, Accidenti.Bemolle);
		private static Tone _defaultTone3 = new Tone(5, NoteBianche.@do, Accidenti.Bequadro);

		private static Tone _defaultTone0b = new Tone(3, NoteBianche.@do, Accidenti.Bequadro);
		private static Tone _defaultTone1b = new Tone(3, NoteBianche.mi, Accidenti.Diesis);
		private static Tone _defaultTone2b = new Tone(3, NoteBianche.sol, Accidenti.Bemolle);
		private static Tone _defaultTone3b = new Tone(4, NoteBianche.@do, Accidenti.Bequadro);

		private static Chord _defaultChord0 = new Chord(new Duration(DurateCanoniche.Semibiscroma), SegniSuNote.Nil, _defaultTone0, _defaultTone1, _defaultTone2, _defaultTone3);
		private static Chord _defaultChord1 = new Chord(new Duration(DurateCanoniche.Croma), SegniSuNote.Nil, _defaultTone0, _defaultTone2);
		private static Chord _defaultChord2 = new Chord(new Duration(DurateCanoniche.Semibiscroma), SegniSuNote.Nil, _defaultTone0, _defaultTone1, _defaultTone2, _defaultTone3);
		private static Chord _defaultChord3 = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, _defaultTone0, _defaultTone2);
		private static Chord _defaultChord4 = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, _defaultTone0, _defaultTone1, _defaultTone2, _defaultTone3);
		private static Chord _defaultChord5 = new Chord(new Duration(DurateCanoniche.Biscroma), SegniSuNote.Nil, _defaultTone0, _defaultTone2);

		private static Chord _defaultChord6 = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, _defaultTone0, _defaultTone2);
		private static Chord _defaultChord7 = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, _defaultTone1, _defaultTone2);
		private static Chord _defaultChord8 = new Chord(new Duration(DurateCanoniche.Biscroma), SegniSuNote.Nil, _defaultTone2, _defaultTone3);

		private static Chord _defaultChord9 = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, _defaultTone0, _defaultTone2);
		private static Chord _defaultChord10 = new Chord(new Duration(DurateCanoniche.Biscroma), SegniSuNote.Nil, _defaultTone1, _defaultTone2);
		private static Chord _defaultChord11 = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, _defaultTone2, _defaultTone3);

		private static Chord _defaultChord12 = new Chord(new Duration(DurateCanoniche.Biscroma), SegniSuNote.Nil, _defaultTone0, _defaultTone2);
		private static Chord _defaultChord13 = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, _defaultTone1, _defaultTone2);
		private static Chord _defaultChord14 = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, _defaultTone2, _defaultTone3);

		private static Chord _defaultChord15 = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, _defaultTone0, _defaultTone2);
		private static Chord _defaultChord16 = new Chord(new Duration(DurateCanoniche.Croma), SegniSuNote.Nil, _defaultTone1, _defaultTone2);
		private static Chord _defaultChord17 = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, _defaultTone2, _defaultTone3);

		private static Chord _defaultChord18 = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, _defaultTone0, _defaultTone2);
		private static Chord _defaultChord19 = new Chord(new Duration(DurateCanoniche.Biscroma), SegniSuNote.Nil, _defaultTone1, _defaultTone2);
		private static Chord _defaultChord20 = new Chord(new Duration(DurateCanoniche.Croma), SegniSuNote.Nil, _defaultTone2, _defaultTone3);

		private static Chord _defaultChord21 = new Chord(new Duration(DurateCanoniche.Croma), SegniSuNote.Nil, _defaultTone0, _defaultTone2);
		private static Chord _defaultChord22 = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, _defaultTone1, _defaultTone2);
		private static Chord _defaultChord23 = new Chord(new Duration(DurateCanoniche.Biscroma), SegniSuNote.Nil, _defaultTone2, _defaultTone3);

		private static Chord _defaultChord6b = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, true, _defaultTone0b, _defaultTone2b);
		private static Chord _defaultChord7b = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, true, _defaultTone1b, _defaultTone2b);
		private static Chord _defaultChord8b = new Chord(new Duration(DurateCanoniche.Biscroma), SegniSuNote.Nil, true, _defaultTone2b, _defaultTone3b);

		private static Chord _defaultChord9b = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, true, _defaultTone0b, _defaultTone2b);
		private static Chord _defaultChord10b = new Chord(new Duration(DurateCanoniche.Biscroma), SegniSuNote.Nil, true, _defaultTone1b, _defaultTone2b);
		private static Chord _defaultChord11b = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, true, _defaultTone2b, _defaultTone3b);

		private static Chord _defaultChord12b = new Chord(new Duration(DurateCanoniche.Biscroma), SegniSuNote.Nil, true, _defaultTone0b, _defaultTone2b);
		private static Chord _defaultChord13b = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, true, _defaultTone1b, _defaultTone2b);
		private static Chord _defaultChord14b = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, true, _defaultTone2b, _defaultTone3b);

		private static Chord _defaultChord15b = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, true, _defaultTone0b, _defaultTone2b);
		private static Chord _defaultChord16b = new Chord(new Duration(DurateCanoniche.Croma), SegniSuNote.Nil, true, _defaultTone1b, _defaultTone2b);
		private static Chord _defaultChord17b = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, true, _defaultTone2b, _defaultTone3b);

		private static Chord _defaultChord18b = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, true, _defaultTone0b, _defaultTone2b);
		private static Chord _defaultChord19b = new Chord(new Duration(DurateCanoniche.Biscroma), SegniSuNote.Nil, true, _defaultTone1b, _defaultTone2b);
		private static Chord _defaultChord20b = new Chord(new Duration(DurateCanoniche.Croma), SegniSuNote.Nil, true, _defaultTone2b, _defaultTone3b);

		private static Chord _defaultChord21b = new Chord(new Duration(DurateCanoniche.Croma), SegniSuNote.Nil, true, _defaultTone0b, _defaultTone2b);
		private static Chord _defaultChord22b = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, true, _defaultTone1b, _defaultTone2b);
		private static Chord _defaultChord23b = new Chord(new Duration(DurateCanoniche.Biscroma), SegniSuNote.Nil, true, _defaultTone2b, _defaultTone3b);

		private readonly Voice _voice = null;
		public Voice Voice { get { return _voice; } }

		public VoiceVM(Voice voice)
		{
			if (voice == null) throw new ArgumentNullException("VoiceVM needs a voice");
			_voice = voice;
			if (voice.Instants.Count < 1)
			{
				InstantWithTouches ins0 = null;
				InstantWithTouches ins1 = null;
				InstantWithTouches ins2 = null;

				//voice.AddInstant();
				//ins0 = voice.Instants[0];
				//voice.AddSoundToInstant(_defaultChord0, ins0);

				//voice.AddInstant();
				//ins0 = voice.Instants[1];
				//voice.AddSoundToInstant(_defaultChord1, ins0);

				//voice.AddInstant();
				//ins0 = voice.Instants[2];
				//voice.AddSoundToInstant(_defaultChord2, ins0);

				//voice.AddInstant();
				//ins1 = voice.Instants[3];
				//voice.AddSoundToInstant(_defaultChord3, ins1);

				//voice.LinkChord1ToChord2(_defaultChord2, _defaultChord3, ins0, ins1);

				//voice.AddInstant();
				//ins0 = voice.Instants[4];
				//voice.AddSoundToInstant(_defaultChord4, ins0);

				//voice.AddInstant();
				//ins1 = voice.Instants[5];
				//voice.AddSoundToInstant(_defaultChord5, ins1);

				//voice.LinkChord1ToChord2(_defaultChord4, _defaultChord5, ins0, ins1);

				ins0 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord6, ins0);
				voice.AddSoundToInstant(_defaultChord6b, ins0);
				ins1 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord7, ins1);
				voice.AddSoundToInstant(_defaultChord7b, ins1);
				ins2 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord8, ins2);
				voice.AddSoundToInstant(_defaultChord8b, ins2);

				voice.TryLinkChord1ToChord2(_defaultChord6, _defaultChord7, ins0, ins1);
				voice.TryLinkChord1ToChord2(_defaultChord7, _defaultChord8, ins1, ins2);
				voice.TryLinkChord1ToChord2(_defaultChord6b, _defaultChord7b, ins0, ins1);
				voice.TryLinkChord1ToChord2(_defaultChord7b, _defaultChord8b, ins1, ins2);

				ins0 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord9, ins0);
				voice.AddSoundToInstant(_defaultChord9b, ins0);
				ins1 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord10, ins1);
				voice.AddSoundToInstant(_defaultChord10b, ins1);
				ins2 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord11, ins2);
				voice.AddSoundToInstant(_defaultChord11b, ins2);

				voice.TryLinkChord1ToChord2(_defaultChord9, _defaultChord10, ins0, ins1);
				voice.TryLinkChord1ToChord2(_defaultChord10, _defaultChord11, ins1, ins2);
				voice.TryLinkChord1ToChord2(_defaultChord9b, _defaultChord10b, ins0, ins1);
				voice.TryLinkChord1ToChord2(_defaultChord10b, _defaultChord11b, ins1, ins2);

				ins0 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord12, ins0);
				voice.AddSoundToInstant(_defaultChord12b, ins0);
				ins1 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord13, ins1);
				voice.AddSoundToInstant(_defaultChord13b, ins1);
				ins2 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord14, ins2);
				voice.AddSoundToInstant(_defaultChord14b, ins2);

				voice.TryLinkChord1ToChord2(_defaultChord12, _defaultChord13, ins0, ins1);
				voice.TryLinkChord1ToChord2(_defaultChord13, _defaultChord14, ins1, ins2);
				voice.TryLinkChord1ToChord2(_defaultChord12b, _defaultChord13b, ins0, ins1);
				voice.TryLinkChord1ToChord2(_defaultChord13b, _defaultChord14b, ins1, ins2);

				ins0 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord15, ins0);
				voice.AddSoundToInstant(_defaultChord15b, ins0);
				ins1 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord16, ins1);
				voice.AddSoundToInstant(_defaultChord16b, ins1);
				ins2 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord17, ins2);
				voice.AddSoundToInstant(_defaultChord17b, ins2);

				voice.TryLinkChord1ToChord2(_defaultChord15, _defaultChord16, ins0, ins1);
				voice.TryLinkChord1ToChord2(_defaultChord16, _defaultChord17, ins1, ins2);
				voice.TryLinkChord1ToChord2(_defaultChord15b, _defaultChord16b, ins0, ins1);
				voice.TryLinkChord1ToChord2(_defaultChord16b, _defaultChord17b, ins1, ins2);

				ins0 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord18, ins0);
				voice.AddSoundToInstant(_defaultChord18b, ins0);
				ins1 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord19, ins1);
				voice.AddSoundToInstant(_defaultChord19b, ins1);
				ins2 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord20, ins2);
				voice.AddSoundToInstant(_defaultChord20b, ins2);

				voice.TryLinkChord1ToChord2(_defaultChord18, _defaultChord19, ins0, ins1);
				voice.TryLinkChord1ToChord2(_defaultChord19, _defaultChord20, ins1, ins2);
				voice.TryLinkChord1ToChord2(_defaultChord18b, _defaultChord19b, ins0, ins1);
				voice.TryLinkChord1ToChord2(_defaultChord19b, _defaultChord20b, ins1, ins2);

				ins0 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord21, ins0);
				voice.AddSoundToInstant(_defaultChord21b, ins0);
				ins1 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord22, ins1);
				voice.AddSoundToInstant(_defaultChord22b, ins1);
				ins2 = voice.AddInstant();
				voice.AddSoundToInstant(_defaultChord23, ins2);
				voice.AddSoundToInstant(_defaultChord23b, ins2);

				voice.TryLinkChord1ToChord2(_defaultChord21, _defaultChord22, ins0, ins1);
				voice.TryLinkChord1ToChord2(_defaultChord22, _defaultChord23, ins1, ins2);
				voice.TryLinkChord1ToChord2(_defaultChord21b, _defaultChord22b, ins0, ins1);
				voice.TryLinkChord1ToChord2(_defaultChord22b, _defaultChord23b, ins1, ins2);
			}
		}

		//public void AddNote(DurateCanoniche durataCanonica, uint ottava, NoteBianche notaBianca, Accidenti accidente)
		//{
		//	var note = new Note(durataCanonica, ottava, notaBianca, accidente);
		//	_voice.AddSoundToInstant(new Chord(note));
		//}
		//public void AddNoteToChord(DurateCanoniche durataCanonica, uint ottava, NoteBianche notaBianca, Accidenti accidente, Chord chord)
		//{
		//	if (chord == null) return;
		//	var note = new Note(durataCanonica, ottava, notaBianca, accidente);
		//	var chordInVoice = _voice.Chords.FirstOrDefault(cho => cho == chord);
		//	if (chordInVoice == null) return;
		//	_voice.AddToneToChord(chordInVoice, note);
		//}
		//public void AddPause(Pause pause)
		//{
		//	if (pause == null) return;
		//	_voice.AddSoundToInstant(new Chord(pause));
		//}
		//public void RemoveChord(Chord chord)
		//{
		//	_voice.RemoveSoundFromInstant(chord);
		//}
	}
}
