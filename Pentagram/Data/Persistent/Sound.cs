using Pentagram.Utilz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pentagram.PersistentData
{
	//[DataContract(IsReference=true)]
	//public sealed class Note : Sound
	//{
	//	private SegniSuNote _segno = SegniSuNote.Nil;
	//	[DataMember]
	//	public SegniSuNote Segno { get { return _segno; } private set { _segno = value; RaisePropertyChanged(); } }

	//	private Tone _tone = null;
	//	[DataMember]
	//	public Tone Tone { get { return _tone; } private set { _tone = value; RaisePropertyChanged(); } }

	//	public Note(Duration duration, SegniSuNote segno, Tone tone) : base(duration)
	//	{
	//		if (tone == null) throw new ArgumentOutOfRangeException("Note ctor wants a tone");
	//		Segno = segno;
	//		Tone = tone;
	//	}
	//}

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
		private uint _ottava = 3;
		[DataMember]
		public uint Ottava { get { return _ottava; } private set { if (_ottava == value) return; _ottava = value < 10 ? value : 10; RaisePropertyChanged(); } }
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

			if (Ottava > otherTone.Ottava) return +1;
			if (Ottava < otherTone.Ottava) return -1;
			if ((int)NotaBianca > (int)otherTone.NotaBianca) return +1;
			if ((int)NotaBianca < (int)otherTone.NotaBianca) return -1;
			return 0;
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
