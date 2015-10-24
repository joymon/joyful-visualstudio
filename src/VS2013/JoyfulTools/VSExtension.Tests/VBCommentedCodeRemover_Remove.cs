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
        [TestMethod]
        public void WhenInputContainsSingleLineComment_Remove()
        {
            string inputCode = @"dim a as Int32
'How are you doing hello
a=a+1";
            string expected = @"dim a as Int32

a=a+1";
            string actualOutput = VBCommentedCodeRemover.Remove(inputCode);
            Assert.AreEqual(expected, actualOutput);
        }
        [TestMethod]
        public void WhenInputContainsXMLComment_ShouldNotRemove()
        {
            string inputCode = @"dim a as Int32
'''<summary>How are you doing hello</summary>
private Sub Test(a as Int32)
a=a+1
End Sub";
            string expected = @"dim a as Int32
'''<summary>How are you doing hello</summary>
private Sub Test(a as Int32)
a=a+1
End Sub";
            string actualOutput = VBCommentedCodeRemover.Remove(inputCode);
            Assert.AreEqual(expected, actualOutput);
        }
        [TestMethod]
        public void WhenInputContainsXMLCommentWithWhiteSpace_ShouldNotRemove()
        {

            string inputCode = @"dim a as Int32
''' <summary>
''' How are you doing hello
''' </summary> 

private Sub Test(a as Int32)
a=a+1
End Sub";
            string expected = @"dim a as Int32
''' <summary>
''' How are you doing hello
''' </summary> 

private Sub Test(a as Int32)
a=a+1
End Sub";
            string actualOutput = VBCommentedCodeRemover.Remove(inputCode);
            Assert.AreEqual(expected, actualOutput);
        }
        [TestMethod]
        public void WhenInputContains2XMLCommentNodes_ShouldNotRemove()
        {
            string inputCode = "''' <summary>''' fdfdf\r\n''' </summary>\r\n''' <remarks></remarks>\r\n";
            string expected = "''' <summary>''' fdfdf\r\n''' </summary>\r\n''' <remarks></remarks>\r\n";
            string actualOutput = VBCommentedCodeRemover.Remove(inputCode);
            Assert.AreEqual(expected, actualOutput);
        }
        [TestMethod]
        public void WhenInputContains3SingleQuitesToMimicXMLComment_ShouldNotRemove()
        {
            string inputCode = @"dim a as Int32
private Sub Test(a as Int32)
'''How are you doing hello
a=a+1
End Sub";
            string expected = @"dim a as Int32
private Sub Test(a as Int32)
'''How are you doing hello
a=a+1
End Sub";
            string actualOutput = VBCommentedCodeRemover.Remove(inputCode);
            Assert.AreEqual(expected, actualOutput);
        }
        [TestMethod]
        public void WhenInputContainsValidAndInvalidXMLComments_ShouldNotRemove()
        {
            string inputCode = @"dim a as Int32
'''How are you doing hello
'''<summary>Proper</summary>
private Sub Test(a as Int32)
a=a+1
End Sub";
            string expected = @"dim a as Int32
'''How are you doing hello
'''<summary>Proper</summary>
private Sub Test(a as Int32)
a=a+1
End Sub";
            string actualOutput = VBCommentedCodeRemover.Remove(inputCode);
            Assert.AreEqual(expected, actualOutput);
        }
    }
}
