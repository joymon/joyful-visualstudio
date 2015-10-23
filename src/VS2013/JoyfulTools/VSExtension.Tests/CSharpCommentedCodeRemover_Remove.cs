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
    public class CSharpCommentedCodeRemover_Remove
    {
        [TestMethod]
        public void WhenInputIsNotInCodeFormat_ReturnInput()
        {
            string inputCode = @"#$@%$;
How are you doing hello";
            string expected = @"#$@%$;
How are you doing hello";
            string actualOutput = CSharpCommentedCodeRemover.Remove(inputCode);
            Assert.AreEqual(expected, actualOutput);
        }
        [TestMethod]
        public void WhenInputContainsSingleLineComment_Remove()
        {
            string inputCode = @"var a;
//hello
a++;";
            string expected = @"var a;

a++;";
            string actualOutput = CSharpCommentedCodeRemover.Remove(inputCode);
            Assert.AreEqual(expected, actualOutput);

        }
        [TestMethod]
        public void WhenInputContainsMultipleCommentLines_Remove()
        {
            string inputCode = @"var a;
//hello
a++;//hi
a++;
//how are you";
            string expected = @"var a;

a++;
a++;
";
            string actualOutput = CSharpCommentedCodeRemover.Remove(inputCode);
            Assert.AreEqual(expected, actualOutput);
        }
        [TestMethod]
        public void WhenInputContainsXMLComment_ShouldNotRemove()
        {
            string inputCode = @"        /// <summary>
        /// This is to print
        /// </summary>
        void Print()
        {
            var a=10;a++;
        }";
            string expected = @"        /// <summary>
        /// This is to print
        /// </summary>
        void Print()
        {
            var a=10;a++;
        }";
            string actualOutput = CSharpCommentedCodeRemover.Remove(inputCode);
            Assert.AreEqual(expected, actualOutput);
        }
    }
}
