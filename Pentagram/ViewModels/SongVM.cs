using Pentagram.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilz.Data;

namespace Pentagram.ViewModels
{
	public class SongVM : ObservableData
	{
		private readonly Song _song = null;
		public Song Song { get { return _song; } }

		public SongVM(Song song)
		{
			if (song == null) throw new ArgumentNullException("SongVM needs a song");
			_song = song;
		}

		public void AddVoice(Ritmi ritmo, Chiavi chiave)
		{
			var voice = new Voice(ritmo, chiave);
			_song.AddVoice(voice);
		}
		public void RemoveVoice(Voice voice)
		{
			_song.RemoveVoice(voice);
		}
	}
}
