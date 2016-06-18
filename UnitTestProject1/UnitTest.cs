using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Pentagram.PersistentData;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestNoteComparer()
        {
			Note one = new Note(DurateCanoniche.Biscroma, 2, NoteBianche.@do, Accidenti.Bequadro);
			Note two = new Note(DurateCanoniche.Biscroma, 2, NoteBianche.sol, Accidenti.Bequadro);
			Assert.IsTrue(two.CompareTo(one) > 0);
		}
    }
}
