using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JoyfulTools.VSExtension;
namespace VSExtension.Tests
{
    [TestClass]
    public class MultipleBlankLinesRemover_GetReplacedText
    {
        [TestMethod]
        public void WhenInputHasMultipleConsecutiveBlankLines_RemoveMultipleBlankLinesTo1()
        {
            MultipleBlankLinesRemover.GetReplacedText("");
        }
    }
}
