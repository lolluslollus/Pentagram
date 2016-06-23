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
	public sealed class InstantWithTouches : ObservableData
	{
		// we cannot make this readonly because it is serialised. we only use the setter for serialising.
		private SwitchableObservableCollection<SoundOrTab> _soundsOrTabs = new SwitchableObservableCollection<SoundOrTab>();
		[DataMember]
		public SwitchableObservableCollection<SoundOrTab> SoundsOrTabs { get { return _soundsOrTabs; } private set { _soundsOrTabs = value; } }
	}
}
