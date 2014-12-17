﻿using System;
using AnimatedTextDemo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace AnimatedTextUnitTest
{
    [TestClass]
    public class ChangesFinderUnitTest
    {
        private ChangesFinder _changesFinder = new ChangesFinder();
        [TestMethod]
        public void Swap()
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
        public void Replace()
        {            
            var caseReplace = _changesFinder.Find("heaf","head");
            Assert.AreEqual(caseReplace.Count, 1);
            Assert.AreEqual(caseReplace[0].ChangeType, ChangeType.Replace);
            Assert.AreEqual(caseReplace[0].Character, 'f');
            Assert.AreEqual(caseReplace[0].Character2, 'd');
            Assert.AreEqual(caseReplace[0].Index, 3);
        }

        [TestMethod]
        public void Find()
        { 
        }
    }
}