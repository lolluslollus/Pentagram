using Pentagram.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilz.Data;

namespace Pentagram.ViewModels
{
	public class PentagramVM : OpenableObservableData
	{
		private Song _song = null;
		public Song Song { get { return _song; } }

		public PentagramVM(Song song)
		{
			if (song == null) throw new ArgumentNullException("PentagramVM needs a song");
			_song = song;
		}

		protected override async Task OpenMayOverrideAsync(object args = null)
		{
			if (_song == null) _song = new Song();
			await _song.OpenAsync(args);
		}

		protected override Task CloseMayOverrideAsync()
		{
			return _song?.CloseAsync(); // LOLLO TODO check for errors if song is null
		}

		public void AddVoice(Voice voice)
		{
			if (voice == null) return;
			_song.AddVoice(voice);
		}
		public void AddNote(Voice voice, DurateCanoniche durataCanonica, uint ottava, NoteBianche notaBianca, Accidenti accidente)
		{
			if (_song.Voices.Count == 0 || voice == null) return;
			var note = new Note(durataCanonica, ottava, notaBianca, accidente);
			voice.AddChord(new Chord(note));
		}
		public void AddNoteToChord(Voice voice, DurateCanoniche durataCanonica, uint ottava, NoteBianche notaBianca, Accidenti accidente, Chord chord)
		{
			if (_song.Voices.Count == 0 || voice == null || chord == null) return;
			var note = new Note(durataCanonica, ottava, notaBianca, accidente);
			var chordInVoice = voice.Chords.FirstOrDefault(cho => cho == chord);
			if (chordInVoice == null) return;
			chordInVoice.AddNote(note);
		}
		public void AddPause(Voice voice, Pause pause)
		{
			if (_song.Voices.Count == 0 || pause == null) return;
			var ppause = new Pause(pause.DurataCanonica);
			voice.AddChord(new Chord(pause));
		}
		public void ResetChord(Voice voice, Chord chord)
		{
			if (_song.Voices.Count == 0 || voice == null || chord == null) return;
			chord.Reset();
		}
	}
}
