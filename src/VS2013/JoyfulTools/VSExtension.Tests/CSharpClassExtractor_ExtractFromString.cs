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
            
        }
    }
}
