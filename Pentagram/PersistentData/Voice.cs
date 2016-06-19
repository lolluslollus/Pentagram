using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Utilz;
using Utilz.Data;

namespace Pentagram.PersistentData
{
	[DataContract]
	public sealed class InstantWithTouches : ObservableData
	{
		// we cannot make this readonly because it is serialised. we only use the setter for serialising.
		private SwitchableObservableCollection<Sound> _sounds = new SwitchableObservableCollection<Sound>();
		[DataMember]
		public SwitchableObservableCollection<Sound> Sounds { get { return _sounds; } private set { _sounds = value; } }
	}
	[DataContract]
	public sealed class Voice : OpenableObservableData
	{
		private Ritmi _ritmo = All.DEFAULT_RITMO;
		[DataMember]
		public Ritmi Ritmo { get { return _ritmo; } private set { _ritmo = value; RaisePropertyChanged(); } }

		private Chiavi _chiave = All.DEFAULT_CHIAVE;
		[DataMember]
		public Chiavi Chiave { get { return _chiave; } private set { _chiave = value; RaisePropertyChanged(); } }

		// we cannot make this readonly because it is serialised. we only use the setter for serialising.
		private SwitchableObservableCollection<InstantWithTouches> _instants = new SwitchableObservableCollection<InstantWithTouches>();
		[DataMember]
		public SwitchableObservableCollection<InstantWithTouches> Instants { get { return _instants; } private set { _instants = value; } }

		public Voice(Ritmi ritmo, Chiavi chiave)
		{
			Ritmo = ritmo;
			Chiave = chiave;
		}

		public InstantWithTouches AddInstant()
		{
			var instant = new InstantWithTouches();
			_instants.Add(instant);
			return instant;
		}
		public void RemoveInstant(InstantWithTouches instant)
		{
			if (instant == null) return;
			foreach (var sound in instant.Sounds)
			{
				RemoveSoundFromInstant(sound, instant);
			}
			_instants.Remove(new InstantWithTouches());
		}
		public void AddSoundToInstant(Sound sound, InstantWithTouches instant)
		{
			if (sound == null || instant == null) return;
			var instantBeingUpdated = _instants.FirstOrDefault(tou => tou == instant);
			if (instantBeingUpdated == null) return;

			instantBeingUpdated.Sounds.Add(sound);
		}
		public void RemoveSoundFromInstant(Sound sound, InstantWithTouches instant)
		{
			if (sound == null || instant == null || !_instants.Contains(instant)) return;

			if (sound is Chord)
			{
				var chord = sound as Chord;
				var idx = _instants.IndexOf(instant);

				if (idx > 0)
				{
					var prevInstant = _instants[idx - 1];
					foreach (Chord chordInPrevInstant in prevInstant.Sounds.Where(sou => sou is Chord))
					{
						if (chordInPrevInstant.NextJoinedChord == chord) chordInPrevInstant.NextJoinedChord = null;
					}
				}
				if (idx < _instants.Count - 1)
				{
					var nextInstant = _instants[idx + 1];
					foreach (Chord chordInNextInstant in nextInstant.Sounds.Where(sou => sou is Chord))
					{
						if (chordInNextInstant.PrevJoinedChord == chord) chordInNextInstant.PrevJoinedChord = null;
					}
				}
			}

			instant.Sounds.Remove(sound);

			if (instant.Sounds.Count > 0) return;
			_instants.Remove(instant);
		}
		public void AddToneToChord(Tone tone, Chord chord, InstantWithTouches instant)
		{
			if (chord == null || tone == null || instant == null) return;
			var instantBeingUpdated = _instants.FirstOrDefault(ins => ins == instant);
			if (instantBeingUpdated == null) return;
			var chordBeingUpdated = instantBeingUpdated.Sounds.FirstOrDefault(sou => sou == chord) as Chord;
			if (chordBeingUpdated == null) return;
			if (chordBeingUpdated.Tones.Contains(tone)) return;

			chordBeingUpdated.Tones.Add(tone);
		}
		public void RemoveToneFromChord(Chord chord, Tone tone, InstantWithTouches instant)
		{
			if (chord == null || tone == null || instant == null) return;
			var instantBeingUpdated = _instants.FirstOrDefault(ins => ins == instant);
			if (instantBeingUpdated == null) return;
			var chordBeingUpdated = instantBeingUpdated.Sounds.FirstOrDefault(sou => sou == chord) as Chord;
			if (chordBeingUpdated == null) return;

			chordBeingUpdated.Tones.Remove(tone);

			if (chordBeingUpdated.Tones.Count > 0) return;
			RemoveSoundFromInstant(chordBeingUpdated, instant);
		}
		public bool TryLinkChord1ToChord2(Chord chord1, Chord chord2, InstantWithTouches instant1, InstantWithTouches instant2)
		{
			bool result = false;

			if (chord1 == null || chord2 == null || instant1 == null || instant2 == null || !instant1.Sounds.Contains(chord1) || !instant2.Sounds.Contains(chord2)) return result;
			if (chord1.Duration.DurataCanonica.CompareTo(DurateCanoniche.Croma) > 0 || chord2.Duration.DurataCanonica.CompareTo(DurateCanoniche.Croma) > 0) return result;

			int instant1Idx = _instants.IndexOf(instant1);
			int instant2Idx = _instants.IndexOf(instant2);

			if (instant1Idx + 1 == instant2Idx)
			{
				chord2.PrevJoinedChord = chord1;
				chord1.NextJoinedChord = chord2;
				result = true;
			}
			else if (instant1Idx - 1 == instant2Idx)
			{
				chord2.NextJoinedChord = chord1;
				chord1.PrevJoinedChord = chord2;
				result = true;
			}

			return result;
		}
		public void UnlinkChord1FromChord2(Chord chord1, Chord chord2, InstantWithTouches instant1, InstantWithTouches instant2)
		{
			if (chord1 == null || chord2 == null || instant1 == null || instant2 == null || !instant1.Sounds.Contains(chord1) || !instant2.Sounds.Contains(chord2)) return;
			int instant1Idx = _instants.IndexOf(instant1);
			int instant2Idx = _instants.IndexOf(instant2);

			if (instant1Idx < instant2Idx)
			{
				if (chord2.PrevJoinedChord == chord1) chord2.PrevJoinedChord = null;
				if (chord1.NextJoinedChord == chord2) chord1.NextJoinedChord = null;
			}
			else if (instant1Idx > instant2Idx)
			{
				if (chord2.NextJoinedChord == chord1) chord2.NextJoinedChord = null;
				if (chord1.PrevJoinedChord == chord2) chord1.PrevJoinedChord = null;
			}
		}
	}
}
