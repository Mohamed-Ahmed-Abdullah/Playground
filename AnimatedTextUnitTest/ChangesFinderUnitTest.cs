using System;
using AnimatedTextDemo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace AnimatedTextUnitTest
{
    [TestClass]
    public class ChangesFinderUnitTest
    {
        private ChangesFinder _changesFinder = new ChangesFinder();

        #region Two Changes
        //require rquirez  insert remove 
        [TestMethod]
        public void InsertRemove()
        {
            var caseInsert = _changesFinder.Find("rquirez", "require");
            Assert.AreEqual(caseInsert.Count, 2);
            Assert.AreEqual(caseInsert[0].ChangeType, ChangeType.Insert);
            Assert.AreEqual(caseInsert[0].Character, 'e');
            Assert.AreEqual(caseInsert[0].Index, 1);

            Assert.AreEqual(caseInsert[1].ChangeType, ChangeType.Remove);
            Assert.AreEqual(caseInsert[1].Character, 'z');
            Assert.AreEqual(caseInsert[1].Index, 6);
        }

        //require requriez remove swap 
        [TestMethod]
        public void RemoveSwap()
        {
            var caseInsert = _changesFinder.Find("requriez", "require");
            Assert.AreEqual(caseInsert.Count, 2);
            Assert.AreEqual(caseInsert[0].ChangeType, ChangeType.Remove);
            Assert.AreEqual(caseInsert[0].Character, 'z');
            Assert.AreEqual(caseInsert[0].Index, 7);

            Assert.AreEqual(caseInsert[1].ChangeType, ChangeType.Swap);
            Assert.AreEqual(caseInsert[1].Character, 'r');
            Assert.AreEqual(caseInsert[1].Index, 4);
            Assert.AreEqual(caseInsert[1].Character2, 'i');
            Assert.AreEqual(caseInsert[1].Index2, 5);
        }

        //require rekurie swap replace 
        [TestMethod]
        public void SwapReplace()
        {
            var caseInsert = _changesFinder.Find("rekurie", "require");
            Assert.AreEqual(caseInsert.Count, 2);
        }

        //require rekuir replace insert 
        [TestMethod]
        public void ReplaceInsert()
        {
            var caseInsert = _changesFinder.Find("rekuir", "require");
            Assert.AreEqual(caseInsert.Count, 2);
        }


        #endregion

        #region One Change
        [TestMethod]
        public void SwapStart()
        {
            var caseSwap = _changesFinder.Find("ehad", "head");
            Assert.AreEqual(caseSwap.Count, 1);
            Assert.AreEqual(caseSwap[0].ChangeType, ChangeType.Swap);
            Assert.AreEqual(caseSwap[0].Character,'e');
            Assert.AreEqual(caseSwap[0].Character2,'h');
            Assert.AreEqual(caseSwap[0].Index, 0);
            Assert.AreEqual(caseSwap[0].Index2, 1);
        }

        [TestMethod]
        public void SwapFromFar()
        {
            var caseSwap = _changesFinder.Find("deah", "head");
            Assert.AreEqual(caseSwap.Count, 1);
            Assert.AreEqual(caseSwap[0].ChangeType, ChangeType.Swap);
            Assert.AreEqual(caseSwap[0].Character,'e');
            Assert.AreEqual(caseSwap[0].Character2,'h');
            Assert.AreEqual(caseSwap[0].Index, 0);
            Assert.AreEqual(caseSwap[0].Index2, 1);
        }

        [TestMethod]
        public void SwapMid()
        {
            var caseSwap = _changesFinder.Find("haed", "head");
            Assert.AreEqual(caseSwap.Count, 1);
            Assert.AreEqual(caseSwap[0].ChangeType, ChangeType.Swap);
            Assert.AreEqual(caseSwap[0].Character,'a');
            Assert.AreEqual(caseSwap[0].Character2,'e');
            Assert.AreEqual(caseSwap[0].Index, 1);
            Assert.AreEqual(caseSwap[0].Index2, 2);
        }

        [TestMethod]
        public void SwapLast()
        {
            var caseSwap = _changesFinder.Find("heda", "head");
            Assert.AreEqual(caseSwap.Count, 1);
            Assert.AreEqual(caseSwap[0].ChangeType, ChangeType.Swap);
            Assert.AreEqual(caseSwap[0].Character,'d');
            Assert.AreEqual(caseSwap[0].Character2,'a');
            Assert.AreEqual(caseSwap[0].Index, 2);
            Assert.AreEqual(caseSwap[0].Index2, 3);
        }

        [TestMethod]
        public void InsertFirst()
        {
            var caseInsert = _changesFinder.Find("equire", "require");
            Assert.AreEqual(caseInsert.Count, 1);
            Assert.AreEqual(caseInsert[0].ChangeType, ChangeType.Insert);
            Assert.AreEqual(caseInsert[0].Character, 'r');
            Assert.AreEqual(caseInsert[0].Index, 0);
        }

        [TestMethod]
        public void InsertMiddle()
        {
            var caseInsert = _changesFinder.Find("requre", "require");
            Assert.AreEqual(caseInsert.Count, 1);
            Assert.AreEqual(caseInsert[0].ChangeType, ChangeType.Insert);
            Assert.AreEqual(caseInsert[0].Character, 'i');
            Assert.AreEqual(caseInsert[0].Index, 4);
        }

        [TestMethod]
        public void InsertLast()
        {
            var caseInsert = _changesFinder.Find("requir", "require");
            Assert.AreEqual(caseInsert.Count, 1);
            Assert.AreEqual(caseInsert[0].ChangeType, ChangeType.Insert);
            Assert.AreEqual(caseInsert[0].Character, 'e');
            Assert.AreEqual(caseInsert[0].Index, 6);
        }

        [TestMethod]
        public void RemoveFirst()
        {            
            var caseRemove = _changesFinder.Find("zrequire", "require");
            Assert.AreEqual(caseRemove.Count, 1);
            Assert.AreEqual(caseRemove[0].ChangeType, ChangeType.Remove);
            Assert.AreEqual(caseRemove[0].Character, 'z');
            Assert.AreEqual(caseRemove[0].Index, 0);
        }

        [TestMethod]
        public void RemoveMiddle()
        {            
            var caseRemove = _changesFinder.Find("reqzuire", "require");
            Assert.AreEqual(caseRemove.Count, 1);
            Assert.AreEqual(caseRemove[0].ChangeType, ChangeType.Remove);
            Assert.AreEqual(caseRemove[0].Character, 'z');
            Assert.AreEqual(caseRemove[0].Index, 3);
        }

        [TestMethod]
        public void RemoveLast()
        {            
            var caseRemove = _changesFinder.Find("requirez", "require");
            Assert.AreEqual(caseRemove.Count, 1);
            Assert.AreEqual(caseRemove[0].ChangeType, ChangeType.Remove);
            Assert.AreEqual(caseRemove[0].Character, 'z');
            Assert.AreEqual(caseRemove[0].Index, 7);
        }

        [TestMethod]
        public void ReplaceFirst()
        {
            var caseReplace = _changesFinder.Find("zead", "head");
            Assert.AreEqual(caseReplace.Count, 1);
            Assert.AreEqual(caseReplace[0].ChangeType, ChangeType.Replace);
            Assert.AreEqual(caseReplace[0].Character, 'z');
            Assert.AreEqual(caseReplace[0].Character2, 'h');
            Assert.AreEqual(caseReplace[0].Index, 0);
        }

        [TestMethod]
        public void ReplaceMid()
        {
            var caseReplace = _changesFinder.Find("requere", "require");
            Assert.AreEqual(caseReplace.Count, 1);
            Assert.AreEqual(caseReplace[0].ChangeType, ChangeType.Replace);
            Assert.AreEqual(caseReplace[0].Character, 'e');
            Assert.AreEqual(caseReplace[0].Character2, 'i');
            Assert.AreEqual(caseReplace[0].Index, 4);
        }

        [TestMethod]
        public void ReplaceLast()
        {            
            var caseReplace = _changesFinder.Find("heaf","head");
            Assert.AreEqual(caseReplace.Count, 1);
            Assert.AreEqual(caseReplace[0].ChangeType, ChangeType.Replace);
            Assert.AreEqual(caseReplace[0].Character, 'f');
            Assert.AreEqual(caseReplace[0].Character2, 'd');
            Assert.AreEqual(caseReplace[0].Index, 3);
        }
        #endregion 

    }
}