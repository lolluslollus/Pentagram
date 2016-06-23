using Pentagram.Utilz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pentagram.PersistentData
{
	[DataContract]
	public sealed class Misura : ObservableData, IComparable
	{
		private uint _num = 4;
		public uint Num { get { return _num; } set { if (_num == value) return; _num = value; RaisePropertyChanged(); } }
		private uint _den = 4;
		public uint Den { get { return _den; } set { if (_den == value) return; _den = value; RaisePropertyChanged(); } }

		public Misura(uint num = 4, uint den = 4)
		{
			_num = num;
			_den = den;
		}

		public int CompareTo(object obj)
		{
			var otherMisura = obj as Misura;
			if (otherMisura._num == _num && otherMisura._den == _den) return 0;
			return 1;
		}
	}
}
