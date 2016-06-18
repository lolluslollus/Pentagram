using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Utilz.Data;

namespace Pentagram.PersistentData
{
	[DataContract]
	public class Note : Touch, IComparable
	{
		private uint _ottava = 3;
		[DataMember]
		public uint Ottava { get { return _ottava; } private set { _ottava = value < 10 ? value : 10; RaisePropertyChanged(); } }
		private NoteBianche _notaBianca = NoteBianche.@do;
		[DataMember]
		public NoteBianche NotaBianca { get { return _notaBianca; } private set { _notaBianca = value; RaisePropertyChanged(); } }
		private Accidenti _accidente = Accidenti.Nil;
		[DataMember]
		public Accidenti Accidente { get { return _accidente; } private set { _accidente = value; RaisePropertyChanged(); } }
		private SegniSuNote _segno = SegniSuNote.None;
		[DataMember]
		public SegniSuNote Segno { get { return _segno; } private set { _segno = value; RaisePropertyChanged(); } }

		public Note(DurateCanoniche durataCanonica, uint ottava, NoteBianche notaBianca, Accidenti accidente) : base(durataCanonica)
		{
			Ottava = ottava;
			NotaBianca = notaBianca;
			Accidente = accidente;
		}
		public Note(DurateCanoniche durataCanonica, uint ottava, NoteBianche notaBianca, Accidenti accidente, SegniSuNote segno) : this(durataCanonica, ottava, notaBianca, accidente)
		{
			Segno = segno;
		}

		public int CompareTo(object obj)
		{
			var otherNote = obj as Note;

			if (Ottava > otherNote.Ottava) return +1;
			if (Ottava < otherNote.Ottava) return -1;
			if ((int)NotaBianca > (int)otherNote.NotaBianca) return +1;
			if ((int)NotaBianca < (int)otherNote.NotaBianca) return -1;
			return 0;
		}
	}
	[DataContract]
	public class Pause : Touch
	{
		public Pause(DurateCanoniche durataCanonica) : base(durataCanonica) { }
	}
	[DataContract]
	public abstract class Touch : ObservableData
	{
		[IgnoreDataMember]
		public uint Durata64
		{
			get
			{
				switch (_durataCanonica)
				{
					case DurateCanoniche.Breve:
						return GetDurata64(128);
					case DurateCanoniche.Semibreve:
						return GetDurata64(64);
					case DurateCanoniche.Minima:
						return GetDurata64(32);
					case DurateCanoniche.Semiminima:
						return GetDurata64(16);
					case DurateCanoniche.Croma:
						return GetDurata64(8);
					case DurateCanoniche.Semicroma:
						return GetDurata64(4);
					case DurateCanoniche.Biscroma:
						return GetDurata64(2);
					case DurateCanoniche.Semibiscroma:
						return GetDurata64(1);
					default:
						return 1;
				}
			}
		}
		private uint GetDurata64(uint basicDurata)
		{
			return basicDurata + ((2 ^ PuntiDiValore - 1) / 2 ^ PuntiDiValore) * basicDurata;
		}
		private DurateCanoniche _durataCanonica = DurateCanoniche.Semibreve;
		[DataMember]
		public DurateCanoniche DurataCanonica { get { return _durataCanonica; } private set { _durataCanonica = value; RaisePropertyChanged(); } }
		private uint _puntiDiValore = 0;
		[DataMember]
		public uint PuntiDiValore { get { return _puntiDiValore; } private set { _puntiDiValore = value > 3 ? 3 : value < 0 ? 0 : value; RaisePropertyChanged(); } }

		public Touch(DurateCanoniche durataCanonica)
		{
			DurataCanonica = durataCanonica;
		}
	}
}
