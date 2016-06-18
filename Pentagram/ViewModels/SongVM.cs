using Pentagram.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilz.Data;

namespace Pentagram.ViewModels
{
	public class SongVM : OpenableObservableData
	{
		private readonly Song _song = null;
		public Song Song { get { return _song; } }

		private readonly SongHeader _songHeader = null;
		public SongHeader SongHeader { get { return _songHeader; } }

		public SongVM(SongHeader songHeader)
		{
			if (songHeader == null) throw new ArgumentNullException("SongVM needs a song");
			_songHeader = songHeader;
			if (_song?.Id != songHeader.Id) _song = Song.GetCreateInstance(songHeader.Id);
		}
		protected override Task OpenMayOverrideAsync(object args = null)
		{
			return _song.OpenAsync();
		}
		protected override Task CloseMayOverrideAsync()
		{
			return _song.CloseAsync();
		}
		public Task<bool> AddVoiceAsync(Ritmi ritmo, Chiavi chiave)
		{
			var voice = new Voice(ritmo, chiave);
			return _song.AddVoiceAsync(voice);
		}
		public Task<bool> RemoveVoiceAsync(Voice voice)
		{
			return _song.RemoveVoiceAsync(voice);
		}
	}
}
