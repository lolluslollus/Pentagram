using Pentagram.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilz.Data;

namespace Pentagram.ViewModels
{
	public class AllVM : OpenableObservableData
	{
		private readonly All _all = null;
		public All All { get { return _all; } }

		public AllVM()
		{
			_all = All.GetCreateInstance();
		}
		protected override async Task OpenMayOverrideAsync(object args = null)
		{
			await _all.OpenAsync();
		}
		public Task<bool> AddSongAsync(string name)
		{
			return _all.AddSongAsync(new SongHeader(name, true));
		}
		public Task<bool> RemoveSongAsync(SongHeader songHeader)
		{
			return _all.RemoveSongAsync(songHeader);
		}
		public async Task<bool> TrySetCurrentSongAsync(SongHeader songHeader)
		{
			if (songHeader == null) return false;
			return await _all.TrySetCurrentSongAsync(songHeader).ConfigureAwait(false);
		}
	}
}
