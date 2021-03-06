﻿using Pentagram.Utilz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pentagram.PersistentData
{
	[DataContract(IsReference = true)]
	// [DataContract]
	public sealed class Pause : Sound
	{
		private SegniSuNote _segno = SegniSuNote.Nil;
		[DataMember]
		public SegniSuNote Segno { get { return _segno; } private set { if (_segno == value) return; _segno = value; RaisePropertyChanged(); } }

		public Pause(Duration duration, SegniSuNote segno) : base(duration)
		{
			Segno = segno;
		}
	}

	[DataContract(IsReference = true)]
	// [DataContract]
	public abstract class Sound : SoundOrTab
	{
		private Duration _duration = null;
		[DataMember]
		public Duration Duration { get { return _duration; } private set { if (_duration == value) return; _duration = value; RaisePropertyChanged(); } }

		public Sound(Duration duration)
		{
			if (duration == null) throw new ArgumentOutOfRangeException("Sound ctor wants a duration");
			Duration = duration;
		}
	}

	[DataContract]
	public sealed class Duration : ObservableData, IComparable
	{
		public double GetDurata64()
		{
			switch (_durataCanonica)
			{
				case DurateCanoniche.Breve:
					return GetDurata64_2(128);
				case DurateCanoniche.Semibreve:
					return GetDurata64_2(64);
				case DurateCanoniche.Minima:
					return GetDurata64_2(32);
				case DurateCanoniche.Semiminima:
					return GetDurata64_2(16);
				case DurateCanoniche.Croma:
					return GetDurata64_2(8);
				case DurateCanoniche.Semicroma:
					return GetDurata64_2(4);
				case DurateCanoniche.Biscroma:
					return GetDurata64_2(2);
				case DurateCanoniche.Semibiscroma:
					return GetDurata64_2(1);
				default:
					return 1;
			}
		}
		private double GetDurata64_2(double basicDurata)
		{
			uint pv = (uint)PuntiDiValore;
			double result = basicDurata;
			for (uint i = 1; i <= pv; i++)
			{
				result += result * Math.Pow(.5, i);
			}
			return result;
		}
		private DurateCanoniche _durataCanonica = DurateCanoniche.Semibreve;
		[DataMember]
		public DurateCanoniche DurataCanonica { get { return _durataCanonica; } private set { if (_durataCanonica == value) return; _durataCanonica = value; RaisePropertyChanged(); } }
		private PuntiDiValore _puntiDiValore = PuntiDiValore.Nil;
		[DataMember]
		public PuntiDiValore PuntiDiValore { get { return _puntiDiValore; } private set { if (_puntiDiValore == value) return; _puntiDiValore = value; RaisePropertyChanged(); } }

		public Duration(DurateCanoniche durataCanonica)
		{
			DurataCanonica = durataCanonica;
		}
		public Duration(DurateCanoniche durataCanonica, PuntiDiValore puntiDiValore) : this(durataCanonica)
		{
			PuntiDiValore = puntiDiValore;
		}

		public int CompareTo(object obj)
		{
			var otherDuration = obj as Duration;
			if (otherDuration == null) throw new ArgumentException("Duration.CompareTo() needs a valid duration to compare to");
			return GetDurata64().CompareTo(otherDuration.GetDurata64());
		}
	}

	[DataContract]
	public sealed class Tone : ObservableData, IComparable
	{
		public const uint MAX_OTTAVA = 10;
		private uint _ottava = 3;
		[DataMember]
		public uint Ottava { get { return _ottava; } private set { if (_ottava == value) return; _ottava = value < MAX_OTTAVA ? value : MAX_OTTAVA; RaisePropertyChanged(); } }
		private NoteBianche _notaBianca = NoteBianche.@do;
		[DataMember]
		public NoteBianche NotaBianca { get { return _notaBianca; } private set { if (_notaBianca == value) return; _notaBianca = value; RaisePropertyChanged(); } }
		private Accidenti _accidente = Accidenti.Nil;
		[DataMember]
		public Accidenti Accidente { get { return _accidente; } private set { if (_accidente == value) return; _accidente = value; RaisePropertyChanged(); } }

		public Tone(uint ottava, NoteBianche notaBianca, Accidenti accidente)
		{
			Ottava = ottava;
			NotaBianca = notaBianca;
			Accidente = accidente;
		}

		public int CompareTo(object obj)
		{
			var otherTone = obj as Tone;
			if (otherTone == null) throw new ArgumentNullException("Tone.CompareTo() needs a tone");

			var one = this;
			var two = otherTone;
			if (one._accidente == Accidenti.Nil)
			{
				one = Clone(this);
				one._accidente = Accidenti.Bequadro;
			}
			if (two._accidente == Accidenti.Nil)
			{
				two = Clone(otherTone);
				two._accidente = Accidenti.Bequadro;
			}

			if (one._ottava > two._ottava) return +1;
			if (one._ottava < two._ottava) return -1;
			if ((int)one._notaBianca > (int)two._notaBianca) return +1;
			if ((int)one._notaBianca < (int)two._notaBianca) return -1;
			if (one._accidente == two._accidente) return 0;
			if ((int)one._accidente > (int)two._accidente) return +1;
			return -1;
		}

		public static Tone Clone(Tone tone)
		{
			if (tone == null) throw new ArgumentNullException("Tone.Clone() needs a tone");
			return new Tone(tone._ottava, tone._notaBianca, tone._accidente);
		}
	}

	[DataContract(IsReference = true)]
	// [DataContract]
	public class Tab : SoundOrTab
	{
		private TabSymbols _tabSymbol = TabSymbols.Nil;
		[DataMember]
		public TabSymbols TabSymbol { get { return _tabSymbol; } private set { if (_tabSymbol == value) return; _tabSymbol = value; RaisePropertyChanged(); } }

		public Tab(TabSymbols tabSymbol = TabSymbols.Nil)
		{
			TabSymbol = tabSymbol;
		}
	}

	[DataContract(IsReference = true)]
	// [DataContract]
	public abstract class SoundOrTab : ObservableData { }
}
