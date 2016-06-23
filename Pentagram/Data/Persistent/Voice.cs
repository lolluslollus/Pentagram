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
	public sealed class Voice : OpenableObservableData
	{
		// we cannot make this readonly because it is serialised. we only use the setter for serialising.
		private SwitchableObservableCollection<Battuta> _battute = new SwitchableObservableCollection<Battuta>();
		[DataMember]
		public SwitchableObservableCollection<Battuta> Battute { get { return _battute; } private set { _battute = value; } }

		public Voice() { }

		public Battuta AddBattuta(Chiavi chiave, Misura misura)
		{
			var battuta = new Battuta(chiave, misura);
			_battute.Add(battuta);
			return battuta;
		}
		public void RemoveBattuta(Battuta battuta)
		{
			if (battuta == null) return;
			_battute.Remove(battuta);
		}
		public InstantWithTouches AddInstant(Battuta battuta)
		{
			if (battuta == null || !_battute.Contains(battuta)) return null;
			var instant = new InstantWithTouches();
			battuta.Instants.Add(instant);
			return instant;
		}
		public InstantWithTouches AddInstantWithTab(Battuta battuta)
		{
			if (battuta == null || !_battute.Contains(battuta)) return null;
			var instant = new InstantWithTouches();
			instant.SoundsOrTabs.Add(new Tab());
			battuta.Instants.Add(instant);
			return instant;
		}
		public void RemoveInstant(InstantWithTouches instant, Battuta battuta)
		{
			if (instant == null || battuta == null) return;
			// do we need the following?
			//if (!battuta.Instants.Contains(instant)) return;

			//foreach (var sound in instant.Sounds)
			//{
			//	RemoveSoundFromInstant(sound, instant);
			//}
			battuta.Instants.Remove(instant);
		}
		public void AddSoundToInstant(Sound sound, InstantWithTouches instant, Battuta battuta)
		{
			if (sound == null || instant == null || battuta == null) return;
			var battutaBeingUpdated = _battute.FirstOrDefault(bat => bat == battuta);
			if (battutaBeingUpdated == null) return;
			var instantBeingUpdated = battutaBeingUpdated.Instants.FirstOrDefault(ins => ins == instant);
			if (instantBeingUpdated == null) return;

			instantBeingUpdated.SoundsOrTabs.Add(sound);
		}
		public void RemoveSoundFromInstant(Sound sound, InstantWithTouches instant, Battuta battuta)
		{
			if (sound == null || instant == null || battuta == null) return;
			if (!battuta.Instants.Contains(instant) || !instant.SoundsOrTabs.Contains(sound)) return;

			if (sound is Chord)
			{
				var chord = sound as Chord;
				var idx = battuta.Instants.IndexOf(instant);

				if (idx > 0)
				{
					var prevInstant = battuta.Instants[idx - 1];
					foreach (Chord chordInPrevInstant in prevInstant.SoundsOrTabs.Where(sou => sou is Chord))
					{
						if (chordInPrevInstant.NextJoinedChord == chord) chordInPrevInstant.NextJoinedChord = null;
					}
				}
				if (idx < battuta.Instants.Count - 1)
				{
					var nextInstant = battuta.Instants[idx + 1];
					foreach (Chord chordInNextInstant in nextInstant.SoundsOrTabs.Where(sou => sou is Chord))
					{
						if (chordInNextInstant.PrevJoinedChord == chord) chordInNextInstant.PrevJoinedChord = null;
					}
				}
			}

			instant.SoundsOrTabs.Remove(sound);

			if (instant.SoundsOrTabs.Count > 0) return;
			RemoveInstant(instant, battuta);
		}
		public void AddTabToInstant(Tab tab, InstantWithTouches instant, Battuta battuta)
		{
			if (tab == null || instant == null || battuta == null) return;
			var battutaBeingUpdated = _battute.FirstOrDefault(bat => bat == battuta);
			if (battutaBeingUpdated == null) return;
			var instantBeingUpdated = battutaBeingUpdated.Instants.FirstOrDefault(ins => ins == instant);
			if (instantBeingUpdated == null) return;

			instantBeingUpdated.SoundsOrTabs.Add(tab);
		}
		public void RemoveTabFromInstant(Tab tab, InstantWithTouches instant, Battuta battuta)
		{
			if (tab == null || instant == null || battuta == null) return;
			if (!battuta.Instants.Contains(instant) || !instant.SoundsOrTabs.Contains(tab)) return;

			instant.SoundsOrTabs.Remove(tab);

			if (instant.SoundsOrTabs.Count > 0) return;
			RemoveInstant(instant, battuta);
		}
		public void AddToneToChord(Tone tone, Chord chord, InstantWithTouches instant, Battuta battuta)
		{
			if (chord == null || tone == null || instant == null || battuta == null) return;
			var battutaBeingUpdated = _battute.FirstOrDefault(bat => bat == battuta);
			if (battutaBeingUpdated == null) return;
			var instantBeingUpdated = battutaBeingUpdated.Instants.FirstOrDefault(ins => ins == instant);
			if (instantBeingUpdated == null) return;
			var chordBeingUpdated = instantBeingUpdated.SoundsOrTabs.FirstOrDefault(sou => sou == chord) as Chord;
			if (chordBeingUpdated == null) return;
			if (chordBeingUpdated.Tones.Contains(tone)) return;

			chordBeingUpdated.Tones.Add(tone);
		}
		public void RemoveToneFromChord(Chord chord, Tone tone, InstantWithTouches instant, Battuta battuta)
		{
			if (chord == null || tone == null || instant == null || battuta == null) return;
			var battutaBeingUpdated = _battute.FirstOrDefault(bat => bat == battuta);
			if (battutaBeingUpdated == null) return;
			var instantBeingUpdated = battutaBeingUpdated.Instants.FirstOrDefault(ins => ins == instant);
			if (instantBeingUpdated == null) return;
			var chordBeingUpdated = instantBeingUpdated.SoundsOrTabs.FirstOrDefault(sou => sou == chord) as Chord;
			if (chordBeingUpdated == null) return;

			chordBeingUpdated.Tones.Remove(tone);

			if (chordBeingUpdated.Tones.Count > 0) return;
			RemoveSoundFromInstant(chordBeingUpdated, instant, battuta);
		}
		public bool TryLinkChord1ToChord2(Chord chord1, Chord chord2, InstantWithTouches instant1, InstantWithTouches instant2, Battuta battuta)
		{
			bool result = false;

			if (chord1 == null || chord2 == null || instant1 == null || instant2 == null || battuta == null) return result;
			if (!battuta.Instants.Contains(instant1) || !battuta.Instants.Contains(instant2)) return result;
			if (!instant1.SoundsOrTabs.Contains(chord1) || !instant2.SoundsOrTabs.Contains(chord2)) return result;
			if (chord1.Duration.DurataCanonica.CompareTo(DurateCanoniche.Croma) > 0 || chord2.Duration.DurataCanonica.CompareTo(DurateCanoniche.Croma) > 0) return result;

			int instant1Idx = battuta.Instants.IndexOf(instant1);
			int instant2Idx = battuta.Instants.IndexOf(instant2);

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

			if (result) chord1.SetChromaFlagsPositions(chord2.IsChromaFlagsBelow);
			return result;
		}
		public void UnlinkChord1FromChord2(Chord chord1, Chord chord2, InstantWithTouches instant1, InstantWithTouches instant2, Battuta battuta)
		{
			if (chord1 == null || chord2 == null || instant1 == null || instant2 == null || battuta == null) return;
			if (!battuta.Instants.Contains(instant1) || !battuta.Instants.Contains(instant2)) return;
			if (!instant1.SoundsOrTabs.Contains(chord1) || !instant2.SoundsOrTabs.Contains(chord2)) return;

			int instant1Idx = battuta.Instants.IndexOf(instant1);
			int instant2Idx = battuta.Instants.IndexOf(instant2);

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
