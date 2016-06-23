using Pentagram.Utilz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Utilz;

namespace Pentagram.PersistentData
{
	[DataContract]
	public sealed class Battuta : ObservableData
	{
		private Chiavi _chiave = All.DEFAULT_CHIAVE;
		[DataMember]
		public Chiavi Chiave { get { return _chiave; } private set { if (_chiave == value) return; _chiave = value; RaisePropertyChanged(); } }

		private Misura _misura;
		[DataMember]
		public Misura Misura { get { return _misura; } private set { if (_misura?.CompareTo(value) == 0) return; _misura = value; RaisePropertyChanged(); } }

		// we cannot make this readonly because it is serialised. we only use the setter for serialising.
		private SwitchableObservableCollection<InstantWithTouches> _instants = new SwitchableObservableCollection<InstantWithTouches>();
		[DataMember]
		public SwitchableObservableCollection<InstantWithTouches> Instants { get { return _instants; } private set { _instants = value; } }

		public Battuta(Chiavi chiave, Misura misura)
		{
			Chiave = chiave;
			Misura = misura;
		}
	}
}
