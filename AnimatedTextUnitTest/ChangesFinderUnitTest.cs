using System;
using AnimatedTextDemo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace AnimatedTextUnitTest
{
    [TestClass]
    public class ChangesFinderUnitTest
    {
        [TestMethod]
        public void Find()
        { 
            var changesFinder = new ChangesFinder();

            var caseSwap = changesFinder.Find("haed", "head");
            Assert.AreEqual(caseSwap.Count, 1);
            Assert.AreEqual(caseSwap[0].ChangeType, ChangesFinder.ChangeType.Swap);
            Assert.AreEqual(caseSwap[0].Character,'a');
            Assert.AreEqual(caseSwap[0].Character2,'e');
            Assert.AreEqual(caseSwap[0].Index, 1);
            Assert.AreEqual(caseSwap[0].Index2, 2);

            var caseInsert = changesFinder.Find("requir", "require");
            Assert.AreEqual(caseInsert.Count, 1);
            Assert.AreEqual(caseInsert[0].ChangeType, ChangesFinder.ChangeType.Insert);
            Assert.AreEqual(caseInsert[0].Character, 'e');
            Assert.AreEqual(caseInsert[0].Index, 6);

            var caseRemove = changesFinder.Find("requirez", "require");
            Assert.AreEqual(caseRemove.Count, 1);
            Assert.AreEqual(caseRemove[0].ChangeType, ChangesFinder.ChangeType.Remove);
            Assert.AreEqual(caseRemove[0].Character, 'z');
            Assert.AreEqual(caseRemove[0].Index, 7);

            var caseReplace = changesFinder.Find("heaf","head");
            Assert.AreEqual(caseReplace.Count, 1);
            Assert.AreEqual(caseReplace[0].ChangeType, ChangesFinder.ChangeType.Replace);
            Assert.AreEqual(caseReplace[0].Character, 'f');
            Assert.AreEqual(caseReplace[0].Character2, 'd');
            Assert.AreEqual(caseReplace[0].Index, 3);
        }
    }
}