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
		

		public PentagramVM()
		{
			
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

		public void AddNote(Note note)
		{
			if (note == null) return;
		}
		public void AddNote(Note note, Chord chord)
		{
			if (note == null || chord == null) return;
		}
		public void AddPause(Pause pause)
		{
			if (pause == null) return;
		}
		public void AddPause(Pause pause, Chord chord)
		{
			if (pause == null || chord == null) return;
		}
	}
}
