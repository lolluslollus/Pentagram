using Pentagram.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilz.Data;

namespace Pentagram.ViewModels
{
	public class AllVM : ObservableData
	{
		private readonly All _all = null;
		public All All { get { return _all; } }

		public AllVM()
		{
			_all = App.All;
		}

		public void AddSong(string name)
		{
			_all.AddSong(new Song(name));
		}
		public void RemoveSong(Song song)
		{
			_all.RemoveSong(song);
		}
		public void SetCurrentSong(Song song)
		{
			_all.SetCurrentSong(song);
		}
	}
}
