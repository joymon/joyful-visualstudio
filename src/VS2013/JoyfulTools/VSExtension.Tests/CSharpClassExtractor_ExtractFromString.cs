using System;
using JoyfulTools.VSExtension;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VSExtension.Tests
{
    [TestClass]
    public class CSharpClassExtractor_ExtractFromString
    {
        [TestMethod]
        public void WhenCreateNewCSharpClassExtractor_ReturnNewObject()
        {
            CSharpClassExtractor ext = new CSharpClassExtractor();
            Assert.IsNotNull(ext);
        }
        [TestMethod]
        public void ExtractFromString_ShouldAcceptStringAndReturnArrayOfString()
        {
            CSharpClassExtractor ext = new CSharpClassExtractor();
            string[] classes = ext.ExtractFromString("dummy string");
            Assert.IsNotNull(classes);
        }
        [TestMethod]
        public void WhenThereIsNoClass_ReturnEmptyArray()
        {
            CSharpClassExtractor ext = new CSharpClassExtractor();
            string[] classes = ext.ExtractFromString("dummy string");
            Assert.AreEqual(classes.Length,0,"The expected length is 0.But actual was {0}",classes.Length);
        }
        [TestMethod]
        public void WhenInputContains1Class_ReturnThatClass()
        {
            string input = @"class Test {}";
            string expected = @"class Test {}";

            CSharpClassExtractor ext = new CSharpClassExtractor();

            string actual = ext.ExtractFromString(input)[0];

            Assert.AreEqual(expected, actual, "Expected {0}.{1} But actual was {2}", expected,Environment.NewLine, actual);
        }
        [TestMethod]
        public void WhenInputContainsNamsepaceImportsAnd1Class_ReturnOnlyClass()
        {
            string input = @"
using System;
using System.Windows;
class Test {}";
            string expected = @"class Test {}";

            CSharpClassExtractor ext = new CSharpClassExtractor();

            string actual = ext.ExtractFromString(input)[0];

            Assert.AreEqual(expected, actual, "Expected {0}.{1} But actual was {2}", expected, Environment.NewLine, actual);
        }
        [TestMethod]
        public void WhenInputContains2Classes_ReturnThoseClass()
        {
            string input = @"class Test {}
class Test2 {
    void Test(){};
}";
            string expected = @"class Test2 {
    void Test(){};
}";

            CSharpClassExtractor ext = new CSharpClassExtractor();

            string actual = ext.ExtractFromString(input)[1];

            Assert.AreEqual(expected, actual, "Expected {0}.{1} But actual was {2}", expected, Environment.NewLine, actual);
        }
        [TestMethod]
        public void WhenSecondClassIsPrivate_ShouldNotIncludeInResult()
        {
            string input = @"class Test {}
private class Test2 {
    void Test(){};
}";
            int expected = 1;

            CSharpClassExtractor ext = new CSharpClassExtractor();

            int actualLength = ext.ExtractFromString(input).Length;

            Assert.AreEqual(expected, actualLength, "Expected {0}.{1} But actual was {2}", expected, Environment.NewLine, actualLength);
        }
        [TestMethod]
        public void WhenSecondClassIsProtected_ShouldNotIncludeInResult()
        {
            string input = @"class Test {}
protected class Test2 {
    void Test(){};
}";
            int expected = 1;

            CSharpClassExtractor ext = new CSharpClassExtractor();

            int actualLength = ext.ExtractFromString(input).Length;

            Assert.AreEqual(expected, actualLength, "Expected {0}.{1} But actual was {2}", expected, Environment.NewLine, actualLength);
        }
        [TestMethod]
        public void WhenThereIsInnerClassAndIsPrivate_ShouldNotIncludeInResult()
        {
            string input = @"class Test {
protected class Test2 {
    void Test(){};
}
}";
            int expected = 1;

            CSharpClassExtractor ext = new CSharpClassExtractor();

            int actualLength = ext.ExtractFromString(input).Length;

            Assert.AreEqual(expected, actualLength, "Expected {0}.{1} But actual was {2}", expected, Environment.NewLine, actualLength);
        }
    }
}