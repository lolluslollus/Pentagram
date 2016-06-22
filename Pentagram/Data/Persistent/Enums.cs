using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pentagram.PersistentData
{
	//public enum Ritmi { qq, tq, dq }
	public enum Chiavi { Violino, Basso }
	public enum DurateCanoniche { Semibiscroma, Biscroma, Semicroma, Croma, Semiminima, Minima, Semibreve, Breve }
	public enum PuntiDiValore { Nil, One, Two, Three }
	public enum NoteBianche { @do, re, mi, fa, sol, la, si }
	public enum SegniSuNote { Nil, Accento, Trillo }
	public enum Accidenti { DoppioBemolle, Bemolle, Nil, Bequadro, Diesis, DoppioDiesis }
	public enum TabSymbols { Nil, Chiave, Armatura, Misura, TwoVerticalBars, Refrain }
}
