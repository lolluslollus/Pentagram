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
		private readonly Voice _voice = null;
		public Voice Voice { get { return _voice; } }

		public VoiceVM(Voice voice)
		{
			if (voice == null) throw new ArgumentNullException("VoiceVM needs a voice");
			_voice = voice;
		}

		public void AddNoteAndChord(DurateCanoniche durataCanonica, uint ottava, NoteBianche notaBianca, Accidenti accidente)
		{
			var note = new Note(durataCanonica, ottava, notaBianca, accidente);
			_voice.AddChord(new Chord(note));
		}
		public void AddNoteToChord(DurateCanoniche durataCanonica, uint ottava, NoteBianche notaBianca, Accidenti accidente, Chord chord)
		{
			if (chord == null) return;
			var note = new Note(durataCanonica, ottava, notaBianca, accidente);
			var chordInVoice = _voice.Chords.FirstOrDefault(cho => cho == chord);
			if (chordInVoice == null) return;
			chordInVoice.AddNote(note);
		}
		public void AddPause(Pause pause)
		{
			if (pause == null) return;
			_voice.AddChord(new Chord(pause));
		}
		public void ResetChord(Chord chord)
		{
			chord?.Reset();
		}
	}
}
