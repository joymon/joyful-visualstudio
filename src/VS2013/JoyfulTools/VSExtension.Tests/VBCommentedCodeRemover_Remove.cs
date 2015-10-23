using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoyfulTools.VSExtension;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VSExtension.Tests
{
    [TestClass]
    public class VBCommentedCodeRemover_Remove
    {
        [TestMethod]
        public void WhenInputIsNotInCodeFormat_ReturnInput()
        {
            string inputCode = @"#$@%$;
How are you doing hello";
            string expected = @"#$@%$;
How are you doing hello";
            string actualOutput = VBCommentedCodeRemover.Remove(inputCode);
            Assert.AreEqual(expected, actualOutput);
        }

    }
}
