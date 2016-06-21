using Pentagram.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilz;
using Utilz.Data;

namespace Pentagram.ViewModels
{
	public class VoicesVM : ObservableData
	{
		private static Tone _defaultTone0 = new Tone(3, NoteBianche.@do, Accidenti.Nil);
		private static Tone _defaultTone1 = new Tone(3, NoteBianche.mi, Accidenti.Diesis);
		private static Tone _defaultTone2 = new Tone(3, NoteBianche.sol, Accidenti.Bemolle);
		private static Tone _defaultTone3 = new Tone(4, NoteBianche.@do, Accidenti.Bequadro);

		private static Tone _defaultTone0b = new Tone(2, NoteBianche.@do, Accidenti.Bequadro);
		private static Tone _defaultTone1b = new Tone(2, NoteBianche.mi, Accidenti.Diesis);
		private static Tone _defaultTone2b = new Tone(2, NoteBianche.sol, Accidenti.Bemolle);
		private static Tone _defaultTone3b = new Tone(3, NoteBianche.@do, Accidenti.Nil);

		private static Chord _defaultChord0 = new Chord(new Duration(DurateCanoniche.Semibiscroma, PuntiDiValore.Three), SegniSuNote.Nil, _defaultTone0, _defaultTone1, _defaultTone2, _defaultTone3);
		private static Chord _defaultChord1 = new Chord(new Duration(DurateCanoniche.Croma), SegniSuNote.Nil, _defaultTone0, _defaultTone2);
		private static Chord _defaultChord2 = new Chord(new Duration(DurateCanoniche.Semibiscroma), SegniSuNote.Nil, _defaultTone0, _defaultTone1, _defaultTone2, _defaultTone3);
		private static Chord _defaultChord3 = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, _defaultTone0, _defaultTone2);
		private static Chord _defaultChord4 = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, _defaultTone0, _defaultTone1, _defaultTone2, _defaultTone3);
		private static Chord _defaultChord5 = new Chord(new Duration(DurateCanoniche.Biscroma), SegniSuNote.Nil, _defaultTone0, _defaultTone2);

		private static Chord _defaultChord0b = new Chord(new Duration(DurateCanoniche.Semibiscroma), SegniSuNote.Nil, true, _defaultTone0, _defaultTone1, _defaultTone2, _defaultTone3);
		private static Chord _defaultChord1b = new Chord(new Duration(DurateCanoniche.Croma), SegniSuNote.Nil, true, _defaultTone0, _defaultTone2);
		private static Chord _defaultChord2b = new Chord(new Duration(DurateCanoniche.Semibiscroma), SegniSuNote.Nil, true, _defaultTone0, _defaultTone1, _defaultTone2, _defaultTone3);
		private static Chord _defaultChord3b = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, true, _defaultTone0, _defaultTone2);
		private static Chord _defaultChord4b = new Chord(new Duration(DurateCanoniche.Semicroma), SegniSuNote.Nil, true, _defaultTone0, _defaultTone1, _defaultTone2, _defaultTone3);
		private static Chord _defaultChord5b = new Chord(new Duration(DurateCanoniche.Biscroma), SegniSuNote.Nil, true, _defaultTone0, _defaultTone2);

		private static Chord _defaultChord6 = new Chord(new Duration(DurateCanoniche.Semicroma, PuntiDiValore.Two), SegniSuNote.Nil, _defaultTone0, _defaultTone2);
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

		private readonly SwitchableObservableCollection<Voice> _voices = null;
		public SwitchableObservableCollection<Voice> Voices { get { return _voices; } }

		public VoicesVM(SwitchableObservableCollection<Voice> voices)
		{
			if (voices == null) throw new ArgumentNullException("VoicesVM needs voices");
			_voices = voices;
			if (voices.Count < 1)
			{
				AddTestVoice0();
				AddTestVoice1();
			}
		}

		public void AddNote(Voice voice, Chiavi chiave, Ritmi ritmo, DurateCanoniche durataCanonica, PuntiDiValore puntiDiValore, SegniSuNote segniSuNote, bool isChromaFlagsBelow, uint ottava, NoteBianche notaBianca, Accidenti accidente)
		{
			if (voice == null) return;
			var bat = voice.AddBattuta(chiave, ritmo);
			var ins = voice.AddInstant(bat);
			voice.AddSoundToInstant(new Chord(new Duration(durataCanonica, puntiDiValore), segniSuNote, isChromaFlagsBelow, new Tone(ottava, notaBianca, accidente)), ins, bat);
		}
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

		private void AddTestVoice0()
		{
			var voice = new Voice();
			Battuta bat0 = null;
			Battuta bat1 = null;
			InstantWithTouches ins0 = null;
			InstantWithTouches ins1 = null;
			InstantWithTouches ins2 = null;

			bat0 = voice.AddBattuta(Chiavi.Violino, Ritmi.qq);
			bat1 = voice.AddBattuta(Chiavi.Basso, Ritmi.tq);

			ins0 = voice.AddInstant(bat0);
			ins0.SoundsOrTabs.Add(new Tab(TabSymbols.Chiave));
			ins0 = voice.AddInstant(bat0);
			ins0.SoundsOrTabs.Add(new Tab(TabSymbols.Ritmo));

			ins0 = voice.AddInstant(bat0);
			voice.AddSoundToInstant(_defaultChord6, ins0, bat0);
			voice.AddSoundToInstant(_defaultChord6b, ins0, bat0);
			ins1 = voice.AddInstant(bat0);
			voice.AddSoundToInstant(_defaultChord7, ins1, bat0);
			voice.AddSoundToInstant(_defaultChord7b, ins1, bat0);
			ins2 = voice.AddInstant(bat0);
			voice.AddSoundToInstant(_defaultChord8, ins2, bat0);
			voice.AddSoundToInstant(_defaultChord8b, ins2, bat0);

			voice.TryLinkChord1ToChord2(_defaultChord6, _defaultChord7, ins0, ins1, bat0);
			voice.TryLinkChord1ToChord2(_defaultChord7, _defaultChord8, ins1, ins2, bat0);
			voice.TryLinkChord1ToChord2(_defaultChord6b, _defaultChord7b, ins0, ins1, bat0);
			voice.TryLinkChord1ToChord2(_defaultChord7b, _defaultChord8b, ins1, ins2, bat0);
			voice.AddInstantWithTab(bat0);

			//ins0 = voice.AddInstant(bat0);
			//voice.AddSoundToInstant(_defaultChord9, ins0, bat0);
			//voice.AddSoundToInstant(_defaultChord9b, ins0, bat0);
			//ins1 = voice.AddInstant(bat0);
			//voice.AddSoundToInstant(_defaultChord10, ins1, bat0);
			//voice.AddSoundToInstant(_defaultChord10b, ins1, bat0);
			//ins2 = voice.AddInstant(bat0);
			//voice.AddSoundToInstant(_defaultChord11, ins2, bat0);
			//voice.AddSoundToInstant(_defaultChord11b, ins2, bat0);

			//voice.TryLinkChord1ToChord2(_defaultChord9, _defaultChord10, ins0, ins1, bat0);
			//voice.TryLinkChord1ToChord2(_defaultChord10, _defaultChord11, ins1, ins2, bat0);
			//voice.TryLinkChord1ToChord2(_defaultChord9b, _defaultChord10b, ins0, ins1, bat0);
			//voice.TryLinkChord1ToChord2(_defaultChord10b, _defaultChord11b, ins1, ins2, bat0);
			//voice.AddInstantWithTab(bat0);

			//ins0 = voice.AddInstant(bat0);
			//voice.AddSoundToInstant(_defaultChord12, ins0, bat0);
			//voice.AddSoundToInstant(_defaultChord12b, ins0, bat0);
			//ins1 = voice.AddInstant(bat0);
			//voice.AddSoundToInstant(_defaultChord13, ins1, bat0);
			//voice.AddSoundToInstant(_defaultChord13b, ins1, bat0);
			//ins2 = voice.AddInstant(bat0);
			//voice.AddSoundToInstant(_defaultChord14, ins2, bat0);
			//voice.AddSoundToInstant(_defaultChord14b, ins2, bat0);

			//voice.TryLinkChord1ToChord2(_defaultChord12, _defaultChord13, ins0, ins1, bat0);
			//voice.TryLinkChord1ToChord2(_defaultChord13, _defaultChord14, ins1, ins2, bat0);
			//voice.TryLinkChord1ToChord2(_defaultChord12b, _defaultChord13b, ins0, ins1, bat0);
			//voice.TryLinkChord1ToChord2(_defaultChord13b, _defaultChord14b, ins1, ins2, bat0);
			//voice.AddInstantWithTab(bat0);

			ins0 = voice.AddInstant(bat1);
			voice.AddSoundToInstant(_defaultChord15, ins0, bat1);
			voice.AddSoundToInstant(_defaultChord15b, ins0, bat1);
			ins1 = voice.AddInstant(bat1);
			voice.AddSoundToInstant(_defaultChord16, ins1, bat1);
			voice.AddSoundToInstant(_defaultChord16b, ins1, bat1);
			ins2 = voice.AddInstant(bat1);
			voice.AddSoundToInstant(_defaultChord17, ins2, bat1);
			voice.AddSoundToInstant(_defaultChord17b, ins2, bat1);

			voice.TryLinkChord1ToChord2(_defaultChord15, _defaultChord16, ins0, ins1, bat1);
			voice.TryLinkChord1ToChord2(_defaultChord16, _defaultChord17, ins1, ins2, bat1);
			voice.TryLinkChord1ToChord2(_defaultChord15b, _defaultChord16b, ins0, ins1, bat1);
			voice.TryLinkChord1ToChord2(_defaultChord16b, _defaultChord17b, ins1, ins2, bat1);
			voice.AddInstantWithTab(bat1);

			//ins0 = voice.AddInstant(bat1);
			//voice.AddSoundToInstant(_defaultChord18, ins0, bat1);
			//voice.AddSoundToInstant(_defaultChord18b, ins0, bat1);
			//ins1 = voice.AddInstant(bat1);
			//voice.AddSoundToInstant(_defaultChord19, ins1, bat1);
			//voice.AddSoundToInstant(_defaultChord19b, ins1, bat1);
			//ins2 = voice.AddInstant(bat1);
			//voice.AddSoundToInstant(_defaultChord20, ins2, bat1);
			//voice.AddSoundToInstant(_defaultChord20b, ins2, bat1);

			//voice.TryLinkChord1ToChord2(_defaultChord18, _defaultChord19, ins0, ins1, bat1);
			//voice.TryLinkChord1ToChord2(_defaultChord19, _defaultChord20, ins1, ins2, bat1);
			//voice.TryLinkChord1ToChord2(_defaultChord18b, _defaultChord19b, ins0, ins1, bat1);
			//voice.TryLinkChord1ToChord2(_defaultChord19b, _defaultChord20b, ins1, ins2, bat1);
			//voice.AddInstantWithTab(bat1);

			//ins0 = voice.AddInstant(bat1);
			//voice.AddSoundToInstant(_defaultChord21, ins0, bat1);
			//voice.AddSoundToInstant(_defaultChord21b, ins0, bat1);
			//ins1 = voice.AddInstant(bat1);
			//voice.AddSoundToInstant(_defaultChord22, ins1, bat1);
			//voice.AddSoundToInstant(_defaultChord22b, ins1, bat1);
			//ins2 = voice.AddInstant(bat1);
			//voice.AddSoundToInstant(_defaultChord23, ins2, bat1);
			//voice.AddSoundToInstant(_defaultChord23b, ins2, bat1);

			//voice.TryLinkChord1ToChord2(_defaultChord21, _defaultChord22, ins0, ins1, bat1);
			//voice.TryLinkChord1ToChord2(_defaultChord22, _defaultChord23, ins1, ins2, bat1);
			//voice.TryLinkChord1ToChord2(_defaultChord21b, _defaultChord22b, ins0, ins1, bat1);
			//voice.TryLinkChord1ToChord2(_defaultChord22b, _defaultChord23b, ins1, ins2, bat1);
			//voice.AddInstantWithTab(bat1);

			_voices.Add(voice);
		}

		private void AddTestVoice1()
		{
			var voice = new Voice();
			Battuta bat0 = null;
			Battuta bat1 = null;
			InstantWithTouches ins0 = null;
			InstantWithTouches ins1 = null;
			InstantWithTouches ins2 = null;

			bat0 = voice.AddBattuta(Chiavi.Basso, Ritmi.dq);
			bat1 = voice.AddBattuta(Chiavi.Violino, Ritmi.tq);

			ins0 = voice.AddInstant(bat0);
			ins0.SoundsOrTabs.Add(new Tab(TabSymbols.Chiave));
			ins0 = voice.AddInstant(bat0);
			ins0.SoundsOrTabs.Add(new Tab(TabSymbols.Ritmo));

			ins0 = voice.AddInstant(bat0);
			voice.AddSoundToInstant(_defaultChord0, ins0, bat0);
			ins1 = voice.AddInstant(bat0);
			voice.AddSoundToInstant(_defaultChord1, ins1, bat0);
			voice.AddSoundToInstant(_defaultChord1b, ins1, bat0);
			ins2 = voice.AddInstant(bat0);
			voice.AddSoundToInstant(_defaultChord2, ins2, bat0);
			voice.AddSoundToInstant(_defaultChord2b, ins2, bat0);

			voice.TryLinkChord1ToChord2(_defaultChord0, _defaultChord1, ins0, ins1, bat0);
			voice.TryLinkChord1ToChord2(_defaultChord1b, _defaultChord2b, ins1, ins2, bat0);
			voice.AddInstantWithTab(bat0);

			ins0 = voice.AddInstant(bat1);
			voice.AddSoundToInstant(_defaultChord3, ins0, bat1);
			voice.AddSoundToInstant(_defaultChord3b, ins0, bat1);
			voice.AddInstantWithTab(bat1);

			ins1 = voice.AddInstant(bat1);
			voice.AddSoundToInstant(_defaultChord4, ins1, bat1);
			voice.AddSoundToInstant(_defaultChord5b, ins1, bat1);
			ins2 = voice.AddInstant(bat1);
			voice.AddSoundToInstant(_defaultChord5, ins2, bat1);
			voice.AddSoundToInstant(_defaultChord4b, ins2, bat1);
			voice.AddInstantWithTab(bat1);

			_voices.Add(voice);
		}
	}
}
